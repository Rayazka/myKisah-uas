using myKisah.Models;

namespace myKisah.Automata;

// Journal System
// PENANGGUNG JAWAB: Azka
// TEKNIK: Automata (Finite State Machine)
//
// ** Penjelasan:
// State machine untuk mengatur lifecycle  sebuah journal.
// Journal tidak bisa loncat state sembarangan — harus ikuti aturan transisi

// Flow state:
//   Draft ──[Submit]──► Submitted
//                           │
//                     ┌─────┴─────┐
//                   [Save]      [Reject]
//                     │            │
//                     ▼            ▼
//                  Saved       Rejected
//                (terminal)       │
//                              [Reset]
//                                 │
//                                 ▼
//                               Draft
//

public class JournalStateMachine
{
    // Proses transisi state berdasarkan trigger yang diberikan.
    private readonly Dictionary<(JournalState, JournalTrigger), JournalState> _transitions = new()
    {
        // Draft → Submitted (trigger: Submit). Ketika journal masih di Draft, user bisa Submit untuk pindah ke Submitted
        // Submitted → Saved (trigger: Save). Ketika journal sudah di Submitted, user bisa Save untuk pindah ke Saved
        // Submitted → Rejected (trigger: Reject). Ketika journal sudah di Submitted, user bisa Reject untuk pindah ke Rejected
        // Rejected → Draft (trigger: Reset). Ketika journal sudah di Rejected, user bisa Reset untuk kembali ke Draft

        {(JournalState.Draft, JournalTrigger.Submit), JournalState.Submitted},
        {(JournalState.Submitted, JournalTrigger.Save), JournalState.Saved},
        {(JournalState.Submitted, JournalTrigger.Reject), JournalState.Rejected},
        {(JournalState.Rejected, JournalTrigger.Reset), JournalState.Draft},
    };
    // Melakukan transisi state berdasarkan trigger yang diberikan.
    public JournalState Transition(JournalState currentState, JournalTrigger trigger)
    {        
        // Cek apakah transisi valid. Jika valid, return state tujuan. Jika tidak, lempar exception.
        if(_transitions.TryGetValue((currentState, trigger), out var nextState))
        {
            return nextState;
        }

        throw new InvalidOperationException($"Transisi tidak valid: state '{currentState}' tidak dapat menerima '{trigger}'");
    }

    // Mengecek apakah state adalah terminal (tidak bisa transisi lagi). 
        // Supaya kita tahu kalau journal sudah selesai (Saved) atau belum (karena tidak ada transisi keluar dari Saved)
    public bool IsTerminal(JournalState state)
    {
        // Mengembalikan true jika state adalah Saved, karena Saved adalah state akhir
        return state == JournalState.Saved;
    }
}
