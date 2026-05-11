// ═══════════════════════════════════════════════════════════
// TEST PLAN: CharacterService (Table-Driven)
// DOMAIN: Character Companion System
// PENANGGUNG JAWAB: Toni Kurniawan
// ═══════════════════════════════════════════════════════════
//
// 📘 PETUNJUK: Setup xUnit project + Moq, copy test dari file ini.
//
// ⚠️ TEST INI MEMBUKTIKAN TABLE-DRIVEN:
// GenerateResponse TIDAK mengakses if/switch — hanya lookup via repo mock.
// Pastikan mock ICharacterResponseRepository mengembalikan data yang benar.
//
// ═══════════════════════════════════════════════════════════
// 📋 TEST CASE LIST (6 test):
// ═══════════════════════════════════════════════════════════
//
// [ ] GenerateResponse_HappyMood_ReturnsResponse
//     Setup: mock GetByMood("char1", Happy) → list { Response="Yeay!" }
//     Action: service.GenerateResponse("char1", MoodType.Happy)
//     Assert: result == "Yeay!"
//
// [ ] GenerateResponse_SadMood_ReturnsResponse
//     Setup: mock GetByMood("char1", Sad) → list { Response="Hiks..." }
//     Action: service.GenerateResponse("char1", MoodType.Sad)
//     Assert: result == "Hiks..."
//
// [ ] GenerateResponse_AllMoods_Covered
//     Setup: mock untuk SEMUA MoodType
//     Action: foreach (MoodType m in Enum.GetValues<MoodType>())
//               service.GenerateResponse("char1", m)
//     Assert: semua return string tidak null
//
// [ ] GenerateResponse_InvalidMood_ThrowsException
//     Action: service.GenerateResponse("char1", (MoodType)999)
//     Assert: throws ArgumentException
//
// [ ] GenerateResponse_NullCharacterId_ThrowsException
//     Action: service.GenerateResponse("", MoodType.Happy)
//     Assert: throws ArgumentException
//
// [ ] GetAllCharacters_ReturnsAllCharacters
//     Setup: mock GetAll() → list 3 karakter
//     Action: service.GetAllCharacters()
//     Assert: Count == 3
//
// ═══════════════════════════════════════════════════════════
// CONTOH IMPLEMENTASI:
// ═══════════════════════════════════════════════════════════
//
// using Xunit; using Moq;
// using myKisah.Models; using myKisah.Services; using myKisah.Interfaces;
//
// public class CharacterServiceTests
// {
//     private readonly Mock<ICharacterRepository> _mockCharRepo;
//     private readonly Mock<ICharacterResponseRepository> _mockRespRepo;
//     private readonly CharacterService _service;
//
//     public CharacterServiceTests()
//     {
//         _mockCharRepo = new Mock<ICharacterRepository>();
//         _mockRespRepo = new Mock<ICharacterResponseRepository>();
//         _service = new CharacterService(_mockCharRepo.Object, _mockRespRepo.Object);
//     }
//
//     [Fact]
//     public void GenerateResponse_HappyMood_ReturnsResponse()
//     {
//         _mockRespRepo.Setup(r => r.GetByMood("char1", MoodType.Happy))
//             .Returns(new List<CharacterResponse> {
//                 new() { Response = "Yeay! Senang!" }
//             });
//         var result = _service.GenerateResponse("char1", MoodType.Happy);
//         Assert.Equal("Yeay! Senang!", result);
//     }
// }
