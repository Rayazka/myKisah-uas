using Microsoft.Extensions.Configuration;
using Moq;
using myKisah.Automata;
using myKisah.Interfaces;
using myKisah.Models;
using myKisah.Services;

namespace myKisah.Tests.JournalTests;
// Unit test untuk JournalService — validasi CreateJournal + GetJournalsByUser + DeleteJournal.

public class JournalServiceTests
{

    // Test Scenario:
    // 1. CreateJournal
    //    - Valid input → berhasil buat journal dengan state Draft
    //    - Empty userId/title/content → throw ArgumentException
    //    - Invalid mood (enum out of range) → throw ArgumentException
    //    - Mood not in valid moods config → throw ArgumentException
    //    - Content length exceeds max → throw ArgumentException
    // 2. GetJournalsByUser
    //    - Valid userId → return list of journals for that user
    //    - Empty userId → throw ArgumentException
    //    - User with no journals → return empty list
    // 3. DeleteJournal
    //    - Existing journalId → delete and return true
    //    - Non-existing journalId → throw KeyNotFoundException 
    
    private readonly Mock<IJournalRepository> _mockRepo; 
    private readonly JournalStateMachine _stateMachine;
    private readonly JournalService _service;

    public JournalServiceTests()
    {
        // Setup default config dan dependencies
        _mockRepo = new Mock<IJournalRepository>();
        _stateMachine = new JournalStateMachine();
        // Service dengan config default (maxContentLength=5000, validMoods=Happy,Sad,Angry,Anxious,Calm)
        _service = new JournalService(_mockRepo.Object, _stateMachine, CreateConfig());
    }

    // Untuk membuat konfigurasi dinamis sesuai kebutuhan test
    private static IConfiguration CreateConfig(int maxContentLength = 5000, string[]? validMoods = null)
    {
        // Default valid moods
        validMoods ??= new[] { "Happy", "Sad", "Angry", "Anxious", "Calm" };

        var configData = new Dictionary<string, string?>
        {
            { "JournalConfig:MaxContentLength", maxContentLength.ToString() }
        };

        // Tambahkan valid moods ke config
        for (int i = 0; i < validMoods.Length; i++)
            configData[$"JournalConfig:ValidMoods:{i}"] = validMoods[i];

        // Mengembalikan konfigurasi yang dibangun dari dictionary
        return new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();
    }

    // Helper untuk membuat service dengan konfigurasi khusus
    private JournalService CreateServiceWithConfig(int maxContentLength, string[]? validMoods = null)
    {
        return new JournalService(_mockRepo.Object, _stateMachine, CreateConfig(maxContentLength, validMoods));
    }

    //** CREATE JOURNAL — VALID CASES
    [Fact]
    public void CreateJournal_ValidInput_ReturnsJournalWithDraftState()
    {
        // Menguji pembuatan journal dengan input yang valid, memastikan state awal adalah Draft dan repository dipanggil untuk menyimpan journal baru. 
        // Juga memastikan mood yang dipilih valid dan sesuai dengan konfigurasi.
        var result = _service.CreateJournal("user-001", "Judul Test", "Konten test yang valid", MoodType.Happy);

        // Pastikan journal tidak null
        Assert.NotNull(result); 
        // UserId pada journal yang dibuat sesuai dengan input yang diberikan
        Assert.Equal("user-001", result.UserId);
        // Title pada journal yang dibuat sesuai dengan input yang diberikan 
        Assert.Equal("Judul Test", result.Title);
        // Content pada journal yang dibuat sesuai dengan input yang diberikan
        Assert.Equal(MoodType.Happy, result.Mood);
        // State awal journal yang dibuat adalah Saved (karena content tidak kosong)
        Assert.Equal(JournalState.Saved, result.State);

        // Verifikasi bahwa repository dipanggil untuk menambahkan journal baru.
        _mockRepo.Verify(r => r.Add(It.IsAny<Journal>()), Times.Once); 
    }

    //** CREATE JOURNAL — VALID MOODS
    [Fact] 
    public void CreateJournal_AllValidMoods_DoesNotThrow()
    {
        // Menguji bahwa semua mood yang valid dalam enum dapat digunakan untuk membuat journal tanpa menyebabkan exception, 
        foreach (MoodType mood in Enum.GetValues<MoodType>())
        {
            // Untuk setiap mood yang valid, pastikan tidak terjadi exception dan journal dibuat dengan mood yang benar serta state Draft.
            var result = _service.CreateJournal("user-001", "T", "C", mood);
            Assert.Equal(mood, result.Mood);
            Assert.Equal(JournalState.Saved, result.State);
        }
    }

    //** CREATE JOURNAL — EMPTY INPUT
    [Fact]
    public void CreateJournal_EmptyUserId_ThrowsArgumentException()
    {
        // Menguji bahwa jika userId kosong, maka CreateJournal akan melempar ArgumentException dengan pesan yang menyebutkan "UserId".
        var ex = Assert.Throws<ArgumentException>(() =>
            _service.CreateJournal("", "Judul", "Konten", MoodType.Happy));
        Assert.Contains("UserId", ex.Message);
    }

    //** CREATE JOURNAL — TITLE/CONTENT VALIDATION (Allows empty for auto-draft)
    [Fact]
    public void CreateJournal_EmptyTitle_ReturnsDraftJournal()
    {
        var result = _service.CreateJournal("user-001", "", "Konten", MoodType.Happy);
        Assert.NotNull(result);
        Assert.Equal(JournalState.Saved, result.State);
    }
    [Fact]
    public void CreateJournal_EmptyContent_ReturnsDraftJournal()
    {
        var result = _service.CreateJournal("user-001", "Judul", "", MoodType.Happy);
        Assert.NotNull(result);
        Assert.Equal(JournalState.Draft, result.State);
    }
    [Fact]
    public void CreateJournal_WhitespaceTitle_ReturnsDraftJournal()
    {
        var result = _service.CreateJournal("user-001", "   ", "Konten", MoodType.Happy);
        Assert.NotNull(result);
        Assert.Equal(JournalState.Saved, result.State);
    }

    //** CREATE JOURNAL — MOOD VALIDATION
    [Fact]
    public void CreateJournal_InvalidEnumMood_ThrowsArgumentException()
    {
        // Menguji bahwa jika mood yang diberikan tidak sesuai dengan nilai enum yang valid, maka CreateJournal akan melempar ArgumentException.
        Assert.Throws<ArgumentException>(() =>
            _service.CreateJournal("user-001", "Judul", "Konten", (MoodType)999));
    }

    [Fact]
    public void CreateJournal_MoodNotInValidMoodsConfig_ThrowsArgumentException()
    {
        // Config tanpa Calm
        var restrictedService = CreateServiceWithConfig(2000, new[] { "Happy", "Sad" });

        var ex = Assert.Throws<ArgumentException>(() =>
            restrictedService.CreateJournal("user-001", "Judul", "Konten", MoodType.Calm));

        Assert.Contains("tidak valid", ex.Message);
    }

    //** CREATE JOURNAL — CONTENT LENGTH (RUNTIME CONFIG)
    [Fact]
    public void CreateJournal_ContentExceedsMaxLength_ThrowsArgumentException()
    {
        var restrictedService = CreateServiceWithConfig(10);
        var longContent = new string('A', 100);

        var ex = Assert.Throws<ArgumentException>(() =>
            restrictedService.CreateJournal("user-001", "J", longContent, MoodType.Happy));

        Assert.Contains("10", ex.Message);
    }

    //** GET JOURNALS BY USER
    // 1. GetJournalsByUser dengan userId yang valid → mengembalikan daftar journals untuk user tersebut.
    // 2. GetJournalsByUser dengan userId kosong → melempar

    [Fact]
    public void GetJournalsByUser_ReturnsFilteredJournals()
    {
        var expected = new List<Journal>
        {
            new() { Id = "j1", UserId = "user-001", Title = "J1", Content = "C1", Mood = MoodType.Happy },
            new() { Id = "j2", UserId = "user-001", Title = "J2", Content = "C2", Mood = MoodType.Sad }
        };

        _mockRepo.Setup(r => r.GetByUserId("user-001")).Returns(expected);

        var result = _service.GetJournalsByUser("user-001");

        Assert.Equal(2, result.Count());
        _mockRepo.Verify(r => r.GetByUserId("user-001"), Times.Once);
    }

    [Fact]
    public void GetJournalsByUser_EmptyUserId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _service.GetJournalsByUser(""));
    }

    //** DELETE JOURNAL
        // 1. DeleteJournal dengan journalId yang ada → berhasil menghapus dan mengembalikan true.
        // 2. DeleteJournal dengan journalId yang tidak ada → melempar KeyNotFoundException dengan pesan yang menyebutkan journalId yang tidak ditemukan.
    [Fact]
    public void DeleteJournal_ExistingId_ReturnsTrue()
    {
        _mockRepo.Setup(r => r.GetById("j1")).Returns(new Journal { Id = "j1" });

        var result = _service.DeleteJournal("j1");

        Assert.True(result);
        _mockRepo.Verify(r => r.Delete("j1"), Times.Once);
    }

    [Fact]
    public void DeleteJournal_NonExistingId_ThrowsKeyNotFoundException()
    {
        _mockRepo.Setup(r => r.GetById("nonexist")).Returns((Journal?)null);

        var ex = Assert.Throws<KeyNotFoundException>(() =>
            _service.DeleteJournal("nonexist"));

        Assert.Contains("nonexist", ex.Message);
    }
}
