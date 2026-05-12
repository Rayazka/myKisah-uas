using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using myKisah.Services;
using myKisah.Models;
using myKisah.Interfaces;

namespace myKisah.Tests.Character
{
    public class CharacterTest
    {
        // PERBAIKAN 1: Menggunakan Interface Spesifik sesuai error CS1503
        private readonly Mock<ICharacterRepository> _mockCharRepo;
        private readonly Mock<ICharacterResponseRepository> _mockRespRepo;
        private readonly CharacterService _service;

        public CharacterTest()
        {
            // Setup Mock dengan Interface Spesifik
            _mockCharRepo = new Mock<ICharacterRepository>();
            _mockRespRepo = new Mock<ICharacterResponseRepository>();

            // Setup data palsu. (Pastikan properti 'ResponseText' sesuai dengan model CharacterResponse Anda)
            _mockRespRepo.Setup(repo => repo.GetAll()).Returns(new List<CharacterResponse>
            {
                new CharacterResponse { CharacterId = "C01", Mood = MoodType.Happy, Response = "Aku ikut senang!" },
                new CharacterResponse { CharacterId = "C01", Mood = MoodType.Sad, Response = "Aku di sini untukmu." }
            });

            _mockCharRepo.Setup(repo => repo.GetById("C01")).Returns(new Models.Character { Id = "C01", Name = "Gista" });

            // Inisialisasi service tidak akan error lagi (CS1503 teratasi)
            _service = new CharacterService(_mockCharRepo.Object, _mockRespRepo.Object);
        }

        // PERBAIKAN 2: Test AssignCharacter dihapus (CS1061 teratasi). Fokus pada GenerateResponse sesuai PRD.

        [Theory]
        [InlineData(MoodType.Happy, "Aku ikut senang!")]
        [InlineData(MoodType.Sad, "Aku di sini untukmu.")]
        public void GenerateResponse_ValidMood_ReturnsCorrectText(MoodType mood, string expectedText)
        {
            // Act
            var result = _service.GenerateResponse("C01", mood);
            
            // Assert
            Assert.Equal(expectedText, result);
        }

        [Fact]
        public void GenerateResponse_InvalidMood_ThrowsArgumentException()
        {
            // Arrange
            MoodType invalidMood = (MoodType)999;
            
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _service.GenerateResponse("C01", invalidMood));
        }

        [Fact]
        public void GenerateResponse_NullCharacterId_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _service.GenerateResponse(null, MoodType.Happy));
        }
    }
}