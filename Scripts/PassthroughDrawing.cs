using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enables drawing in passthrough mode for stylizing the environment.
/// Attach this to the controller/hand that will be used for drawing.
/// </summary>
public class PassthroughDrawing : MonoBehaviour
{
    [Header("Drawing Settings")]
    [SerializeField] private Material drawingMaterial;
    [SerializeField] private float brushSize = 0.02f;
    [SerializeField] private Color brushColor = Color.white;
    [SerializeField] private float maxDrawDistance = 2.0f;
    
    [Header("Line Settings")]
    [SerializeField] private int positionsPerLine = 1000;
    [SerializeField] private bool useWorldSpace = true;
    
    [Header("References")]
    [SerializeField] private Transform drawingPoint;
    [SerializeField] private Camera xrCamera;
    
    [Header("Audio Feedback")]
    [SerializeField] private AudioClip drawStartSound;
    [SerializeField] private AudioClip drawEndSound;
    
    private List<GameObject> drawnLines = new List<GameObject>();
    private Dictionary<Color, Material> materialCache = new Dictionary<Color, Material>();
    private LineRenderer currentLine;
    private int currentPositionCount;
    private bool isDrawing = false;
    private Vector3 lastDrawPosition;
    private float minDrawDistance = 0.005f;
    private AudioSource audioSource;

    private void Start()
    {
        // Setup audio
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        
        // Auto-find camera if not assigned
        if (xrCamera == null)
        {
            xrCamera = Camera.main;
        }
        
        // Use this transform as drawing point if not specified
        if (drawingPoint == null)
        {
            drawingPoint = transform;
        }
        
        // Create default material if not assigned
        if (drawingMaterial == null)
        {
            drawingMaterial = CreateDefaultDrawingMaterial();
        }
    }

    private void Update()
    {
        // Check for drawing input
        if (GetDrawInput())
        {
            if (!isDrawing)
            {
                StartDrawing();
            }
            else
            {
                ContinueDrawing();
            }
        }
        else if (isDrawing)
        {
            StopDrawing();
        }
        
        // Clear all drawings with secondary input
        if (GetClearInput())
        {
            ClearAllDrawings();
        }
        
        // Undo last line
        if (GetUndoInput())
        {
            UndoLastLine();
        }
    }

    /// <summary>
    /// Gets the draw input based on platform.
    /// </summary>
    private bool GetDrawInput()
    {
        // For Quest controllers - uncomment when using Meta XR SDK:
        // return OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger);
        
        #if UNITY_EDITOR
        return Input.GetMouseButton(1); // Right mouse button for testing
        #else
        return false; // Replace with: OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger);
        #endif
    }

    /// <summary>
    /// Gets the clear input.
    /// </summary>
    private bool GetClearInput()
    {
        #if UNITY_EDITOR
        return Input.GetKeyDown(KeyCode.C);
        #else
        return false; // Replace with appropriate controller input
        #endif
    }

    /// <summary>
    /// Gets the undo input.
    /// </summary>
    private bool GetUndoInput()
    {
        #if UNITY_EDITOR
        return Input.GetKeyDown(KeyCode.Z);
        #else
        return false; // Replace with appropriate controller input
        #endif
    }

    /// <summary>
    /// Starts a new drawing stroke.
    /// </summary>
    private void StartDrawing()
    {
        isDrawing = true;
        currentPositionCount = 0;
        
        // Create new line object
        GameObject lineObj = new GameObject("DrawnLine_" + drawnLines.Count);
        currentLine = lineObj.AddComponent<LineRenderer>();
        
        // Configure line renderer with cached material to reduce allocations
        currentLine.material = GetOrCreateMaterial(brushColor);
        currentLine.startWidth = brushSize;
        currentLine.endWidth = brushSize;
        currentLine.useWorldSpace = useWorldSpace;
        currentLine.positionCount = 0;
        
        // Set line rendering quality
        currentLine.numCapVertices = 5;
        currentLine.numCornerVertices = 5;
        
        // Store last position
        lastDrawPosition = GetDrawPosition();
        
        // Add first point
        AddPointToLine(lastDrawPosition);
        
        drawnLines.Add(lineObj);
        
        // Play sound
        if (drawStartSound != null)
        {
            audioSource.PlayOneShot(drawStartSound);
        }
        
        Debug.Log("Started drawing");
    }

    /// <summary>
    /// Gets a cached material for the given color, or creates one if not cached.
    /// This reduces memory allocations when drawing many lines with the same color.
    /// </summary>
    private Material GetOrCreateMaterial(Color color)
    {
        if (materialCache.TryGetValue(color, out Material cachedMaterial))
        {
            return cachedMaterial;
        }
        
        Material newMaterial = new Material(drawingMaterial);
        newMaterial.color = color;
        materialCache[color] = newMaterial;
        return newMaterial;
    }

    /// <summary>
    /// Continues the current drawing stroke.
    /// </summary>
    private void ContinueDrawing()
    {
        if (currentLine == null) return;
        
        Vector3 currentPosition = GetDrawPosition();
        
        // Only add point if we've moved enough
        if (Vector3.Distance(currentPosition, lastDrawPosition) >= minDrawDistance)
        {
            AddPointToLine(currentPosition);
            lastDrawPosition = currentPosition;
        }
    }

    /// <summary>
    /// Stops the current drawing stroke.
    /// </summary>
    private void StopDrawing()
    {
        isDrawing = false;
        currentLine = null;
        
        // Play sound
        if (drawEndSound != null)
        {
            audioSource.PlayOneShot(drawEndSound);
        }
        
        Debug.Log("Stopped drawing");
    }

    /// <summary>
    /// Gets the current drawing position from raycast or controller.
    /// </summary>
    private Vector3 GetDrawPosition()
    {
        // Raycast from drawing point to find surface
        Vector3 origin = drawingPoint.position;
        Vector3 direction = drawingPoint.forward;
        
        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDrawDistance))
        {
            // Draw slightly in front of the surface to prevent z-fighting
            return hit.point + hit.normal * 0.001f;
        }
        
        // If no surface hit, draw in air at max distance
        return origin + direction * maxDrawDistance;
    }

    /// <summary>
    /// Adds a point to the current line.
    /// </summary>
    private void AddPointToLine(Vector3 position)
    {
        if (currentLine == null) return;
        if (currentPositionCount >= positionsPerLine) return;
        
        currentPositionCount++;
        currentLine.positionCount = currentPositionCount;
        currentLine.SetPosition(currentPositionCount - 1, position);
    }

    /// <summary>
    /// Clears all drawn lines.
    /// </summary>
    public void ClearAllDrawings()
    {
        foreach (var line in drawnLines)
        {
            if (line != null)
            {
                Destroy(line);
            }
        }
        drawnLines.Clear();
        currentLine = null;
        isDrawing = false;
        
        Debug.Log("Cleared all drawings");
    }

    /// <summary>
    /// Removes the last drawn line.
    /// </summary>
    public void UndoLastLine()
    {
        if (drawnLines.Count == 0) return;
        
        int lastIndex = drawnLines.Count - 1;
        GameObject lastLine = drawnLines[lastIndex];
        
        if (lastLine != null)
        {
            Destroy(lastLine);
        }
        
        drawnLines.RemoveAt(lastIndex);
        
        // If we were drawing, stop
        if (currentLine != null && currentLine.gameObject == lastLine)
        {
            currentLine = null;
            isDrawing = false;
        }
        
        Debug.Log("Undid last line");
    }

    /// <summary>
    /// Sets the brush color.
    /// </summary>
    public void SetBrushColor(Color color)
    {
        brushColor = color;
    }

    /// <summary>
    /// Sets the brush size.
    /// </summary>
    public void SetBrushSize(float size)
    {
        brushSize = Mathf.Clamp(size, 0.001f, 0.1f);
    }

    /// <summary>
    /// Gets the count of drawn lines.
    /// </summary>
    public int GetLineCount()
    {
        return drawnLines.Count;
    }

    /// <summary>
    /// Creates a default unlit material for drawing.
    /// </summary>
    private Material CreateDefaultDrawingMaterial()
    {
        // Use an unlit shader that works well in passthrough
        Shader shader = Shader.Find("Unlit/Color");
        if (shader == null)
        {
            shader = Shader.Find("Standard");
        }
        
        Material mat = new Material(shader);
        mat.color = brushColor;
        return mat;
    }

    /// <summary>
    /// Saves all drawings as anchored objects (for persistence).
    /// </summary>
    public void SaveDrawingsAsAnchors()
    {
        // Placeholder for saving drawings with spatial anchors
        // This would integrate with AnchorManager for persistence
        Debug.Log($"Saving {drawnLines.Count} drawings as anchors...");
        
        // Implementation would create spatial anchors for each line's origin
        // and store the line data for reconstruction
    }

    private void OnDestroy()
    {
        ClearAllDrawings();
        
        // Clean up cached materials
        foreach (var material in materialCache.Values)
        {
            if (material != null)
            {
                Destroy(material);
            }
        }
        materialCache.Clear();
    }
}
