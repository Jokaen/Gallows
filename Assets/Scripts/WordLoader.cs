using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WordLoader : MonoBehaviour
{
    [Tooltip("Назва файлу в папці Resources (без розширення)")]
    [SerializeField] private string fileName = "words";

    private List<string> _words = new List<string>();

    private void Awake()
    {
        LoadWords();
    }

    private void LoadWords()
    {
        TextAsset textAsset = Resources.Load<TextAsset>(fileName);

        if (textAsset == null)
        {
            Debug.LogError($"[WordLoader] Файл '{fileName}.txt' не знайдено в папці Resources!");
            return;
        }

        _words = textAsset.text
            .Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries)
            .Select(w => w.Trim().ToUpperInvariant())
            .Where(w => w.Length > 0)
            .ToList();

        Debug.Log($"[WordLoader] Завантажено {_words.Count} слів.");
    }

    public string GetRandomWord()
    {
        if (_words.Count == 0)
        {
            Debug.LogWarning("[WordLoader] Список слів порожній! Повертаю заглушку.");
            return "ЄДИНОРІГ";
        }

        return _words[Random.Range(0, _words.Count)];
    }
}
