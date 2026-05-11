using myKisah.Models;

namespace myKisah.Interfaces;


// Character Companion System
// Azka (Interface), Toni (Implementasi)
// TEKNIK: Table-Driven + API (digunakan oleh CharacterController)

// ** PENJELASAN:
// Kontrak business logic untuk Character Companion.
// Dipanggil oleh CharacterController (API layer).
//
// Method:
// - GetAllCharacters()                  : List semua karakter yang tersedia
// - GenerateResponse(characterId, mood): Ambil response karakter untuk mood tertentu
//
// ** TABLE-DRIVEN:
// GenerateResponse() HARUS melakukan lookup ke characterResponses.json
// via ICharacterResponseRepository.GetByMood().
// TIDAK BOLEH ada if/switch di method ini.
// - Flow: Controller → Service.GenerateResponse() → Repository.GetByMood() → JSON file
// - Pastikan tidak ada hardcoded response string di kode C#
// - Semua response ada di characterResponses.json (data)
//
// TODO untuk Toni:
// 1. Buat kelas CharacterService di folder Services/
// 2. Extend ServiceBase
// 3. Inject ICharacterRepository + ICharacterResponseRepository
// 4. GetAllCharacters: panggil _characterRepo.GetAll()
// 5. GenerateResponse: validasi characterId tidak null + mood valid,
//        lalu panggil _responseRepo.GetByMood(characterId, mood)
//        dan return response.Response (string text-nya)
//
public interface ICharacterService
{
    IEnumerable<Character> GetAllCharacters();
    string GenerateResponse(string characterId, MoodType mood);
}
