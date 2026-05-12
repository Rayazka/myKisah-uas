using myKisah.Interfaces;
using myKisah.Models;
using myKisah.Utils;

namespace myKisah.Repositories;

/// Repository untuk akses tabel response karakter di Data/characterResponses.json.
public class JsonCharacterResponseRepository : ICharacterResponseRepository
{
    // Variabel readonly untuk menyimpan referensi ke helper penyimpanan dan konfigurasi path
    private readonly JsonStorageHelper _storage;
    private readonly FilePathConfig _filePath;

    /// Constructor yang menerapkan Dependency Injection (DI).
    /// Kelas ini tidak membuat JsonStorageHelper sendiri, melainkan menerimanya dari luar,
    /// sehingga lebih mudah untuk di-mocking saat Unit Testing.
    public JsonCharacterResponseRepository(JsonStorageHelper storage, FilePathConfig filePath)
    {
        _storage = storage;
        _filePath = filePath;
    }

    /// Mengambil seluruh data respons karakter dari file JSON.
    /// Operasi ini membaca langsung dari storage menggunakan helper.
    public IEnumerable<CharacterResponse> GetAll()
    {
        return _storage.ReadJson<CharacterResponse>(_filePath.ResponsesFile);
    }

    /// Mencari spesifik satu data respons berdasarkan ID uniknya.
    /// Menggunakan LINQ FirstOrDefault agar mengembalikan null jika ID tidak ditemukan.
    public CharacterResponse? GetById(string id)
    {
        return GetAll().FirstOrDefault(r => r.Id == id);
    }

    /// Menambahkan data respons karakter baru ke dalam file JSON.
    /// Karena ini flat-file database (JSON), kita harus membaca semua data dulu,
    /// menambahkannya ke list memori, lalu menulis ulang keseluruhan list ke file.
    public void Add(CharacterResponse entity)
    {
        var responses = GetAll().ToList(); // Tarik semua data ke memori
        entity.Id = Guid.NewGuid().ToString(); // Generate ID unik (UUID) untuk data baru
        responses.Add(entity); // Tambahkan entitas baru ke dalam list
        _storage.WriteJson(_filePath.ResponsesFile, responses); // Timpa file JSON dengan list terbaru
    }

    /// Memperbarui (update) data respons yang sudah ada berdasarkan ID.
    public void Update(CharacterResponse entity)
    {
        var responses = GetAll().ToList();
        
        // Mencari posisi index dari data yang ingin diubah
        var index = responses.FindIndex(r => r.Id == entity.Id);
        
        //  Jika data tidak ditemukan (index -1), lemparkan error
        if (index == -1)
            throw new KeyNotFoundException($"CharacterResponse dengan Id '{entity.Id}' tidak ditemukan.");
            
        responses[index] = entity; // Ganti data lama dengan data baru di index tersebut
        _storage.WriteJson(_filePath.ResponsesFile, responses); // Simpan perubahan ke file JSON
    }

    /// Menghapus data respons dari file JSON berdasarkan ID.
    public void Delete(string id)
    {
        var responses = GetAll().ToList();
        
        // Menghapus semua elemen dalam list yang memiliki Id yang cocok
        responses.RemoveAll(r => r.Id == id);
        
        // Simpan sisa list ke dalam file JSON
        _storage.WriteJson(_filePath.ResponsesFile, responses);
    }

    /// INTI TABLE-DRIVEN: Filter response berdasarkan CharacterId + Mood.
    /// Metode ini mencari semua kemungkinan kalimat respons untuk satu karakter dan satu mood tertentu.
    public IEnumerable<CharacterResponse> GetByMood(string characterId, MoodType mood)
    {
        // Mengeksekusi query pencarian secara dinamis ke koleksi data
        return GetAll().Where(r => r.CharacterId == characterId && r.Mood == mood);
    }
}