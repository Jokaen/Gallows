using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HangmanGame))]
public class HangmanUI : MonoBehaviour
{
    private static readonly string[] UkrAlphabet =
    {
        "А","Б","В","Г","Ґ","Д","Е","Є","Ж","З",
        "И","І","Ї","Й","К","Л","М","Н","О","П",
        "Р","С","Т","У","Ф","Х","Ц","Ч","Ш","Щ",
        "Ь","Ю","Я"
    };

    private TMP_Text _wordText;
    private TMP_Text _wrongCountText;
    private TMP_Text _resultTitleText;
    private TMP_Text _resultBodyText;
    private GameObject _resultPanel;
    private Button[] _letterButtons;
    private HangmanGame _game;

    private void Awake()
    {
        _game = GetComponent<HangmanGame>();
        BuildUI();
    }

    private void OnEnable()
    {
        HangmanGame.OnNewGame += HandleNewGame;
        HangmanGame.OnMaskUpdated += HandleMaskUpdate;
        HangmanGame.OnWrongGuessChanged += HandleWrongCount;
        HangmanGame.OnGameOver += HandleGameOver;
        HangmanGame.OnWordRevealed += HandleWordRevealed;
        HangmanGame.OnLetterGuessed += HandleLetterGuessed;
    }

    private void OnDisable()
    {
        HangmanGame.OnNewGame -= HandleNewGame;
        HangmanGame.OnMaskUpdated -= HandleMaskUpdate;
        HangmanGame.OnWrongGuessChanged -= HandleWrongCount;
        HangmanGame.OnGameOver -= HandleGameOver;
        HangmanGame.OnWordRevealed -= HandleWordRevealed;
        HangmanGame.OnLetterGuessed -= HandleLetterGuessed;
    }


    private void HandleNewGame(string mask)
    {
        _wordText.text = mask;
        _wrongCountText.text = $"Помилки: 0 / {_game.MaxWrongGuesses}";
        _resultPanel.SetActive(false);
        ResetButtons();
    }

    private void HandleMaskUpdate(string mask) => _wordText.text = mask;

    private void HandleWrongCount(int count) =>
        _wrongCountText.text = $"Помилки: {count} / {_game.MaxWrongGuesses}";

    private void HandleGameOver(bool win)
    {
        _resultTitleText.text = win ? "🎉 Перемога!" : "💀 Поразка!";
        _resultPanel.SetActive(true);
        DisableAllButtons();
    }

    private void HandleWordRevealed(string word) =>
        _resultBodyText.text = $"Слово було:\n{word}";

    private void HandleLetterGuessed(char letter, bool correct)
    {
        foreach (var btn in _letterButtons)
        {
            if (btn == null) continue;
            var label = btn.GetComponentInChildren<TMP_Text>();
            if (label != null && label.text == letter.ToString())
            {
                btn.interactable = false;
                var img = btn.GetComponent<Image>();
                if (img != null)
                    img.color = correct
                        ? new Color(0.3f, 0.75f, 0.3f)   
                        : new Color(0.75f, 0.3f, 0.3f);  
                break;
            }
        }
    }


    private void BuildUI()
    {
        var canvasGO = new GameObject("HangmanCanvas");
        canvasGO.transform.SetParent(transform);
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.75f;
        canvasGO.AddComponent<GraphicRaycaster>();

        _wordText = CreateText(canvasGO.transform, "WordText",
            new Vector2(0.5f, 0.78f), new Vector2(1200f, 90f),
            "", 46, FontStyles.Bold, TextAlignmentOptions.Center);


        _wordText.rectTransform.anchorMin = new Vector2(0.5f, 0.85f);
        _wordText.rectTransform.anchorMax = new Vector2(0.5f, 0.85f);

        _wrongCountText = CreateText(canvasGO.transform, "WrongCount",
            new Vector2(0.5f, 0.94f), new Vector2(700f, 60f),
            $"Помилки: 0 / 6", 30, FontStyles.Bold, TextAlignmentOptions.Center);

        BuildKeyboard(canvasGO.transform);

        BuildResultPanel(canvasGO.transform);
    }

    private void BuildKeyboard(Transform parent)
    {
        _letterButtons = new Button[UkrAlphabet.Length];

        var kbGO = new GameObject("Keyboard");
        kbGO.transform.SetParent(parent, false);
        var rt = kbGO.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 0f);
        rt.anchorMax = new Vector2(1f, 0f);
        rt.pivot = new Vector2(0.5f, 0f);

        rt.anchoredPosition = new Vector2(0f, 40f);
        rt.sizeDelta = new Vector2(0f, 170f);

        var grid = kbGO.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(60f, 60f);
        grid.spacing = new Vector2(6f, 6f);
        grid.padding = new RectOffset(10, 10, 10, 10);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 17;
        grid.childAlignment = TextAnchor.MiddleCenter;

        for (int i = 0; i < UkrAlphabet.Length; i++)
        {
            string letter = UkrAlphabet[i];
            var btnGO = new GameObject($"Btn_{letter}");
            btnGO.transform.SetParent(kbGO.transform, false);

            var img = btnGO.AddComponent<Image>();
            img.color = new Color(0.2f, 0.45f, 0.7f);

            var btn = btnGO.AddComponent<Button>();
            var colors = btn.colors;
            colors.highlightedColor = new Color(0.3f, 0.6f, 0.9f);
            colors.pressedColor = new Color(0.1f, 0.3f, 0.5f);
            btn.colors = colors;

            var txtGO = new GameObject("Label");
            txtGO.transform.SetParent(btnGO.transform, false);
            var tmp = txtGO.AddComponent<TextMeshProUGUI>();
            tmp.text = letter;
            tmp.fontSize = 28;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
            var trt = txtGO.GetComponent<RectTransform>();
            trt.anchorMin = Vector2.zero;
            trt.anchorMax = Vector2.one;
            trt.sizeDelta = Vector2.zero;

            int idx = i; 
            btn.onClick.AddListener(() => OnLetterButtonClicked(UkrAlphabet[idx][0]));
            _letterButtons[i] = btn;
        }
    }

    private void BuildResultPanel(Transform parent)
    {
        _resultPanel = new GameObject("ResultPanel");
        _resultPanel.transform.SetParent(parent, false);
        var img = _resultPanel.AddComponent<Image>();
        img.color = new Color(0f, 0f, 0f, 0.75f);
        var rt = _resultPanel.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;

        _resultTitleText = CreateText(_resultPanel.transform, "ResultTitle",
            new Vector2(0.5f, 0.6f), new Vector2(600f, 80f),
            "", 54, FontStyles.Bold, TextAlignmentOptions.Center);

        _resultBodyText = CreateText(_resultPanel.transform, "ResultBody",
            new Vector2(0.5f, 0.47f), new Vector2(600f, 70f),
            "", 34, FontStyles.Normal, TextAlignmentOptions.Center);

        var btnGO = new GameObject("NewGameButton");
        btnGO.transform.SetParent(_resultPanel.transform, false);
        var brt = btnGO.AddComponent<RectTransform>();
        brt.anchorMin = new Vector2(0.5f, 0.3f);
        brt.anchorMax = new Vector2(0.5f, 0.3f);
        brt.pivot = new Vector2(0.5f, 0.5f);
        brt.anchoredPosition = Vector2.zero;
        brt.sizeDelta = new Vector2(220f, 60f);
        var bimg = btnGO.AddComponent<Image>();
        bimg.color = new Color(0.15f, 0.6f, 0.3f);
        var newGameBtn = btnGO.AddComponent<Button>();
        newGameBtn.onClick.AddListener(OnNewGameClicked);

        var btnLabel = new GameObject("Label");
        btnLabel.transform.SetParent(btnGO.transform, false);
        var tmp = btnLabel.AddComponent<TextMeshProUGUI>();
        tmp.text = "Нова гра";
        tmp.fontSize = 26;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        var lrt = btnLabel.GetComponent<RectTransform>();
        lrt.anchorMin = Vector2.zero;
        lrt.anchorMax = Vector2.one;
        lrt.sizeDelta = Vector2.zero;

        _resultPanel.SetActive(false);
    }


    private TMP_Text CreateText(Transform parent, string name,
        Vector2 anchorCenter, Vector2 size,
        string content, float fontSize,
        FontStyles style, TextAlignmentOptions align)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorCenter;
        rt.anchorMax = anchorCenter;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = size;
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = content;
        tmp.fontSize = fontSize;
        tmp.fontStyle = style;
        tmp.alignment = align;
        tmp.color = Color.white;
        return tmp;
    }

    private void ResetButtons()
    {
        foreach (var btn in _letterButtons)
        {
            if (btn == null) continue;
            btn.interactable = true;
            var img = btn.GetComponent<Image>();
            if (img != null) img.color = new Color(0.2f, 0.45f, 0.7f);
        }
    }

    private void DisableAllButtons()
    {
        foreach (var btn in _letterButtons)
            if (btn != null) btn.interactable = false;
    }

    private void OnLetterButtonClicked(char letter) => _game.GuessLetter(letter);
    private void OnNewGameClicked() => _game.StartNewGame();
}