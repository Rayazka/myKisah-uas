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

//  TODO:
// [ ] 1. Isi dictionary _transitions dengan 4 entry:
//        (Draft, Submit)      → Submitted
//        (Submitted, Save)    → Saved
//        (Submitted, Reject)  → Rejected
//        (Rejected, Reset)    → Draft
//
// [ ] 2. Implement method Transition():
//        - Cek apakah key (currentState, trigger) ada di _transitions
//          pakai _transitions.TryGetValue()
//        - Jika ADA: return state tujuan
//        - Jika TIDAK: throw new InvalidOperationException(
//            $"Transisi tidak valid: {currentState} + {trigger}")
//
// [ ] 3. Implement method IsTerminal():
//        - Return true jika state == JournalState.Saved
//        - Saved adalah state akhir, tidak ada transisi keluar
//


public class JournalStateMachine
{
    // Tabel transisi: key = (state saat ini, trigger), value = state tujuan
    private readonly Dictionary<(JournalState, JournalTrigger), JournalState> _transitions = new()
    {
        // TODO: Isi 4 entry transisi di bawah ini
        // Format: { (stateAwal, trigger), stateTujuan }
        // Contoh: { (JournalState.Draft, JournalTrigger.Submit), JournalState.Submitted },
    };

    /// <summary>
    /// Melakukan transisi state berdasarkan trigger yang diberikan.
    /// </summary>
    /// <param name="currentState">State journal saat ini</param>
    /// <param name="trigger">Trigger yang memicu transisi</param>
    /// <returns>State baru setelah transisi</returns>
    /// <exception cref="InvalidOperationException">
    /// Dilempar jika kombinasi (currentState, trigger) tidak valid.
    /// Ditangkap Middleware → 422 Unprocessable Entity.
    /// </exception>
    public JournalState Transition(JournalState currentState, JournalTrigger trigger)
    {
        // TODO: Cek apakah transisi valid
        // 1. Coba _transitions.TryGetValue((currentState, trigger), out var nextState)
        // 2. Jika true → return nextState
        // 3. Jika false → throw InvalidOperationException
        throw new NotImplementedException("TODO: Implement Transition method");
    }

    /// <summary>
    /// Mengecek apakah state adalah terminal (tidak bisa transisi lagi).
    /// </summary>
    /// <param name="state">State yang ingin dicek</param>
    /// <returns>true jika terminal, false jika masih bisa transisi</returns>
    public bool IsTerminal(JournalState state)
    {
        // TODO: Return true jika state == JournalState.Saved
        throw new NotImplementedException("TODO: Implement IsTerminal");
    }
}
