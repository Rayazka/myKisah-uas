using myKisah.Models;

namespace myKisah.Interfaces;

// ═══════════════════════════════════════════════════════════
// INTERFACE: ICharacterResponseRepository
// DOMAIN: Character Companion System
// TEKNIK: Generics + Table-Driven
// PENANGGUNG JAWAB: Azka (Interface), Toni (Implementasi)
// ═══════════════════════════════════════════════════════════
//
// ** PENJELASAN:
// Interface untuk akses data CharacterResponse (TABEL table-driven).
// Extends IRepository<CharacterResponse>.
// Method GetByMood adalah INTI dari teknik table-driven.
//
// ** TABLE-DRIVEN:
// GetByMood(characterId, mood) akan melakukan filter ke characterResponses.json
// dan return response yang cocok dan tidak boleh menggunakan if/switch statement sama sekali.
// - Tidak boleh ada if (mood == Happy) return "..." seperti itu
// - Method GetByMood HARUS pure table lookup (LINQ Where dari JSON)
// - Semua data response ada di characterResponses.json
//
// TODO untuk :
// 1. Buat kelas JsonCharacterResponseRepository
// 2. Implement GetByMood: filter data dari JSON berdasarkan characterId dan mood
//        PASTIKAN pakai LINQ Where, BUKAN if/switch!
// 3. Method ini akan dipanggil CharacterService.GenerateResponse()


public interface ICharacterResponseRepository : IRepository<CharacterResponse>
{
    IEnumerable<CharacterResponse> GetByMood(string characterId, MoodType mood);
}
