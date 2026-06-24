using UnityEngine;

public class GameBootStrapper : MonoBehaviour
{
    [Header("Налаштування гри")]
    [Tooltip("Максимум помилок (рекомендовано 6 — по одній частині тіла)")]
    [SerializeField] private int maxWrongGuesses = 6;

    private void Awake()
    {
        EnsureComponent<WordLoader>();

        var game = EnsureComponent<HangmanGame>();

        EnsureComponent<HangmanDrawing>();
        EnsureComponent<HangmanUI>();

        Debug.Log("[GameBootstrapper] Гра 'Шибениця' ініціалізована!");
    }

    private T EnsureComponent<T>() where T : Component
    {
        var c = GetComponent<T>();
        if (c == null) c = gameObject.AddComponent<T>();
        return c;
    }
}
