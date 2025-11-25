using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles placing items in the room using raycasting and spatial anchors.
/// Attach this script to a controller or hand object in your scene.
/// </summary>
public class PlaceItemInRoom : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera xrCamera;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private GameObject placementPreviewPrefab;
    
    [Header("Placement Settings")]
    [SerializeField] private float maxPlaceDistance = 3.0f;
    [SerializeField] private LayerMask realWorldMask = ~0;
    [SerializeField] private bool showPlacementPreview = true;
    
    [Header("Audio Feedback")]
    [SerializeField] private AudioClip placementSound;
    
    private GameObject currentPreview;
    private AudioSource audioSource;
    private bool isPlacementMode = true;

    private void Start()
    {
        // Setup audio source for placement feedback
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        
        // Create preview object if enabled
        if (showPlacementPreview && placementPreviewPrefab != null)
        {
            currentPreview = Instantiate(placementPreviewPrefab);
            currentPreview.SetActive(false);
        }
        
        // Auto-find camera if not assigned
        if (xrCamera == null)
        {
            xrCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (!isPlacementMode) return;
        
        // Update placement preview
        UpdatePlacementPreview();
        
        // Check for placement input
        if (GetPlacementInput())
        {
            TryPlaceAtGaze();
        }
        
        // Toggle placement mode with secondary button
        if (GetToggleInput())
        {
            TogglePlacementMode();
        }
    }

    /// <summary>
    /// Gets the placement input based on platform.
    /// Replace with your preferred input system.
    /// </summary>
    private bool GetPlacementInput()
    {
        // For Quest controllers - uncomment when using Meta XR SDK:
        // return OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger);
        
        // For testing in Editor:
        #if UNITY_EDITOR
        return Input.GetMouseButtonDown(0);
        #else
        // Production: Use OVR Input
        return false; // Replace with: OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger);
        #endif
    }

    /// <summary>
    /// Gets toggle input for placement mode.
    /// </summary>
    private bool GetToggleInput()
    {
        #if UNITY_EDITOR
        return Input.GetKeyDown(KeyCode.P);
        #else
        return false; // Replace with: OVRInput.GetDown(OVRInput.Button.Two);
        #endif
    }

    /// <summary>
    /// Updates the placement preview position based on gaze.
    /// </summary>
    private void UpdatePlacementPreview()
    {
        if (currentPreview == null) return;
        
        Vector3 origin = xrCamera.transform.position;
        Vector3 dir = xrCamera.transform.forward;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, maxPlaceDistance, realWorldMask, QueryTriggerInteraction.Ignore))
        {
            currentPreview.SetActive(true);
            currentPreview.transform.position = hit.point;
            currentPreview.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        }
        else
        {
            currentPreview.SetActive(false);
        }
    }

    /// <summary>
    /// Attempts to place an item at the current gaze position.
    /// </summary>
    private void TryPlaceAtGaze()
    {
        Vector3 origin = xrCamera.transform.position;
        Vector3 dir = xrCamera.transform.forward;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, maxPlaceDistance, realWorldMask, QueryTriggerInteraction.Ignore))
        {
            PlaceItem(hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
        }
        else
        {
            Debug.Log("No valid surface found for placement");
        }
    }

    /// <summary>
    /// Places an item at the specified position and rotation.
    /// </summary>
    private void PlaceItem(Vector3 position, Quaternion rotation)
    {
        if (itemPrefab == null)
        {
            Debug.LogError("Item prefab is not assigned!");
            return;
        }

        // Instantiate the item
        GameObject item = Instantiate(itemPrefab, position, rotation);
        
        // Add spatial anchor component if not present
        // Uncomment when using Meta Spatial Anchors SDK:
        /*
        var anchor = item.GetComponent<OVRSpatialAnchor>();
        if (anchor == null)
        {
            anchor = item.AddComponent<OVRSpatialAnchor>();
        }
        
        // Save the anchor for persistence
        StartCoroutine(SaveAnchorAsync(anchor));
        */
        
        // Play placement sound
        if (placementSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(placementSound);
        }
        
        Debug.Log($"Item placed at {position}");
    }

    /// <summary>
    /// Coroutine to save anchor asynchronously.
    /// Uncomment when using Meta Spatial Anchors SDK.
    /// </summary>
    /*
    private IEnumerator SaveAnchorAsync(OVRSpatialAnchor anchor)
    {
        // Wait for anchor to be created
        while (!anchor.Created)
        {
            yield return null;
        }
        
        // Save the anchor
        anchor.Save((anchor, success) =>
        {
            if (success)
            {
                Debug.Log($"Anchor saved successfully: {anchor.Uuid}");
            }
            else
            {
                Debug.LogError("Failed to save anchor");
            }
        });
    }
    */

    /// <summary>
    /// Toggles placement mode on/off.
    /// </summary>
    public void TogglePlacementMode()
    {
        isPlacementMode = !isPlacementMode;
        
        if (currentPreview != null)
        {
            currentPreview.SetActive(isPlacementMode);
        }
        
        Debug.Log($"Placement mode: {(isPlacementMode ? "ON" : "OFF")}");
    }

    /// <summary>
    /// Sets the item prefab to place.
    /// </summary>
    public void SetItemPrefab(GameObject prefab)
    {
        itemPrefab = prefab;
    }

    /// <summary>
    /// Gets whether placement mode is active.
    /// </summary>
    public bool IsPlacementModeActive => isPlacementMode;

    private void OnDestroy()
    {
        if (currentPreview != null)
        {
            Destroy(currentPreview);
        }
    }
}
