using System;
using System.Collections.Generic;

/// Helper untuk Design by Contract (DbC) — precondition checks.
/// Dipakai di SEMUA method Service untuk validasi input SEBELUM logic.
///
/// TEKNIK: Generics + Code Reuse
/// - Generics <T>: ValidateNotNull<T>() satu method untuk semua tipe
/// - Code Reuse: dipakai semua Service, tidak ada duplikasi
///
/// Exception yang dilempar → ditangkap ErrorHandlingMiddleware:
///   ArgumentNullException → 400 Bad Request
///   ArgumentException     → 400 Bad Request
///   KeyNotFoundException  → 404 Not Found

namespace myKisah.Utils
{
    public class ValidationHelper
    {
        // 1. Cek null untuk semua tipe
        public void ValidateNotNull<T>(T? value, string name)
        {
            if (value == null)
                throw new ArgumentNullException(name, $"{name} tidak boleh null.");
        }

        // 2. Cek string kosong atau whitespace
        public void ValidateNotEmpty(string value, string name)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{name} tidak boleh kosong.", name);
        }

        // 3. Cek apakah nilai valid pada enum
        public void ValidateInEnum<T>(T value, string name) where T : struct, Enum
        {
            if (!Enum.IsDefined(typeof(T), value))
                throw new ArgumentException($"'{value}' bukan nilai {name} yang valid.", name);
        }

        // 4. Cek apakah entity ada
        public void ValidateExists<T>(T? entity, string name)
        {
            if (entity == null)
                throw new KeyNotFoundException($"{name} tidak ditemukan.");
        }
    }
}