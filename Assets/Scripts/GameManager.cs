using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Game Elements")]
    [Range(2, 6)]
    [SerializeField] int difficulty = 4;
    [SerializeField] Transform gameHolder;
    [SerializeField] Transform piecePrefab;
    [SerializeField] int piecesCorrect = 0;

    [Header("UI Elements")]
    [SerializeField] List<Texture2D> imageTextures;
    [SerializeField] Transform levelSelectPanel;
    [SerializeField] Image levelSelectPrefab;
    [SerializeField] TMP_Text chooseYoshiText;
    [SerializeField] Button backButton;
    [SerializeField] SceneLoader sceneLoader;
    [SerializeField] GameObject winScreen;

    List<Transform> pieces;
    Transform draggingPiece = null;
    Vector2Int dimensions;
    Vector3 offset;
    float width;
    float height;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Texture2D texture in imageTextures)
        {
            Image image = Instantiate(levelSelectPrefab, levelSelectPanel);
            image.sprite = Sprite.Create(texture, new Rect(0,0, texture.width, texture.height), Vector2.zero);
            image.GetComponent<Button>().onClick.AddListener(delegate { StartGame(texture); });
        }
    }

    public void StartGame(Texture2D puzzleTexture)
    {
        levelSelectPanel.gameObject.SetActive(false);
        chooseYoshiText.gameObject.SetActive(false);
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(sceneLoader.LoadGameScene);
        pieces = new List<Transform>();
        dimensions = GetDimensions(puzzleTexture, difficulty);
        CreatePuzzlePieces(puzzleTexture);
        Scatter();
        UpdateBorder();
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
            float x = Random.Range(-orthoWidth, orthoWidth);
            float y = Random.Range(-orthoHeight, orthoHeight);
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
            if (piecesCorrect == pieces.Count) { winScreen.SetActive(true); }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
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

        if (draggingPiece)
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
}
