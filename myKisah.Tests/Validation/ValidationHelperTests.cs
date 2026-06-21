using System;
using Xunit;
using myKisah.Utils;
using myKisah.Models;

namespace myKisah.Tests.ValidationTests;

public class ValidationHelperTests
{
    private readonly ValidationHelper _validator = new ValidationHelper();

    // Test ValidateNotNull dengan input null → harus lempar ArgumentNullException
    [Fact]
    public void ValidateNotNull_NullValue_ThrowsException()
    {
        var ex = Assert.Throws<ArgumentNullException>(
            () => _validator.ValidateNotNull<string>(null, "param"));
        Assert.Contains("param", ex.Message); // pastikan pesan exception sesuai
    }

    // Test ValidateNotNull dengan input valid → tidak boleh lempar exception
    [Fact]
    public void ValidateNotNull_ValidValue_NoException()
    {
        _validator.ValidateNotNull("hello", "param");
    }

    // Test ValidateNotEmpty dengan string kosong atau whitespace → harus lempar ArgumentException
    [Fact]
    public void ValidateNotEmpty_EmptyString_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => _validator.ValidateNotEmpty("", "param"));
        Assert.Throws<ArgumentException>(() => _validator.ValidateNotEmpty("   ", "param")); // whitespace
    }

    // Test ValidateInEnum dengan nilai enum tidak valid → harus lempar ArgumentException
    [Fact]
    public void ValidateInEnum_InvalidValue_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => _validator.ValidateInEnum((MoodType)999, "Mood")); // 
    }

    // Test ValidateExists dengan entity null → harus lempar KeyNotFoundException
    [Fact]
    public void ValidateExists_NullEntity_ThrowsException()
    {
        var ex = Assert.Throws<KeyNotFoundException>(
            () => _validator.ValidateExists<object>(null, "Entity"));
        Assert.Contains("Entity", ex.Message); // pastikan pesan exception sesuai
    }
}
