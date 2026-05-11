using myKisah.Models;

namespace myKisah.Interfaces;

// Journal System
// PENANGGUNG JAWAB: Azka
// TEKNIK: Automata + Runtime Configuration (digunakan oleh JournalController)
    // 1. AUTOMATA: JournalStateMachine memvalidasi transisi state (Submit, Save, Reject, Reset)
    // 2. RUNTIME CONFIG: MaxContentLength dan ValidMoods dibaca dari appsettings.json
    //    (tidak di-hardcode di kode)

// ** PENJELASAN:
// Kontrak business logic untuk Journal management.
// Dipanggil oleh JournalController (API layer).
//
// ** Method:
// - CreateJournal(userId, title, content, mood): Buat journal baru dengan State=Draft
// - GetJournalsByUser(userId)                  : Ambil semua journal milik satu user
// - DeleteJournal(id)                         : Hapus journal berdasarkan ID
//

// TODO Fase 2:
// 1. Buat kelas JournalService di folder Services/
// 2. Extend ServiceBase
// 3. Inject IJournalRepository + JournalStateMachine + IConfiguration
// 4. CreateJournal: validasi title/content tidak kosong, content.Length <= MaxContentLength,
//        mood valid di ValidMoods (baca dari config)
// 5. GetJournalsByUser: panggil repo.GetByUserId
// 6. DeleteJournal: validasi ID exists, panggil repo.Delete

public interface IJournalService
{
    Journal CreateJournal(string userId, string title, string content, MoodType mood);
    IEnumerable<Journal> GetJournalsByUser(string userId);
    bool DeleteJournal(string id);
}
