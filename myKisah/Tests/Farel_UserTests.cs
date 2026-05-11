// ═══════════════════════════════════════════════════════════
// TEST PLAN: UserService
// DOMAIN: User Management
// PENANGGUNG JAWAB: Farel Ilham
// ═══════════════════════════════════════════════════════════
//
// 📘 PETUNJUK: Setup xUnit project + Moq, copy test dari file ini.
//
// ═══════════════════════════════════════════════════════════
// 📋 TEST CASE LIST (7 test):
// ═══════════════════════════════════════════════════════════
//
// [ ] RegisterUser_ValidUsername_Success
//     Setup: mock UsernameExists("farel") → false
//     Action: service.RegisterUser("farel")
//     Assert: user.Username == "farel", user.Id tidak null/empty
//
// [ ] RegisterUser_DuplicateUsername_ThrowsException
//     Setup: mock UsernameExists("exist") → true
//     Action: service.RegisterUser("exist")
//     Assert: throws ArgumentException dengan message mengandung "sudah terdaftar"
//
// [ ] RegisterUser_EmptyUsername_ThrowsException
//     Action: service.RegisterUser("") atau service.RegisterUser("   ")
//     Assert: throws ArgumentException
//
// [ ] GetAllUsers_ReturnsAllRecords
//     Setup: mock GetAll() → list 3 user
//     Action: service.GetAllUsers()
//     Assert: hasil.Count() == 3
//
// [ ] DeleteUser_ExistingId_Success
//     Setup: mock GetById("id") → return user
//     Action: service.DeleteUser("id")
//     Assert: return true
//
// [ ] DeleteUser_NonExistingId_ThrowsException
//     Setup: mock GetById("nonexist") → return null
//     Action: service.DeleteUser("nonexist")
//     Assert: throws KeyNotFoundException
//
// [ ] UpdateUser_ValidInput_Success
//     Setup: mock GetById("id") → user, UsernameExists("new") → false
//     Action: service.UpdateUser("id", "new")
//     Assert: result.Username == "new"
//
// ═══════════════════════════════════════════════════════════
// CONTOH IMPLEMENTASI:
// ═══════════════════════════════════════════════════════════
//
// using Xunit; using Moq;
// using myKisah.Models; using myKisah.Services; using myKisah.Interfaces;
//
// public class UserServiceTests
// {
//     private readonly Mock<IUserRepository> _mockRepo;
//     private readonly UserService _service;
//
//     public UserServiceTests()
//     {
//         _mockRepo = new Mock<IUserRepository>();
//         _service = new UserService(_mockRepo.Object);
//     }
//
//     [Fact]
//     public void RegisterUser_ValidUsername_Success()
//     {
//         _mockRepo.Setup(r => r.UsernameExists("farel")).Returns(false);
//         var result = _service.RegisterUser("farel");
//         Assert.Equal("farel", result.Username);
//         Assert.False(string.IsNullOrEmpty(result.Id));
//     }
// }
