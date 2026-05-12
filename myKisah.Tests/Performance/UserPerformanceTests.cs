using System.Diagnostics;
using myKisah.Interfaces;
using myKisah.Models;
using myKisah.Services;
using Xunit;

namespace myKisah.Tests.Performance;

public class UserPerformanceTests
{
    // Fake repository sederhana untuk performance testing
    private class FakeUserRepository : IUserRepository
    {
        private readonly List<User> _users = new();

        public IEnumerable<User> GetAll()
        {
            return _users;
        }

        public User? GetById(string id)
        {
            return _users.FirstOrDefault(u => u.Id == id);
        }

        public void Add(User entity)
        {
            entity.Id = Guid.NewGuid().ToString();
            _users.Add(entity);
        }

        public void Update(User entity)
        {
            var index = _users.FindIndex(u => u.Id == entity.Id);

            if (index >= 0)
                _users[index] = entity;
        }

        public void Delete(string id)
        {
            _users.RemoveAll(u => u.Id == id);
        }

        public User? GetByUsername(string username)
        {
            return _users.FirstOrDefault(
                u => u.Username == username
            );
        }

        public bool UsernameExists(string username)
        {
            return _users.Any(
                u => u.Username == username
            );
        }
    }

    // ═══════════════════════════════════════════════════════
    // GetAllUsers() - 10 records
    // Target: < 10ms
    // ═══════════════════════════════════════════════════════

    [Fact]
    public void GetAllUsers_10Records_Performance()
    {
        // Arrange
        var repo = new FakeUserRepository();

        for (int i = 0; i < 10; i++)
        {
            repo.Add(new User
            {
                Username = $"User{i}"
            });
        }

        var service = new UserService(repo);

        var stopwatch = Stopwatch.StartNew();

        // Act
        var users = service.GetAllUsers().ToList();

        stopwatch.Stop();

        // Assert
        Assert.True(stopwatch.ElapsedMilliseconds < 10);

        Console.WriteLine(
            $"GetAllUsers 10 records: {stopwatch.ElapsedMilliseconds}ms"
        );
    }

    // ═══════════════════════════════════════════════════════
    // GetAllUsers() - 100 records
    // Target: < 30ms
    // ═══════════════════════════════════════════════════════

    [Fact]
    public void GetAllUsers_100Records_Performance()
    {
        var repo = new FakeUserRepository();

        for (int i = 0; i < 100; i++)
        {
            repo.Add(new User
            {
                Username = $"User{i}"
            });
        }

        var service = new UserService(repo);

        var stopwatch = Stopwatch.StartNew();

        var users = service.GetAllUsers().ToList();

        stopwatch.Stop();

        Assert.True(stopwatch.ElapsedMilliseconds < 30);

        Console.WriteLine(
            $"GetAllUsers 100 records: {stopwatch.ElapsedMilliseconds}ms"
        );
    }

    // ═══════════════════════════════════════════════════════
    // GetAllUsers() - 500 records
    // Target: < 100ms
    // ═══════════════════════════════════════════════════════

    [Fact]
    public void GetAllUsers_500Records_Performance()
    {
        var repo = new FakeUserRepository();

        for (int i = 0; i < 500; i++)
        {
            repo.Add(new User
            {
                Username = $"User{i}"
            });
        }

        var service = new UserService(repo);

        var stopwatch = Stopwatch.StartNew();

        var users = service.GetAllUsers().ToList();

        stopwatch.Stop();

        Assert.True(stopwatch.ElapsedMilliseconds < 100);

        Console.WriteLine(
            $"GetAllUsers 500 records: {stopwatch.ElapsedMilliseconds}ms"
        );
    }

    // ═══════════════════════════════════════════════════════
    // RegisterUser()
    // Target: < 20ms
    // ═══════════════════════════════════════════════════════

    [Fact]
    public void RegisterUser_Performance()
    {
        var repo = new FakeUserRepository();

        var service = new UserService(repo);

        var stopwatch = Stopwatch.StartNew();

        // Act
        service.RegisterUser("Farrel");

        stopwatch.Stop();

        Assert.True(stopwatch.ElapsedMilliseconds < 20);

        Console.WriteLine(
            $"RegisterUser: {stopwatch.ElapsedMilliseconds}ms"
        );
    }
}