namespace myKisah.Models;

// TEKNIK: Runtime Configuration (ValidMoods dibaca dari appsettings.json)
    // Ini memungkinkan penambahan/pengurangan mood tanpa recompile.
    // Enum tetap dipakai untuk compile-time type safety, tapi validasi
    // runtime memastikan hanya mood di config yang diterima.
// PENANGGUNG JAWAB: Azka

// ** Penjelasan:
// Enum yang mendefinisikan jenis mood yang valid di seluruh aplikasi.

public enum MoodType
{
    Happy,
    Sad,
    Angry,
    Anxious,
    Calm
}
