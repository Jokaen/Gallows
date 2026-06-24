using UnityEngine;

[RequireComponent(typeof(HangmanGame))]
public class HangmanDrawing : MonoBehaviour
{
    [Header("Кольори")]
    [SerializeField] private Color gallowsColor = new Color(0.55f, 0.35f, 0.15f); // коричневий
    [SerializeField] private Color bodyColor = new Color(0.2f, 0.2f, 0.2f);  // темно-сірий
    [SerializeField] private Color headColor = new Color(0.95f, 0.8f, 0.6f);  // тілесний

    [Header("Розміри")]
    [SerializeField] private float lineWidth = 0.08f;

    private GameObject[] _bodyParts;
    private int _partsShown = 0;

    private void OnEnable()
    {
        HangmanGame.OnWrongGuessChanged += HandleWrongGuess;
        HangmanGame.OnNewGame += HandleNewGame;
    }

    private void OnDisable()
    {
        HangmanGame.OnWrongGuessChanged -= HandleWrongGuess;
        HangmanGame.OnNewGame -= HandleNewGame;
    }

    private void Start()
    {
        BuildGallows();
        BuildBodyParts();
        HideAllParts();
    }


    private void HandleNewGame(string _)
    {
        HideAllParts();
        _partsShown = 0;
    }

    private void HandleWrongGuess(int wrongCount)
    {
        while (_partsShown < wrongCount && _partsShown < _bodyParts.Length)
        {
            _bodyParts[_partsShown].SetActive(true);
            _partsShown++;
        }
    }

    private void BuildGallows()
    {
        GameObject gallows = new GameObject("Gallows");
        gallows.transform.SetParent(transform);

        float[,] lines =
        {
            { -3f, -3f,  0f, -3f },   // основа
            { -2f, -3f, -2f,  3f },   // вертикальний стовп
            { -2f,  3f,  1f,  3f },   // горизонтальна балка
            {  1f,  3f,  1f,  2f },   // мотузка
        };

        for (int i = 0; i < lines.GetLength(0); i++)
        {
            CreateLine(gallows.transform,
                new Vector3(lines[i, 0], lines[i, 1]),
                new Vector3(lines[i, 2], lines[i, 3]),
                gallowsColor, lineWidth, $"GallowsLine{i}");
        }
    }
    private void BuildBodyParts()
    {
        _bodyParts = new GameObject[6];

        // 0 — Голова (коло)
        _bodyParts[0] = CreateCircle(transform, new Vector3(1f, 1.3f), 0.5f, headColor, "Head");

        // 1 — Тулуб
        _bodyParts[1] = CreateLineGO(transform,
            new Vector3(1f, 0.8f), new Vector3(1f, -0.8f), bodyColor, lineWidth, "Body");

        // 2 — Ліва рука
        _bodyParts[2] = CreateLineGO(transform,
            new Vector3(1f, 0.4f), new Vector3(0.2f, -0.2f), bodyColor, lineWidth, "ArmLeft");

        // 3 — Права рука
        _bodyParts[3] = CreateLineGO(transform,
            new Vector3(1f, 0.4f), new Vector3(1.8f, -0.2f), bodyColor, lineWidth, "ArmRight");

        // 4 — Ліва нога
        _bodyParts[4] = CreateLineGO(transform,
            new Vector3(1f, -0.8f), new Vector3(0.3f, -1.8f), bodyColor, lineWidth, "LegLeft");

        // 5 — Права нога
        _bodyParts[5] = CreateLineGO(transform,
            new Vector3(1f, -0.8f), new Vector3(1.7f, -1.8f), bodyColor, lineWidth, "LegRight");
    }

    private void HideAllParts()
    {
        if (_bodyParts == null) return;
        foreach (var part in _bodyParts)
            if (part != null) part.SetActive(false);
    }

    private void CreateLine(Transform parent, Vector3 start, Vector3 end,
                            Color color, float width, string goName)
    {
        var go = new GameObject(goName);
        go.transform.SetParent(parent);
        var lr = go.AddComponent<LineRenderer>();
        SetupLineRenderer(lr, start, end, color, width);
    }

    private GameObject CreateLineGO(Transform parent, Vector3 start, Vector3 end,
                                    Color color, float width, string goName)
    {
        var go = new GameObject(goName);
        go.transform.SetParent(parent);
        var lr = go.AddComponent<LineRenderer>();
        SetupLineRenderer(lr, start, end, color, width);
        return go;
    }

    private void SetupLineRenderer(LineRenderer lr, Vector3 start, Vector3 end,
                                   Color color, float width)
    {
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.startWidth = lr.endWidth = width;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = lr.endColor = color;
        lr.useWorldSpace = false;
        lr.sortingOrder = 1;
    }

    private GameObject CreateCircle(Transform parent, Vector3 center,
                                    float radius, Color color, string goName)
    {
        var go = new GameObject(goName);
        go.transform.SetParent(parent);
        go.transform.localPosition = center;

        var lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.loop = true;
        lr.startWidth = lr.endWidth = lineWidth;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = lr.endColor = color;
        lr.sortingOrder = 1;

        const int segments = 32;
        lr.positionCount = segments;
        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            lr.SetPosition(i, new Vector3(Mathf.Cos(angle) * radius,
                                          Mathf.Sin(angle) * radius, 0f));
        }

        return go;
    }
}
