using myKisah.Interfaces;
using myKisah.Models;
using myKisah.Utils;

namespace myKisah.Services;

// ═══════════════════════════════════════════════════════════
// KELAS: CharacterService
// DOMAIN: Character Companion System
// TEKNIK: Table-Driven (INI ADALAH INTI TABLE-DRIVEN!)
// PENANGGUNG JAWAB: Toni Kurniawan
// ═══════════════════════════════════════════════════════════
//
// 📘 APA INI?
// Business logic untuk Character Companion.
// Extends ServiceBase. Implements ICharacterService.
//
// Method GenerateResponse() adalah method INTI yang mendemonstrasikan
// teknik TABLE-DRIVEN. Method ini TIDAK BOLEH mengandung if/switch —
// seluruh lookup didelegasikan ke repository → JSON file.
//
// 🧠 ALUR TABLE-DRIVEN:
// Controller → CharacterService.GenerateResponse(charId, mood)
//           → CharacterResponseRepository.GetByMood(charId, mood)  [LINQ Where]
//           → characterResponses.json                              [DATA]
//           → return response.Response (string text)
//
// Di seluruh flow ini, TIDAK ADA if/switch/else berdasarkan mood.
// Semua mapping mood→response ada di characterResponses.json.
//
// 📋 TODO:
// [ ] 1. Constructor: terima ICharacterRepository + ICharacterResponseRepository via DI
//
// [ ] 2. Override ServiceName → "CharacterService"
//
// [ ] 3. Implement GetAllCharacters():
//        → return _characterRepo.GetAll()
//
// [ ] 4. IMPLEMENT GenerateResponse — METHOD INTI TABLE-DRIVEN:
//        a. Validator.ValidateNotEmpty(characterId, "CharacterId")
//        b. Validator.ValidateInEnum(mood, "Mood")
//
//        c. var responses = _responseRepo.GetByMood(characterId, mood)
//        d. var response = responses.FirstOrDefault()
//        e. Validator.ValidateExists(response, $"Response untuk character '{characterId}' dengan mood '{mood}'")
//
//        f. return response.Response  (string text-nya)
//
// ⚠️ PERINGATAN UNTUK TONI:
// JANGAN PERNAH menulis:
//   if (mood == MoodType.Happy) return "Selamat!";
//   else if (mood == MoodType.Sad) return "Semangat!";
// Ini MELANGGAR teknik table-driven dan akan mengurangi nilai kamu.
//
// Yang BENAR:
//   var response = _responseRepo.GetByMood(characterId, mood).FirstOrDefault();
//   return response.Response;
// Data response ada di characterResponses.json.
//
// Referensi: Task_myKisah.md baris 67-72, 201-208

public class CharacterService : ServiceBase, ICharacterService
{
    private readonly ICharacterRepository _characterRepo;
    private readonly ICharacterResponseRepository _responseRepo;

    public CharacterService(
        ICharacterRepository characterRepo,
        ICharacterResponseRepository responseRepo)
    {
        _characterRepo = characterRepo;
        _responseRepo = responseRepo;
    }

    protected override string ServiceName => throw new NotImplementedException("TODO: return 'CharacterService'");

    public IEnumerable<Character> GetAllCharacters()
    {
        throw new NotImplementedException("TODO: GetAllCharacters - return semua karakter");
    }

    /// <summary>
    /// GenerateResponse adalah INTI TABLE-DRIVEN.
    /// TIDAK BOLEH ADA IF/SWITCH — lookup ke JSON via repository.
    /// </summary>
    public string GenerateResponse(string characterId, MoodType mood)
    {
        throw new NotImplementedException("TODO: GenerateResponse - validasi + lookup table. NO IF/SWITCH!");
    }
}
