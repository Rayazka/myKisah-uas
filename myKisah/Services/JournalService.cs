using myKisah.Automata;
using myKisah.Interfaces;
using myKisah.Models;
using myKisah.Utils;

namespace myKisah.Services;

// ═══════════════════════════════════════════════════════════
// KELAS: JournalService
// DOMAIN: Journal System
// TEKNIK: Automata + Runtime Configuration
// PENANGGUNG JAWAB: Rayazka Aris
// ═══════════════════════════════════════════════════════════
//
// 📘 APA INI?
// Business logic untuk Journal management.
// Extends ServiceBase. Implements IJournalService.
//
// 🧠 DUA TEKNIK DI SINI:
//
// 1. AUTOMATA — JournalStateMachine:
//    State machine dipakai untuk validasi transisi journal.
//    Di Fase CLO2, CreateJournal selalu membuat journal dengan State=Draft.
//    State machine siap dipakai untuk method Transition() (submit, save, reject, reset).
//    Kalau kamu mau tambah method Transition di service ini, gunakan _stateMachine.Transition().
//
// 2. RUNTIME CONFIGURATION — IConfiguration:
//    MaxContentLength dan ValidMoods dibaca dari appsettings.json section "JournalConfig".
//    Ini TIDAK di-hardcode. Kalau mau ubah batas 2000 → 5000, cukup edit appsettings.json.
//
// 📋 TODO:
// [ ] 1. Constructor: terima IJournalRepository + JournalStateMachine + IConfiguration via DI
//        - Simpan semua di private field
//
// [ ] 2. Override ServiceName → "JournalService"
//
// [ ] 3. Implement CreateJournal(userId, title, content, mood):
//        Precondition checks:
//        a. Validator.ValidateNotEmpty(userId, "UserId")
//        b. Validator.ValidateNotEmpty(title, "Title")
//        c. Validator.ValidateNotEmpty(content, "Content")
//        d. Validator.ValidateInEnum(mood, "Mood")
//
//        Runtime config checks (baca dari config!):
//        e. int maxLength = _config.GetValue<int>("JournalConfig:MaxContentLength")
//        f. var validMoods = _config.GetSection("JournalConfig:ValidMoods").Get<string[]>()
//        g. Jika content.Length > maxLength → throw ArgumentException
//        h. Jika !validMoods.Contains(mood.ToString()) → throw ArgumentException
//
//        Create:
//        i. var journal = new Journal { UserId = userId, Title = title,
//               Content = content, Mood = mood }
//        j. _repository.Add(journal)  (Id, CreatedAt, State=Draft di-set di repo)
//        k. Return journal
//
// [ ] 4. Implement GetJournalsByUser(string userId):
//        a. Validator.ValidateNotEmpty(userId, "UserId")
//        b. return _repository.GetByUserId(userId)
//
// [ ] 5. Implement DeleteJournal(string id):
//        a. var journal = _repository.GetById(id)
//        b. Validator.ValidateExists(journal, $"Journal dengan Id '{id}'")
//        c. _repository.Delete(id)
//        d. Return true
//
// Tips:
// - _config.GetValue<int>("JournalConfig:MaxContentLength") untuk baca config
// - GetSection(...).Get<string[]>() untuk baca array ValidMoods
// - JournalStateMachine siap dipakai untuk method Transition (tambahan opsional)
//
// Referensi: Task_myKisah.md baris 61-65, 150-164, 201-208

public class JournalService : ServiceBase, IJournalService
{
    private readonly IJournalRepository _repository;
    private readonly JournalStateMachine _stateMachine;
    private readonly IConfiguration _configuration;

    public JournalService(
        IJournalRepository repository,
        JournalStateMachine stateMachine,
        IConfiguration configuration)
    {
        _repository = repository;
        _stateMachine = stateMachine;
        _configuration = configuration;
    }

    protected override string ServiceName => throw new NotImplementedException("TODO: return 'JournalService'");

    public Journal CreateJournal(string userId, string title, string content, MoodType mood)
    {
        throw new NotImplementedException("TODO: CreateJournal - validasi input + runtime config + simpan");
    }

    public IEnumerable<Journal> GetJournalsByUser(string userId)
    {
        throw new NotImplementedException("TODO: GetJournalsByUser - validasi + return repo.GetByUserId");
    }

    public bool DeleteJournal(string id)
    {
        throw new NotImplementedException("TODO: DeleteJournal - validasi exists + hapus");
    }
}
