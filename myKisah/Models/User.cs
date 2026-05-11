namespace myKisah.Models;

// User Management System
// PENANGGUNG JAWAB: Azka

// ** Penjelasan:
// Entitas pengguna aplikasi. Disimpan di Data/users.json.
//
// Setiap user memiliki:
// - Id (string GUID) sebagai primary key unik
// - Username (string) yang harus unik, tidak boleh kosong
// - CreatedAt (DateTime) timestamp saat registrasi


public class User
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
