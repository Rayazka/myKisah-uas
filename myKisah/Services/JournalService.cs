using myKisah.Automata;
using myKisah.Interfaces;
using myKisah.Models;
using myKisah.Utils;

namespace myKisah.Services;

// Journal System
// PENANGGUNG JAWAB: azka
// TEKNIK: Automata + Runtime Configuration
    // State machine siap dipakai untuk method Transition() (submit, save, reject, reset).
    // Runtime configuration untuk validasi input di CreateJournal (content length dan mood valid).
        // MaxContentLength dan ValidMoods dibaca dari appsettings.json section "JournalConfig".

// ** Penjelasan:
    // Business logic untuk Journal management.

// Implement ServiceBase untuk logging, IJournalService untuk kontrak service.
public class JournalService : ServiceBase, IJournalService
{
    // Repository untuk akses data, state machine untuk transisi state, configuration untuk runtime config
    private readonly IJournalRepository _repository; // Repository untuk akses data Journal
    private readonly JournalStateMachine _stateMachine; // State machine untuk mengatur lifecycle Journal
    private readonly IConfiguration _configuration; // Configuration untuk baca runtime config (max content length, valid moods)

// Constructor terima repository, state machine, dan configuration v
    public JournalService(IJournalRepository repository, JournalStateMachine stateMachine, IConfiguration configuration)
    {
        _repository = repository;
        _stateMachine = stateMachine;
        _configuration = configuration;
    }

    // Untuk logging di ServiceBase, override ServiceName jadi "JournalService"
    protected override string ServiceName => "JournalService";

    // Implementasi method IJournalService (CreateJournal, GetJournalsByUser, DeleteJournal)
    public Journal CreateJournal(string userId, string title, string content, MoodType mood)
    {
        // Input tidak boleh kosong
        Validator. ValidateNotEmpty(userId, "UserId");
        Validator.ValidateNotEmpty(title, "Title");
        Validator.ValidateNotEmpty(content, "Content");
    
        // Mood harus valid (sesuai enum)
        Validator.ValidateInEnum(mood, "Mood");
        var validMoods = _configuration.GetSection("JournalConfig:ValidMoods").Get<string[]>() ?? Array.Empty<string>(); // Baca valid moods dari config. Array.Empty untuk handle null
        if (!validMoods.Contains(mood.ToString()))
        {
            throw new ArgumentException($"Mood {mood} tidak valid. Valid moods: {string.Join(", ", validMoods)}");
        }

        // Panjang content tidak boleh melebihi batas yang ditentukan di config
        int maxLength = _configuration.GetValue<int>("JournalConfig:MaxContentLength");
        if(content.Length > maxLength)
        {
            throw new ArgumentException($"Content tidak boleh lebih dari {maxLength} karakter");
        }

        // Buat journal baru dengan state Draft (state awal)
        Journal journal = new Journal
        {
            UserId = userId,
            Title = title,
            Content = content,
            Mood = mood
        };

        // Simpan journal ke repository (database)
        _repository.Add(journal);
        return journal;
    }

    // Untuk mendapatkan semua journal milik user tertentu, panggil repository dengan filter UserId
    public IEnumerable<Journal> GetJournalsByUser(string userId)
    {
        Validator.ValidateNotEmpty(userId, "UserId"); // Validasi input tidak boleh kosong
        return _repository.GetByUserId(userId); // Panggil repository untuk dapatkan journal berdasarkan UserId
    }

    // Untuk menghapus journal
    public bool DeleteJournal(string id)
    {
        // Cek apakah journal dengan Id tersebut ada. Jika tidak ada, lempar exception. Jika ada, panggil repository untuk hapus.
        Journal? journal = _repository.GetById(id);
        Validator.ValidateExists(journal, $"Journal dengan Id '{id}'"); 

        // Hapus kalau ada
        _repository.Delete(id); 
        return true; 
    }
}
