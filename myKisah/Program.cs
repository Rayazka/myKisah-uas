using myKisah.Components;
using myKisah.Middleware;
using myKisah.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ═══════════════════════════════════════════════════════════
// FASE 1: Registrasi Dependency Injection
// Semua service/repository/helper yang dibuat di Fase 1
// didaftarkan di DI container di sini.
//
// Catatan: Repository dan Service di-register meskipun
// implementasinya belum ada (Fase 2). Ini mencegah compile error
// saat Fase 2 merge — selama interface sudah ada, DI bisa diatur.
// ═══════════════════════════════════════════════════════════

// --- Config ---
builder.Services.AddSingleton<FilePathConfig>();

// --- Utils ---
builder.Services.AddSingleton<JsonStorageHelper>();
builder.Services.AddSingleton<ValidationHelper>();

// --- Automata ---
builder.Services.AddSingleton<myKisah.Automata.JournalStateMachine>();

// --- Repositories (Fase 2: uncomment saat implementasi sudah ada) ---
// builder.Services.AddSingleton<IUserRepository, JsonUserRepository>();
// builder.Services.AddSingleton<IJournalRepository, JsonJournalRepository>();
// builder.Services.AddSingleton<ICharacterRepository, JsonCharacterRepository>();
// builder.Services.AddSingleton<ICharacterResponseRepository, JsonCharacterResponseRepository>();

// --- Services (Fase 2: uncomment saat implementasi sudah ada) ---
// builder.Services.AddScoped<IUserService, UserService>();
// builder.Services.AddScoped<IJournalService, JournalService>();
// builder.Services.AddScoped<ICharacterService, CharacterService>();

var app = builder.Build();

// --- Global Error Handling Middleware (Jojo - Fase 1) ---
// Tempatkan PALING ATAS agar semua exception tertangkap
app.UseMiddleware<ErrorHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
