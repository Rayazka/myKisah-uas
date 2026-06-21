using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Moq;
using myKisah.Automata;
using myKisah.Interfaces;
using myKisah.Models;
using myKisah.Services;

namespace myKisah.Tests.Performance;

// Performance benchmark untuk Journal system.
// Menggunakan Stopwatch untuk mengukur waktu eksekusi.
public class JournalPerformanceTests
{
    private readonly Mock<IJournalRepository> _mockRepo;
    private readonly JournalStateMachine _stateMachine;
    private readonly JournalService _service;

    public JournalPerformanceTests()
    {
        _mockRepo = new Mock<IJournalRepository>();
        _stateMachine = new JournalStateMachine();

        var configData = new Dictionary<string, string?>
        {
            { "JournalConfig:MaxContentLength", "50000" },
            { "JournalConfig:ValidMoods:0", "Happy" },
            { "JournalConfig:ValidMoods:1", "Sad" },
            { "JournalConfig:ValidMoods:2", "Angry" },
            { "JournalConfig:ValidMoods:3", "Anxious" },
            { "JournalConfig:ValidMoods:4", "Calm" }
        };

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        _service = new JournalService(_mockRepo.Object, _stateMachine, config);
    }

// Helper method untuk generate list journal dengan jumlah tertentu
    private static List<Journal> GenerateJournals(int count)
    {
        var list = new List<Journal>();
        for (int i = 0; i < count; i++)
        {
            list.Add(new Journal
            {
                Id = Guid.NewGuid().ToString(),
                UserId = "user-001",
                Title = $"Journal {i}",
                Content = $"Content {i}",
                Mood = MoodType.Happy,
                State = JournalState.Draft
            });
        }
        return list;
    }

    
    // GET JOURNALS BY USER -- Mengukur performa mendapatkan journals dengan jumlah yang lebih kecil (100 entries) dan memastikan waktu eksekusi di bawah 50ms.
    [Fact]
    public void GetJournalsByUser_100Entries_Under50ms()
    {
        _mockRepo.Setup(r => r.GetByUserId("user-001")).Returns(GenerateJournals(100));

        // Warmup
        _service.GetJournalsByUser("user-001");

        var sw = Stopwatch.StartNew();
        var result = _service.GetJournalsByUser("user-001").ToList();
        sw.Stop();
        Console.WriteLine($"GetJournalsByUser 100 entries: {sw.ElapsedMilliseconds}ms");

        Assert.Equal(100, result.Count);
        Assert.True(sw.ElapsedMilliseconds < 50,
            $"GetJournalsByUser 100 entries too slow: {sw.ElapsedMilliseconds}ms (target < 50ms)");
    }

    // Mengukur performa mendapatkan journals dengan jumlah yang lebih besar (1000 entries) dan memastikan waktu eksekusi di bawah 200ms.
    [Fact]
    public void GetJournalsByUser_1000Entries_Under200ms()
    {
        _mockRepo.Setup(r => r.GetByUserId("user-001")).Returns(GenerateJournals(1000));

        // Warmup
        _service.GetJournalsByUser("user-001");

        var sw = Stopwatch.StartNew();
        var result = _service.GetJournalsByUser("user-001").ToList();
        sw.Stop();
        Console.WriteLine($"GetJournalsByUser 1000 entries: {sw.ElapsedMilliseconds}ms");

        Assert.Equal(1000, result.Count);
        Assert.True(sw.ElapsedMilliseconds < 200,
            $"GetJournalsByUser 1000 entries too slow: {sw.ElapsedMilliseconds}ms (target < 200ms)");
    }

    // Mengukur pembuatan journal baru dan memastikan waktu eksekusi di bawah 20ms.
    [Fact]
    public void CreateJournal_Under20ms()
    {
        // untuk memastikan bahwa konfigurasi valid
        _service.CreateJournal("user-001", "T", "C", MoodType.Happy);

        var sw = Stopwatch.StartNew();
        var result = _service.CreateJournal("user-001", "Performance", "Test", MoodType.Calm);
        sw.Stop();
        Console.WriteLine($"CreateJournal: {sw.ElapsedMilliseconds}ms");

        Assert.NotNull(result);
        Assert.True(sw.ElapsedMilliseconds < 20,
            $"CreateJournal too slow: {sw.ElapsedMilliseconds}ms (target < 20ms)");
    }

    // mengukur performa state machine dengan melakukan transisi berulang-ulang sebanyak 50.000 kali dan memastikan total waktu eksekusi di bawah 50ms.
    [Fact]
    public void StateMachine_20000Transitions_Under50ms()
    {
        // untuk memastikan state machine sudah berada di state yang benar sebelum melakukan transisi berulang
        _stateMachine.Transition(JournalState.Draft, JournalTrigger.Submit);

        var sw = Stopwatch.StartNew();
        for (int i = 0; i < 20000; i++)
        {
            _stateMachine.Transition(JournalState.Draft, JournalTrigger.Submit);
            _stateMachine.Transition(JournalState.Submitted, JournalTrigger.Reject);
            _stateMachine.Transition(JournalState.Rejected, JournalTrigger.Reset);
        }
        sw.Stop();
        Console.WriteLine($"StateMachine 60k transitions: {sw.ElapsedMilliseconds}ms");

        Assert.True(sw.ElapsedMilliseconds < 50,
            $"StateMachine 60k transitions too slow: {sw.ElapsedMilliseconds}ms (target < 50ms)");
    }
}
