namespace myKisah.Models;

// Journal System (State Machine)
// TEKNIK: Automata
// PENANGGUNG JAWAB: Azka

// ** Penjelasan:
// Enum yang mendefinisikan state/status journal dalam state machine.
// Digunakan oleh Journal.State dan JournalStateMachine.
// ** Flow:
//   Draft → Submitted → Saved (terminal)
//   Draft → Submitted → Rejected → Draft (reset)


public enum JournalState
{
    Draft,
    Submitted,
    Saved,
    Rejected
}
