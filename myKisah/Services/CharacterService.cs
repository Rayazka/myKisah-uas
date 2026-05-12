using System;
using System.Collections.Generic;
using System.Linq;
using myKisah.Models;      // Asumsi namespace untuk model Character, CharacterResponse, MoodType
using myKisah.Interfaces;  // Asumsi namespace untuk IRepository<T> dan ICharacterService

namespace myKisah.Services
{
    public class CharacterService : ICharacterService
    {
        // Menggunakan generic repository dari Farel/Rayazka untuk membaca JSON
        private readonly IRepository<Character> _characterRepository;
        private readonly IRepository<CharacterResponse> _responseRepository;

        // Dependency Injection untuk memasukkan repository
        public CharacterService(
            IRepository<Character> characterRepository, 
            IRepository<CharacterResponse> responseRepository)
        {
            _characterRepository = characterRepository;
            _responseRepository = responseRepository;
        }

        /// <summary>
        /// Mengambil semua data karakter yang tersedia
        /// </summary>
        public IEnumerable<Character> GetAllCharacters()
        {
            return _characterRepository.GetAll();
        }

        /// <summary>
        /// Menetapkan karakter pilihan pengguna
        /// </summary>
        public Character AssignCharacter(string characterId)
        {
            // Pre-condition check
            if (string.IsNullOrWhiteSpace(characterId))
            {
                throw new ArgumentNullException(nameof(characterId), "Character ID tidak boleh null atau kosong.");
            }

            var character = _characterRepository.GetById(characterId);
            if (character == null)
            {
                throw new KeyNotFoundException($"Karakter dengan ID {characterId} tidak ditemukan.");
            }

            return character;
        }

        /// Menghasilkan respons menggunakan teknik Table-Driven mapping dari file JSON
        public string GenerateResponse(string characterId, MoodType mood)
        {
            // --- Design by Contract (DbC) ---
            
            // 1. Validasi characterId tidak boleh null
            if (string.IsNullOrWhiteSpace(characterId))
            {
                throw new ArgumentNullException(nameof(characterId), "Character ID tidak boleh null.");
            }

            // 2. Validasi mood harus ada dalam daftar valid 
            if (!Enum.IsDefined(typeof(MoodType), mood))
            {
                throw new ArgumentException("Input mood tidak valid atau tidak dikenali.", nameof(mood));
            }

            // --- Implementasi Table-Driven ---
            
            // Mengambil semua data pemetaan respons dari repository (characterResponses.json)
            var allResponses = _responseRepository.GetAll();

            // Melakukan lookup (mapping) berdasarkan characterId dan mood
            var mappedResponse = allResponses.FirstOrDefault(r => 
                r.CharacterId == characterId && 
                r.Mood == mood
            );

            // Jika respons untuk kombinasi karakter dan mood tersebut tidak ditemukan di "tabel" JSON
            if (mappedResponse == null)
            {
                return "Maaf, aku tidak tahu harus merespons apa untuk perasaan ini.";
            }

            return mappedResponse.Response;
        }
    }
}