using myKisah.Interfaces;
using myKisah.Models;
using myKisah.Utils;

namespace myKisah.Services;

/// <summary>
/// Business logic untuk Character Companion.
/// TEKNIK: TABLE-DRIVEN — GenerateResponse() lookup ke JSON via repository.
/// TIDAK ADA IF/SWITCH di seluruh flow.
/// </summary>
public class CharacterService : ServiceBase, ICharacterService
{
    private readonly ICharacterRepository _characterRepo;
    private readonly ICharacterResponseRepository _responseRepo;

    public CharacterService(
        ICharacterRepository characterRepo,
        ICharacterResponseRepository responseRepo)
    {
        _characterRepo = characterRepo;
        _responseRepo = responseRepo;
    }

    protected override string ServiceName => "CharacterService";

    public IEnumerable<Character> GetAllCharacters()
    {
        return _characterRepo.GetAll();
    }

    /// <summary>
    /// INTI TABLE-DRIVEN: Lookup response dari characterResponses.json.
    /// Validasi input → delegasi ke repository.GetByMood() → return string response.
    /// TIDAK ADA IF/SWITCH — seluruh mapping mood→response ada di JSON file.
    /// </summary>
    public string GenerateResponse(string characterId, MoodType mood)
    {
        Validator.ValidateNotEmpty(characterId, "CharacterId");
        Validator.ValidateInEnum(mood, "Mood");

        var responses = _responseRepo.GetByMood(characterId, mood);
        var response = responses.FirstOrDefault();

        Validator.ValidateExists(response,
            $"Response untuk character '{characterId}' dengan mood '{mood}'");

        return response!.Response;
    }
}
