// ═══════════════════════════════════════════════════════════
// TEST PLAN: JournalService + JournalStateMachine
// DOMAIN: Journal System
// PENANGGUNG JAWAB: Rayazka Aris
// ═══════════════════════════════════════════════════════════
//
// 📘 PETUNJUK SETUP TEST PROJECT:
// 1. Buat test project: dotnet new xunit -n myKisah.Tests
// 2. Tambah reference: dotnet add reference ../myKisah/myKisah.csproj
// 3. Install Moq: dotnet add package Moq
// 4. Copy test method dari file ini ke project test
//
// ═══════════════════════════════════════════════════════════
// 📋 TEST CASE LIST (10 test):
// ═══════════════════════════════════════════════════════════
//
// [ ] CreateJournal_ValidInput_Success
//     Setup: mock config → MaxContentLength=2000, ValidMoods
//     Action: service.CreateJournal("user1", "Judul", "Konten", MoodType.Happy)
//     Assert: journal tidak null, State == Draft, Id tidak kosong
//
// [ ] CreateJournal_EmptyTitle_ThrowsException
//     Action: service.CreateJournal("user1", "", "Konten", MoodType.Happy)
//     Assert: throws ArgumentException
//
// [ ] CreateJournal_EmptyContent_ThrowsException
//     Action: service.CreateJournal("user1", "Judul", "", MoodType.Happy)
//     Assert: throws ArgumentException
//
// [ ] CreateJournal_InvalidMood_ThrowsException
//     Action: service.CreateJournal("user1", "Judul", "Konten", (MoodType)999)
//     Assert: throws ArgumentException
//
// [ ] CreateJournal_ContentTooLong_ThrowsException
//     Setup: mock config → MaxContentLength=10
//     Action: service.CreateJournal("user1", "J", new string('x', 100), MoodType.Happy)
//     Assert: throws ArgumentException
//
// [ ] StateMachine_Draft_Submit_Submitted
//     Action: machine.Transition(JournalState.Draft, JournalTrigger.Submit)
//     Assert: result == JournalState.Submitted
//
// [ ] StateMachine_Submitted_Save_Saved
//     Action: Draft→Submit→Submitted, lalu Submitted+Save→Saved
//     Assert: result == JournalState.Saved
//
// [ ] StateMachine_Submitted_Reject_Rejected
//     Action: Submitted+Reject→Rejected
//     Assert: result == JournalState.Rejected
//
// [ ] StateMachine_Rejected_Reset_Draft
//     Action: Submitted→Reject→Rejected, lalu Rejected+Reset→Draft
//     Assert: result == JournalState.Draft
//
// [ ] StateMachine_InvalidTransition_ThrowsException
//     Action: machine.Transition(JournalState.Saved, JournalTrigger.Submit)
//     Assert: throws InvalidOperationException (Saved is terminal)
//
// [ ] IsTerminal_Saved_ReturnsTrue
//     Action: machine.IsTerminal(JournalState.Saved)
//     Assert: true
//
// ═══════════════════════════════════════════════════════════
// CONTOH IMPLEMENTASI:
// ═══════════════════════════════════════════════════════════
//
// using Xunit;
// using Moq;
// using myKisah.Models;
// using myKisah.Services;
// using myKisah.Interfaces;
// using myKisah.Automata;
// using Microsoft.Extensions.Configuration;
//
// public class JournalServiceTests
// {
//     private readonly Mock<IJournalRepository> _mockRepo;
//     private readonly Mock<IConfiguration> _mockConfig;
//     private readonly Mock<IConfigurationSection> _mockSection;
//     private readonly JournalStateMachine _stateMachine;
//     private readonly JournalService _service;
//
//     public JournalServiceTests()
//     {
//         _mockRepo = new Mock<IJournalRepository>();
//         _mockConfig = new Mock<IConfiguration>();
//         _mockSection = new Mock<IConfigurationSection>();
//         _stateMachine = new JournalStateMachine();
//         _service = new JournalService(_mockRepo.Object, _stateMachine, _mockConfig.Object);
//     }
//
//     [Fact]
//     public void CreateJournal_ValidInput_Success()
//     {
//         _mockConfig.Setup(c => c.GetSection("JournalConfig:ValidMoods"))
//             .Returns(_mockSection.Object);
//         _mockSection.Setup(s => s.Get<string[]>())
//             .Returns(new[] { "Happy", "Sad", "Angry", "Anxious", "Calm" });
//         _mockConfig.Setup(c => c.GetValue<int>("JournalConfig:MaxContentLength"))
//             .Returns(2000);
//
//         var result = _service.CreateJournal("user1", "Judul", "Konten", MoodType.Happy);
//
//         Assert.NotNull(result);
//         Assert.Equal(JournalState.Draft, result.State);
//         Assert.Equal("user1", result.UserId);
//     }
// }
