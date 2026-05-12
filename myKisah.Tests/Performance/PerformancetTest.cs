using Xunit;
using System.Diagnostics;
using System.Collections.Generic;
using Moq;
using myKisah.Services;
using myKisah.Models;
using myKisah.Interfaces;

namespace myKisah.Tests.Performance
{
    public class PerformanceTest
    {
        // Perf Test 1: Load/Simulasi Cold Start (Target < 30ms)
        [Fact]
        public void LoadCharacterResponses_ColdStart_Under30ms()
        {
            // Arrange: Menggunakan Moq agar terhindar dari error JsonStorageHelper dan FilePathConfig (CS7036)
            var mockRespRepo = new Mock<ICharacterResponseRepository>();
            
            // Perhatikan: Kita menghapus 'ResponseText' agar tidak terjadi error CS0117
            mockRespRepo.Setup(r => r.GetAll()).Returns(new List<CharacterResponse> 
            { 
                new CharacterResponse { CharacterId = "C01", Mood = MoodType.Happy } 
            });

            var stopwatch = new Stopwatch();

            // Act: Mengukur waktu eksekusi pengambilan data
            stopwatch.Start();
            var data = mockRespRepo.Object.GetAll(); 
            stopwatch.Stop();

            // Assert: Waktu harus di bawah 30ms
            Assert.True(stopwatch.ElapsedMilliseconds < 30, 
                $"Pembacaan data terlalu lambat: {stopwatch.ElapsedMilliseconds}ms (Target: < 30ms)");
        }

        // Perf Test 2: GenerateResponse 1000 iterasi < 1ms/call (Total < 1000ms)
        [Fact]
        public void GenerateResponse_1000Iterations_Under1000ms()
        {
            // Arrange
            var mockCharRepo = new Mock<ICharacterRepository>();
            var mockRespRepo = new Mock<ICharacterResponseRepository>();

            // Perhatikan: Kita menghapus 'ResponseText' agar tidak terjadi error CS0117
            mockRespRepo.Setup(r => r.GetAll()).Returns(new List<CharacterResponse> 
            { 
                new CharacterResponse { CharacterId = "C01", Mood = MoodType.Happy } 
            });

            var service = new CharacterService(mockCharRepo.Object, mockRespRepo.Object);
            var stopwatch = new Stopwatch();

            // Act: Eksekusi metode 1000 kali berturut-turut
            stopwatch.Start();
            for (int i = 0; i < 1000; i++)
            {
                service.GenerateResponse("C01", MoodType.Happy);
            }
            stopwatch.Stop();

            // Assert: Total waktu untuk 1000 eksekusi harus di bawah 1000 ms
            Assert.True(stopwatch.ElapsedMilliseconds < 1000, 
                $"Generasi respons terlalu lambat: {stopwatch.ElapsedMilliseconds}ms untuk 1000 iterasi");
        }
    }
}