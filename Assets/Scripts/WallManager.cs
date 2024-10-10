using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum DrawingStage
{
    None,
    Drawing,
    CompleteStage,
    CompletingStage,
    CompletedStage,
    CompletedDrawing
}

public class WallManager : MonoBehaviour
{
    [SerializeField] private float sprayRadius;
    [SerializeField] private float completeDrawingThreshold = 0.7f;
    [SerializeField] private GameObject drawingParentObject;
    [SerializeField] public GameObject drawingObject;
    [SerializeField] private Toggle[] toggles;
    [SerializeField] private static DrawingStages[] drawingsTextures = null;
    [SerializeField] private Material fadedDrawingMaterial;
    [SerializeField] private Material drawingMaterial;
    [SerializeField] private MeshFilter drawingQuad;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private int maxPixelsToDrawPerFrame = 15000;
    [SerializeField] private UnityEvent onDrawingCompleted;

    private GameObject fadedDrawingObject;
    private Texture2D drawingTexture;
    private int currentSelectedDrawing;
    private int currentStage;
    private int totalPixelsToDraw;
    private int totalDrawnPixels;
    public DrawingStage drawingStage;
    private bool isPlayerNearWall;
    private MeshRenderer fadedDrawingMeshRenderer;
    private MeshRenderer drawingMeshRenderer;
    private SprayCanHands sprayCan;

    private int accumulatedScore = 0; // Track the score accumulated for the current drawing

    private void Awake()
    {
        Application.targetFrameRate = 60;
		if(drawingsTextures != null)
        {
            enabled = true;
        }

	}

    private void Start()
    {

        InitializeDrawing();
        InitializeFadedDrawingObject();
        SetupToggles();
    }

    private void InitializeDrawing()
    {
        drawingStage = DrawingStage.None;
        drawingMeshRenderer = drawingObject.GetComponent<MeshRenderer>();
        drawingMeshRenderer.material = drawingMaterial;
    }

    void Update()
    {
        if (drawingStage.Equals(DrawingStage.CompleteStage))
        {
            StartCoroutine(CompleteDrawing());
        }
        else if (drawingStage.Equals(DrawingStage.CompletedStage))
        {
            if (sprayCan != null && sprayCan.isTriggering)
            {
                drawingStage = DrawingStage.None;
            }
        }
        else if (drawingStage.Equals(DrawingStage.None) || drawingStage.Equals(DrawingStage.Drawing))
        {
            if (sprayCan != null && sprayCan.isTriggering && isPlayerNearWall)
            {
                sprayCan.DetectSpray();
            }
        }
    }

    public void DrawAtPosition(SprayCanHands sprayCan, Vector2 uv)
    {
        this.sprayCan = sprayCan;
        Vector2 pixelUV = GetPixelUV(uv);
        DrawSpray(pixelUV);
    }

    private void DrawSpray(Vector2 center)
    {
        drawingStage = DrawingStage.Drawing;
        int xCenter = (int)center.x;
        int yCenter = (int)center.y;
        int radius = (int)(sprayRadius * drawingTexture.width / drawingQuad.mesh.bounds.size.x);

        for (int y = yCenter - radius; y <= yCenter + radius; y++)
        {
            for (int x = xCenter - radius; x <= xCenter + radius; x++)
            {
                if (x >= 0 && x < drawingTexture.width && y >= 0 && y < drawingTexture.height)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    if (distance <= radius)
                    {
                        Color newColor = drawingsTextures[currentSelectedDrawing].stages[currentStage].GetPixel(x, y);
                        Color oldColor = drawingTexture.GetPixel(x, y);
                        if (newColor.a > 0 && newColor != oldColor)
                        {
                            totalDrawnPixels++;
                            drawingTexture.SetPixel(x, y, newColor);
                        }
                    }
                }
            }
        }

        drawingTexture.Apply();
        CheckDrawingThreshold();
    }

    private IEnumerator CompleteDrawing()
    {
        drawingStage = DrawingStage.CompletingStage;

        var pixelsCount = 0;
        for (int y = 0; y < drawingTexture.height; y++)
        {
            for (int x = 0; x < drawingTexture.width; x++)
            {
                Color newColor = drawingsTextures[currentSelectedDrawing].stages[currentStage].GetPixel(x, y);
                Color oldColor = drawingTexture.GetPixel(x, y);
                if (newColor.a > 0 && newColor != oldColor)
                {
                    pixelsCount++;
                    drawingTexture.SetPixel(x, y, newColor);
                }
                if (pixelsCount > maxPixelsToDrawPerFrame)
                {
                    yield return null;
                    pixelsCount = 0;
                }
            }
        }

        drawingTexture.Apply();
        currentStage++;

        if (currentStage >= drawingsTextures[currentSelectedDrawing].stages.Length)
        {
            drawingStage = DrawingStage.CompletedDrawing;
            DisableFadedDrawing();
            onDrawingCompleted?.Invoke();

		}
        else
        {
            UpdateFadedDrawing(currentSelectedDrawing, currentStage);
            drawingStage = DrawingStage.CompletedStage;
        }

        // Adjust the score based on the stage completed
        int scoreToAdd = CalculateScoreForStage(currentStage);
        accumulatedScore += scoreToAdd;

        // Report the score to the global ScoreManager
        ScoreManager.Instance.AddScore(scoreToAdd);
    }

    private int CalculateScoreForStage(int stage)
    {
        // Example scoring logic:
        // Stage 1 completion = 100 points
        // Stage 2 completion = 75 points
        // Stage 3 completion = 50 points
        // Customize these values as per your game requirements
        switch (stage)
        {
            case 1: return 100;
            case 2: return 75;
            case 3: return 50;
            default: return 25; // Score for any other stages (if applicable)
        }
    }

    private void CheckDrawingThreshold()
    {
        var percentage = (float)totalDrawnPixels / totalPixelsToDraw;

        if (percentage >= completeDrawingThreshold)
        {
            drawingStage = DrawingStage.CompleteStage;
        }
    }

    private void DisableFadedDrawing()
    {
        fadedDrawingObject.SetActive(false);
    }

    Vector2 GetPixelUV(Vector2 uv)
    {
        int x = Mathf.FloorToInt(uv.x * drawingTexture.width);
        int y = Mathf.FloorToInt(uv.y * drawingTexture.height);
        return new Vector2(x, y);
    }

    Texture2D GenerateTexture2D(Texture2D texture2D)
    {
        Texture2D tex = new Texture2D(texture2D.width, texture2D.height, TextureFormat.ARGB32, false);
        Color[] baseColors = new Color[tex.width * tex.height];
        for (int i = 0; i < baseColors.Length; i++)
        {
            baseColors[i] = new Color(0, 0, 0, 0); // Transparent base
        }
        tex.SetPixels(baseColors);
        tex.Apply();
        return tex;
    }

    private void SetupToggles()
    {
        for (int i = 0; i < toggles.Length; i++)
        {
            var toggleIndex = i;
            toggles[i].onValueChanged.AddListener(delegate (bool isOn) { OnToggleSelected(isOn, toggleIndex); });
            if (toggles[i].isOn)
            {
                UpdateFadedDrawing(toggleIndex, 0); // Start with the first stage
            }
        }
    }

    private void OnToggleSelected(bool isOn, int toggleIndex)
    {
        if (isOn && !IsDrawingCompleted() && isPlayerNearWall)
        {
            // Reset the drawing state and remove accumulated score if switching
            ResetDrawingState(true);

            UpdateFadedDrawing(toggleIndex, 0); // Start with the first stage
        }
    }

    private void UpdateFadedDrawing(int index, int stage)
    {
        currentSelectedDrawing = index;
        currentStage = stage;
        totalDrawnPixels = 0;

        if (stage == 0)
        {
            drawingTexture = GenerateTexture2D(drawingsTextures[index].stages[stage]);
        }

        SetTotalPixelsToDraw(stage);
        drawingMeshRenderer.material.SetTexture("_MainTex", drawingTexture);

        // Update the faded drawing object
        fadedDrawingMeshRenderer.material.SetTexture("_MainTex", drawingsTextures[index].stages[stage]);
    }

    private void SetTotalPixelsToDraw(int stage)
    {
        totalPixelsToDraw = 0;
        for (int y = 0; y < drawingsTextures[currentSelectedDrawing].stages[stage].height; y++)
        {
            for (int x = 0; x < drawingsTextures[currentSelectedDrawing].stages[stage].width; x++)
            {
                var newColor = drawingsTextures[currentSelectedDrawing].stages[stage].GetPixel(x, y);
                var oldColor = drawingTexture.GetPixel(x, y);
                if (newColor.a > 0 && newColor != oldColor)
                {
                    totalPixelsToDraw++;
                }
            }
        }
    }

    private void InitializeFadedDrawingObject()
    {
        var drawingQuadRenderer = drawingQuad.GetComponent<Renderer>();
        // Create a new GameObject for the faded drawing
        fadedDrawingObject = new GameObject("FadedDrawingObject");
        fadedDrawingMeshRenderer = fadedDrawingObject.AddComponent<MeshRenderer>();
        fadedDrawingMeshRenderer.material = fadedDrawingMaterial;

        var fadedFilter = fadedDrawingObject.AddComponent<MeshFilter>();
        fadedFilter.mesh = drawingQuad.mesh;

        // Position it correctly
        fadedDrawingObject.transform.SetParent(drawingQuadRenderer.transform);
        fadedDrawingObject.transform.localPosition = Vector3.zero;
        fadedDrawingObject.transform.localRotation = Quaternion.identity;
        fadedDrawingObject.transform.localScale = Vector3.one;

        // Ensure the faded drawing object is rendered on top
        fadedDrawingMeshRenderer.sortingOrder = 1;
        drawingQuadRenderer.sortingOrder = 0;

        // Initially hide the faded drawing object
        fadedDrawingObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (enabled && other.CompareTag("Player"))
        {
            drawingParentObject.SetActive(true);
            if (!IsDrawingCompleted())
            {
                fadedDrawingObject.SetActive(true);
            }
            isPlayerNearWall = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (enabled && other.CompareTag("Player"))
        {
            fadedDrawingObject.SetActive(false);
            isPlayerNearWall = false;
            if (!IsDrawingCompleted())
            {
                drawingParentObject.SetActive(true); // Keep the drawing visible
            }
        }
    }

    private bool IsDrawingCompleted()
    {
        return drawingStage.Equals(DrawingStage.CompletedDrawing);
    }

    private void ResetDrawingState(bool removeAccumulatedScore = false)
    {
        if (removeAccumulatedScore && accumulatedScore > 0)
        {
            // Deduct the accumulated score from the global ScoreManager
            ScoreManager.Instance.AddScore(-accumulatedScore);
            accumulatedScore = 0;
        }
        currentStage = 0;
        totalDrawnPixels = 0;
        drawingStage = DrawingStage.None;
    }

    internal void SetWallManager(Texture2D[] leftDrawing, Texture2D[] rightDrawing)
    {
        drawingsTextures = new DrawingStages[]
        {
            new DrawingStages(leftDrawing),
            new DrawingStages(rightDrawing)
        };
    }
}

[System.Serializable]
public class DrawingStages
{
    public Texture2D[] stages;

    public DrawingStages(Texture2D[] stages)
    {
        this.stages = stages;
    }
}
