using System;
using System.Collections.Generic;
using System.Linq;
using myKisah.Models;
using myKisah.Interfaces;

namespace myKisah.Services
{
    public class CharacterService : ICharacterService
    {
        // PERUBAHAN: Gunakan interface spesifik, bukan generic IRepository<T>
        private readonly ICharacterRepository _characterRepository;
        private readonly ICharacterResponseRepository _responseRepository;

        // Constructor
        public CharacterService(
            ICharacterRepository characterRepository, 
            ICharacterResponseRepository responseRepository)
        {
            _characterRepository = characterRepository;
            _responseRepository = responseRepository;
        }

        public IEnumerable<Character> GetAllCharacters()
        {
            return _characterRepository.GetAll();
        }

        public Character AssignCharacter(string characterId)
        {
            if (string.IsNullOrWhiteSpace(characterId))
                throw new ArgumentNullException(nameof(characterId), "Character ID tidak boleh null atau kosong.");

            var character = _characterRepository.GetById(characterId);
            if (character == null)
                throw new KeyNotFoundException($"Karakter dengan ID {characterId} tidak ditemukan.");

            return character;
        }

        public string GenerateResponse(string characterId, MoodType mood)
        {
            if (string.IsNullOrWhiteSpace(characterId))
                throw new ArgumentNullException(nameof(characterId), "Character ID tidak boleh null.");

            if (!Enum.IsDefined(typeof(MoodType), mood))
                throw new ArgumentException("Input mood tidak valid atau tidak dikenali.", nameof(mood));

            // PERUBAHAN: Karena _responseRepository (JsonCharacterResponseRepository) sudah 
            // punya method GetByMood() khusus, kita bisa langsung menggunakannya! 
            // Ini jauh lebih efisien daripada memanggil GetAll() lalu di-filter manual.
            var mappedResponse = _responseRepository.GetByMood(characterId, mood).FirstOrDefault();

            if (mappedResponse == null)
            {
                return "Maaf, aku tidak tahu harus merespons apa untuk perasaan ini.";
            }

            return mappedResponse.Response; // Pastikan 'Response' atau 'ResponseText' sesuai model Anda
        }
    }
}