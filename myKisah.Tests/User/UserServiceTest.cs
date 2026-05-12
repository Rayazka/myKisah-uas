using Moq;
using myKisah.Interfaces;
using myKisah.Models;
using myKisah.Services;
using Xunit;

namespace myKisah.Tests.User;

public class UserServiceTest
{
    // Mock repository palsu
    private readonly Mock<IUserRepository> _mockRepository;

    // Service yang mau dites
    private readonly UserService _service;

    public UserServiceTest()
    {
        // Setup mock repository
        _mockRepository = new Mock<IUserRepository>();

        // Inject mock ke service
        _service = new UserService(_mockRepository.Object);
    }

    // ═══════════════════════════════════════════════════════
    // 1. REGISTER USER SUCCESS
    // ═══════════════════════════════════════════════════════

    [Fact]
    public void RegisterUser_ValidUsername_Success()
    {
        // Arrange
        string username = "Farrel";

        // Username belum ada
        _mockRepository
            .Setup(r => r.UsernameExists(username))
            .Returns(false);

        // Act
        var result = _service.RegisterUser(username);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(username, result.Username);

        // Pastikan Add() dipanggil 1x
        _mockRepository.Verify(r => r.Add(It.IsAny<User>()), Times.Once);
    }

    // ═══════════════════════════════════════════════════════
    // 2. DUPLICATE USERNAME
    // ═══════════════════════════════════════════════════════

    [Fact]
    public void RegisterUser_DuplicateUsername_ThrowsException()
    {
        // Arrange
        string username = "Farrel";

        // Simulasikan username sudah ada
        _mockRepository
            .Setup(r => r.UsernameExists(username))
            .Returns(true);

        // Act + Assert
        Assert.Throws<ArgumentException>(
            () => _service.RegisterUser(username)
        );
    }

    // ═══════════════════════════════════════════════════════
    // 3. EMPTY USERNAME
    // ═══════════════════════════════════════════════════════

    [Fact]
    public void RegisterUser_EmptyUsername_ThrowsException()
    {
        // Arrange
        string username = "";

        // Act + Assert
        Assert.Throws<ArgumentException>(
            () => _service.RegisterUser(username)
        );
    }

    // ═══════════════════════════════════════════════════════
    // 4. GET ALL USERS
    // ═══════════════════════════════════════════════════════

    [Fact]
    public void GetAllUsers_ReturnsAllRecords()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Username = "Farrel" },
            new User { Username = "Ray" },
            new User { Username = "Toni" }
        };

        _mockRepository
            .Setup(r => r.GetAll())
            .Returns(users);

        // Act
        var result = _service.GetAllUsers();

        // Assert
        Assert.Equal(3, result.Count());
    }

    // ═══════════════════════════════════════════════════════
    // 5. DELETE USER SUCCESS
    // ═══════════════════════════════════════════════════════

    [Fact]
    public void DeleteUser_ExistingId_Success()
    {
        // Arrange
        string id = "123";

        var user = new User
        {
            Id = id,
            Username = "Farrel"
        };

        _mockRepository
            .Setup(r => r.GetById(id))
            .Returns(user);

        // Act
        var result = _service.DeleteUser(id);

        // Assert
        Assert.True(result);

        _mockRepository.Verify(r => r.Delete(id), Times.Once);
    }

    // ═══════════════════════════════════════════════════════
    // 6. DELETE USER NOT FOUND
    // ═══════════════════════════════════════════════════════

    [Fact]
    public void DeleteUser_NonExistingId_ThrowsException()
    {
        // Arrange
        string id = "999";

        // Simulasikan user tidak ditemukan
        _mockRepository
            .Setup(r => r.GetById(id))
            .Returns((User?)null);

        // Act + Assert
        Assert.Throws<KeyNotFoundException>(
            () => _service.DeleteUser(id)
        );
    }

    // ═══════════════════════════════════════════════════════
    // 7. UPDATE USER SUCCESS
    // ═══════════════════════════════════════════════════════

    [Fact]
    public void UpdateUser_ValidInput_Success()
    {
        // Arrange
        string id = "123";
        string newUsername = "FarrelBaru";

        var user = new User
        {
            Id = id,
            Username = "Farrel"
        };

        _mockRepository
            .Setup(r => r.GetById(id))
            .Returns(user);

        _mockRepository
            .Setup(r => r.UsernameExists(newUsername))
            .Returns(false);

        // Act
        var result = _service.UpdateUser(id, newUsername);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newUsername, result!.Username);

        _mockRepository.Verify(r => r.Update(It.IsAny<User>()), Times.Once);
    }
}