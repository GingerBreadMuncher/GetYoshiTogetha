using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [Header("Game Elements")]
    [Range(1, 5)]
    [SerializeField] public int difficulty = Normal;
    [SerializeField] Transform gameHolder;
    [SerializeField] Transform piecePrefab;
    [SerializeField] int piecesCorrect = 0;
    [SerializeField] bool gameOver = false;
    [SerializeField] SceneLoader sceneLoader;
    [SerializeField] MoveController moveController;


    [Header("UI Elements")]
    [SerializeField] List<Texture2D> imageTextures;
    [SerializeField] Transform levelSelectPanel;
    [SerializeField] Image levelSelectPrefab;
    [SerializeField] Button backButton;
    [SerializeField] Button playAgainButton;
    [SerializeField] GameObject winScreen;
    [SerializeField] GameObject chooseYoshiScreen;
    [SerializeField] SliderController sliderController;
    [SerializeField] GameModeSelect gameModeSelect;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI difficultySeconds;
    [SerializeField] GameObject gameOverScreen;
    [SerializeField] Camera cam;


    // Difficulty levels
    public static int VeryEasy = 2;
    public static int Easy = 3;
    public static int Normal = 4;
    public static int Hard = 5;

    List<Transform> pieces;
    Transform draggingPiece = null;
    Vector2Int dimensions;
    Vector3 offset;
    float width;
    float height;
    TimeSpan timeRemaining;
    bool jigsawComplete = false;
    public bool CountdownMode { get; set; }

    void Start()
    {
        foreach (Texture2D texture in imageTextures)
        {
            Image image = Instantiate(levelSelectPrefab, levelSelectPanel);
            image.sprite = Sprite.Create(texture, new Rect(0,0, texture.width, texture.height), Vector2.zero);
            image.GetComponent<Button>().onClick.AddListener(delegate { StartGame(texture); });
        }
        difficulty = PlayerPrefs.GetInt("DifficultyLevel", difficulty);
        sliderController.slider.value = difficulty - 2;
        if (CountdownMode) { difficultySeconds.enabled = true; }
    }

    public void StartGame(Texture2D puzzleTexture)
    {
        chooseYoshiScreen.gameObject.SetActive(false);
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(sceneLoader.LoadGameScene);
        pieces = new List<Transform>();
        dimensions = GetDimensions(puzzleTexture, difficulty);
        CreatePuzzlePieces(puzzleTexture);
        Scatter();
        UpdateBorder();
        if (CountdownMode) { StartCoroutine(CountdownCoroutine()); }
    }

    Vector2Int GetDimensions(Texture2D puzzleTexture, int difficulty)
    {
        Vector2Int dimensions = Vector2Int.zero;
        if (puzzleTexture.width < puzzleTexture.height)
        {
            dimensions.x = difficulty;
            dimensions.y = (difficulty * puzzleTexture.height) / puzzleTexture.width;
        }
        else 
        {
            dimensions.x = (difficulty * puzzleTexture.width) / puzzleTexture.height;
            dimensions.y = difficulty;
        }
        return dimensions;
    }

    void CreatePuzzlePieces(Texture2D puzzleTexture)
    {
        height = 1f / dimensions.y;
        float aspect = (float)puzzleTexture.width / puzzleTexture.height;
        width = aspect / dimensions.x;

        for (int row = 0; row < dimensions.y; row++)
        {
            for (int col = 0; col < dimensions.x; col++)
            {
                Transform piece = Instantiate(piecePrefab, gameHolder);
                piece.localPosition = new Vector3(
                    (-width * dimensions.x / 2) + (width * col) + (width / 2),
                    (-height * dimensions.y / 2) + (width * row) + (height / 2));
                piece.localScale = new Vector3(width, height, 1f);

                piece.name = $"Piece {(row * dimensions.x) + col}";
                pieces.Add(piece);


                float width1 = 1f / dimensions.x;
                float height1 = 1f / dimensions.y;
                Vector2[] uv = new Vector2[4];
                uv[0] = new Vector2(width1 * col, height1 * row);
                uv[1] = new Vector2(width1 * (col + 1), height1 * row);
                uv[2] = new Vector2(width1 * col, height1 * (row + 1));
                uv[3] = new Vector2(width1 * (col + 1), height1 * (row + 1));

                Mesh mesh = piece.GetComponent<MeshFilter>().mesh;
                mesh.uv = uv;

                piece.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", puzzleTexture);
            }
        }
    }

    IEnumerator CountdownCoroutine()
    {
        timerText.gameObject.SetActive(true);
        int seconds = 0;
        switch(difficulty)
        {
            case 2: seconds = 10; break;
            case 3: seconds = 30; break;
            case 4: seconds = 45; break;
            case 5: seconds = 60; break;
        }
        timeRemaining = new TimeSpan(0, 0, seconds);
        timerText.text = timeRemaining.ToString(@"hh\:mm\:ss");
        while (timeRemaining.TotalSeconds > 0 && !jigsawComplete)
        {
            yield return new WaitForSeconds(1);
            if (jigsawComplete) { break; }
            timeRemaining = timeRemaining.Add(TimeSpan.FromSeconds(-1));
            timerText.text = timeRemaining.ToString(@"hh\:mm\:ss");
        }
        if (timeRemaining.TotalSeconds == 0) { GameOver(); }
    }

    void Scatter()
    {
        float orthoHeight = Camera.main.orthographicSize;
        float aspect = (float)Screen.width / Screen.height;
        float orthoWidth = (aspect * orthoHeight);

        float pieceWidth = width * gameHolder.localScale.x;
        float pieceHeight = height * gameHolder.localScale.y;

        orthoHeight -= pieceHeight;
        orthoWidth -= pieceWidth;

        foreach (Transform piece in pieces)
        {
            float x = UnityEngine.Random.Range(-orthoWidth, orthoWidth);
            float y = UnityEngine.Random.Range(-orthoHeight, orthoHeight);
            piece.position = new Vector2(x, y);
        }
    }

    void UpdateBorder()
    {
        LineRenderer lineRenderer = gameHolder.GetComponent<LineRenderer>();

        float halfWidth = (width * dimensions.x) / 2f;
        float halfHeight = (height * dimensions.y) / 2f;
        float borderZ = 0f;

        lineRenderer.SetPosition(0,new Vector3(-halfWidth, halfHeight, borderZ));
        lineRenderer.SetPosition(1,new Vector3(halfWidth, halfHeight, borderZ));
        lineRenderer.SetPosition(2,new Vector3(halfWidth, -halfHeight, borderZ));
        lineRenderer.SetPosition(3,new Vector3(-halfWidth, -halfHeight, borderZ));

        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        lineRenderer.enabled = true;
    }

    void Snap()
    {
        int pieceIndex = pieces.IndexOf(draggingPiece);

        int col = pieceIndex % dimensions.x;
        int row = pieceIndex / dimensions.x;

        Vector2 targetPosition = new((-width * dimensions.x / 2) + (width * col) + (width / 2),
            (-height * dimensions.y / 2) + (height * row) + (height / 2));

        if (Vector2.Distance(draggingPiece.localPosition, targetPosition) < (width / 3))
        {
            draggingPiece.localPosition = targetPosition;
            draggingPiece.GetComponent<BoxCollider2D>().enabled = false;
            piecesCorrect += 1;
            if (piecesCorrect == pieces.Count)
            {
                JigsawComplete();
            }
        }
    }

    void JigsawComplete()
    {
        jigsawComplete = true;
        winScreen.SetActive(true);
        playAgainButton.gameObject.SetActive(true);
        if (CountdownMode) { SlideUp(); }
    }

    void GameOver()
    {
        gameOver = true;
        SlideUp();
        gameOverScreen.SetActive(true);
        playAgainButton.gameObject.SetActive(true);
        StartCoroutine(PostGameOverVid());
    }

    IEnumerator PostGameOverVid()
    {
        Vector3 currentPos = gameHolder.transform.position;
        yield return new WaitForSeconds((float)3.5);
        cam.backgroundColor = Color.black;
        gameHolder.position = new Vector3(currentPos.x, currentPos.y, currentPos.z - 10);
    }

    void SlideUp()
    {
        Vector3 currentPos = moveController.transform.position;
        moveController.targetPos = new Vector3(currentPos.x, currentPos.y + 100, currentPos.z);
        moveController.StartMoving();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            if (!gameOver)
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit)
                {
                    draggingPiece = hit.transform;
                    offset = draggingPiece.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    offset.z = 0;
                    draggingPiece.position = new Vector3(draggingPiece.position.x, draggingPiece.position.y, -1);
                }
            }
        }

        if (draggingPiece && !gameOver)
        {
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            newPosition.z = draggingPiece.position.z;
            newPosition += offset;
            draggingPiece.position = newPosition;
        }

        if (draggingPiece && Input.GetMouseButtonUp(0))
        {
            Snap();
            offset = Vector3.back;
            draggingPiece = null;
        }
    }

    public void SaveDifficultyLevel()
    {
        PlayerPrefs.SetInt("DifficultyLevel", difficulty);
    }
}
