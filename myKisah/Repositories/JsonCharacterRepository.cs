using myKisah.Interfaces;
using myKisah.Models;
using myKisah.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace myKisah.Repositories;

/// Repository untuk mengelola akses data (CRUD) entitas Character.
/// Data disimpan dalam format JSON flat-file di Data/characters.json.
/// Mengimplementasikan ICharacterRepository yang merupakan turunan dari IRepository<Character>.
public class JsonCharacterRepository : ICharacterRepository
{
    // Dependensi yang diinjeksikan (Dependency Injection).
    // Menggunakan readonly agar aman dan tidak berubah setelah inisialisasi.
    private readonly JsonStorageHelper _storage;
    private readonly FilePathConfig _filePath;

    /// Constructor untuk menginisialisasi repository dengan helper penyimpanan dan konfigurasi path.
    /// Ini memastikan repository ini terisolasi dari detail cara baca/tulis file secara langsung.
    public JsonCharacterRepository(JsonStorageHelper storage, FilePathConfig filePath)
    {
        _storage = storage;
        _filePath = filePath;
    }

    /// Mengambil seluruh daftar karakter dari file JSON.
    public IEnumerable<Character> GetAll()
    {
        // Mendelegasikan proses deserialization ke helper milik modul utility
        return _storage.ReadJson<Character>(_filePath.CharactersFile);
    }

    /// Mencari spesifik satu data karakter berdasarkan ID uniknya.
    public Character? GetById(string id)
    {
        // FirstOrDefault akan mengembalikan null jika ID tidak ditemukan,
        // mencegah terjadinya exception (error crash) jika data kosong.
        return GetAll().FirstOrDefault(c => c.Id == id);
    }

    /// Menambahkan karakter baru ke dalam penyimpanan.
    public void Add(Character entity)
    {
        // 1. Tarik semua data yang ada di JSON ke dalam memori (List)
        var characters = GetAll().ToList();
        
        // 2. Buat ID unik (UUID) secara otomatis untuk karakter baru
        entity.Id = Guid.NewGuid().ToString();
        
        // 3. Tambahkan ke list memori
        characters.Add(entity);
        
        // 4. Timpa (overwrite) file JSON lama dengan list yang baru
        _storage.WriteJson(_filePath.CharactersFile, characters);
    }

    /// Memperbarui data karakter yang sudah ada (misalnya mengubah nama karakter).
    public void Update(Character entity)
    {
        var characters = GetAll().ToList();
        
        // Cari index/posisi data karakter yang ingin diubah berdasarkan ID-nya
        var index = characters.FindIndex(c => c.Id == entity.Id);
        
        // Validasi: Pastikan data yang mau diupdate benar-benar ada
        if (index == -1)
            throw new KeyNotFoundException($"Character dengan Id '{entity.Id}' tidak ditemukan.");
            
        // Ganti objek lama dengan objek baru pada index tersebut
        characters[index] = entity;
        
        // Simpan perubahan ke file
        _storage.WriteJson(_filePath.CharactersFile, characters);
    }

    /// Menghapus karakter dari sistem berdasarkan ID.
    public void Delete(string id)
    {
        var characters = GetAll().ToList();
        
        // RemoveAll akan mencari dan menghapus semua elemen yang ID-nya cocok
        characters.RemoveAll(c => c.Id == id);
        
        // Timpa file JSON dengan data yang sudah dikurangi
        _storage.WriteJson(_filePath.CharactersFile, characters);
    }

    /// Method spesifik (di luar CRUD standar) untuk mencari karakter berdasarkan namanya.

    public Character? GetByName(string name)
    {
        return GetAll().FirstOrDefault(c =>
        /// Pencarian dilakukan secara case-insensitive (tidak mempedulikan huruf besar/kecil).
            c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}