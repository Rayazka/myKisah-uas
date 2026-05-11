namespace myKisah.Models;


// Journal System
// PENANGGUNG JAWAB: Azka

// ** Penjelasan:
// Entri jurnal harian milik seorang user. Disimpan di Data/journals.json.

// Setiap journal memiliki:
// - Id (string GUID) sebagai primary key
// - UserId (string) foreign key ke User.Id
// - Title (string) judul jurnal, tidak boleh kosong
// - Content (string) isi jurnal, tidak boleh kosong, panjang dibatasi config
// - Mood (MoodType enum) mood jurnal, harus valid di config ValidMoods
// - State (JournalState enum) status dalam state machine
// - CreatedAt (DateTime) timestamp


public class Journal
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public MoodType Mood { get; set; }
    public JournalState State { get; set; } = JournalState.Draft;
    public DateTime CreatedAt { get; set; }
}
