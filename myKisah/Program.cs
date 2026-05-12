using myKisah.Components;
using myKisah.Utils;
using myKisah.Interfaces;
using myKisah.Repositories;
using myKisah.Services;
using myKisah.Controllers;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Config 
builder.Services.AddSingleton<FilePathConfig>();

//  Utils 
builder.Services.AddSingleton<JsonStorageHelper>();
builder.Services.AddSingleton<ValidationHelper>();

//  Automata 
builder.Services.AddSingleton<myKisah.Automata.JournalStateMachine>();

//  Repositories 
builder.Services.AddSingleton<IUserRepository, JsonUserRepository>();
builder.Services.AddSingleton<IJournalRepository, JsonJournalRepository>();
builder.Services.AddSingleton<ICharacterRepository, JsonCharacterRepository>();
builder.Services.AddSingleton<ICharacterResponseRepository, JsonCharacterResponseRepository>();

// Services 
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJournalService, JournalService>();
builder.Services.AddScoped<ICharacterService, CharacterService>();

var app = builder.Build();

//  Global Error Handling 
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

// Map controllers (API endpoints) + Blazor components
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
