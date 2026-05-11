// ═══════════════════════════════════════════════════════════
// TEST PLAN: ValidationHelper + ErrorHandlingMiddleware
// DOMAIN: Shared Utilities & Global Error Handling
// PENANGGUNG JAWAB: Josefhint (Jojo)
// ═══════════════════════════════════════════════════════════
//
// 📘 PETUNJUK:
// - ValidationHelper: unit test biasa (no mock needed)
// - Middleware: test dengan DefaultHttpContext()
//
// ═══════════════════════════════════════════════════════════
// 📋 TEST CASE LIST — ValidationHelper (5 test):
// ═══════════════════════════════════════════════════════════
//
// [ ] ValidateNotNull_NullValue_ThrowsException
//     Action: validator.ValidateNotNull<string>(null, "param")
//     Assert: throws ArgumentNullException
//
// [ ] ValidateNotNull_ValidValue_NoException
//     Action: validator.ValidateNotNull("hello", "param")
//     Assert: tidak throw exception
//
// [ ] ValidateNotEmpty_EmptyString_ThrowsException
//     Test: "" → throws ArgumentException
//     Test: "   " (whitespace) → throws ArgumentException
//
// [ ] ValidateNotEmpty_ValidString_NoException
//     Test: "hello" → tidak throw
//
// [ ] ValidateInEnum_InvalidValue_ThrowsException
//     Action: validator.ValidateInEnum((MoodType)999, "Mood")
//     Assert: throws ArgumentException
//
// [ ] ValidateExists_NullEntity_ThrowsException
//     Action: validator.ValidateExists<object>(null, "Entity")
//     Assert: throws KeyNotFoundException
//
// ═══════════════════════════════════════════════════════════
// 📋 TEST CASE LIST — Middleware (4 test):
// ═══════════════════════════════════════════════════════════
//
// [ ] Middleware_ArgumentException_Returns400
//     Setup: next delegate throws ArgumentException
//     Assert: context.Response.StatusCode == 400
//     Assert: response body JSON mengandung "error" dan "statusCode"
//
// [ ] Middleware_KeyNotFoundException_Returns404
//     Setup: next delegate throws KeyNotFoundException
//     Assert: context.Response.StatusCode == 404
//
// [ ] Middleware_InvalidOperationException_Returns422
//     Setup: next delegate throws InvalidOperationException
//     Assert: context.Response.StatusCode == 422
//
// [ ] Middleware_UnknownException_Returns500
//     Setup: next delegate throws generic Exception
//     Assert: context.Response.StatusCode == 500
//
// ═══════════════════════════════════════════════════════════
// PERFORMANCE:
// ═══════════════════════════════════════════════════════════
//
// [ ] ValidateNotNull_1000Calls_Under5ms
//     Loop 1000x ValidateNotNull("valid", "p"), assert total < 5ms
//
// ═══════════════════════════════════════════════════════════
// CONTOH IMPLEMENTASI:
// ═══════════════════════════════════════════════════════════
//
// using Xunit;
// using myKisah.Utils;
// using System.Diagnostics;
//
// public class ValidationHelperTests
// {
//     private readonly ValidationHelper _validator = new();
//
//     [Fact]
//     public void ValidateNotNull_NullValue_ThrowsException()
//     {
//         var ex = Assert.Throws<ArgumentNullException>(() =>
//             _validator.ValidateNotNull<string>(null, "param"));
//         Assert.Contains("param", ex.Message);
//     }
//
//     [Fact]
//     public void ValidateNotEmpty_EmptyString_ThrowsException()
//     {
//         Assert.Throws<ArgumentException>(() =>
//             _validator.ValidateNotEmpty("", "param"));
//         Assert.Throws<ArgumentException>(() =>
//             _validator.ValidateNotEmpty("   ", "param"));
//     }
// }
