using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HangmanGame : MonoBehaviour
{
    public static event System.Action<string> OnNewGame;          // нове слово (маскове)
    public static event System.Action<string> OnMaskUpdated;      // маска оновилась
    public static event System.Action<char, bool> OnLetterGuessed;    // буква, правильно/ні
    public static event System.Action<int> OnWrongGuessChanged;// кількість помилок
    public static event System.Action<bool> OnGameOver;         // true = перемога
    public static event System.Action<string> OnWordRevealed;     // слово при поразці

    [Tooltip("Максимальна кількість помилок (відповідає частинам тіла на шибениці)")]
    [SerializeField] private int maxWrongGuesses = 6;

    private WordLoader _wordLoader;

    private string _secretWord = "";
    private char[] _mask;                         
    private HashSet<char> _guessedLetters = new HashSet<char>();
    private int _wrongGuesses = 0;
    private bool _gameActive = false;

    public int MaxWrongGuesses => maxWrongGuesses;
    public int WrongGuesses => _wrongGuesses;
    public bool GameActive => _gameActive;
    public IReadOnlyCollection<char> GuessedLetters => _guessedLetters;

    private void Awake()
    {
        _wordLoader = GetComponent<WordLoader>();
        if (_wordLoader == null)
            _wordLoader = gameObject.AddComponent<WordLoader>();
    }

    private void Start()
    {
        StartNewGame();
    }

    public void StartNewGame()
    {
        _secretWord = _wordLoader.GetRandomWord();
        _mask = new string('_', _secretWord.Length).ToCharArray();
        _guessedLetters = new HashSet<char>();
        _wrongGuesses = 0;
        _gameActive = true;

        Debug.Log($"[HangmanGame] Нове слово: {_secretWord}");
        OnNewGame?.Invoke(GetMaskString());
        OnWrongGuessChanged?.Invoke(_wrongGuesses);
    }

    public bool GuessLetter(char letter)
    {
        if (!_gameActive) return false;

        letter = char.ToUpperInvariant(letter);

        if (_guessedLetters.Contains(letter))
        {
            Debug.Log($"[HangmanGame] Буква '{letter}' вже була введена.");
            return false;
        }

        _guessedLetters.Add(letter);

        bool correct = _secretWord.Contains(letter);

        if (correct)
        {
            for (int i = 0; i < _secretWord.Length; i++)
                if (_secretWord[i] == letter)
                    _mask[i] = letter;

            OnMaskUpdated?.Invoke(GetMaskString());
        }
        else
        {
            _wrongGuesses++;
            OnWrongGuessChanged?.Invoke(_wrongGuesses);
        }

        OnLetterGuessed?.Invoke(letter, correct);

        CheckGameOver();
        return correct;
    }

    private string GetMaskString() =>
        string.Join(" ", _mask.Select(c => c == '_' ? "_" : c.ToString()));

    private void CheckGameOver()
    {
        if (!_mask.Contains('_'))
        {
            _gameActive = false;
            OnGameOver?.Invoke(true);
        }
        else if (_wrongGuesses >= maxWrongGuesses)
        {
            _gameActive = false;
            OnGameOver?.Invoke(false);
            OnWordRevealed?.Invoke(_secretWord);
        }
    }
}
