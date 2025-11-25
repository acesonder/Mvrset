using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages spatial anchor persistence - loading and saving anchored items.
/// Add this to your scene to automatically load saved anchors on startup.
/// </summary>
public class AnchorManager : MonoBehaviour
{
    [Header("Prefab Mapping")]
    [SerializeField] private List<AnchorPrefabMapping> prefabMappings = new List<AnchorPrefabMapping>();
    [SerializeField] private GameObject defaultPrefab;
    
    [Header("Settings")]
    [SerializeField] private bool loadAnchorsOnStart = true;
    [SerializeField] private int maxAnchors = 50;
    
    [Header("Events")]
    [SerializeField] private UnityEngine.Events.UnityEvent onAnchorsLoaded;
    [SerializeField] private UnityEngine.Events.UnityEvent<int> onAnchorCountChanged;
    
    private Dictionary<string, GameObject> spawnedItems = new Dictionary<string, GameObject>();
    private int currentAnchorCount = 0;

    [System.Serializable]
    public class AnchorPrefabMapping
    {
        public string prefabId;
        public GameObject prefab;
    }

    private void Start()
    {
        if (loadAnchorsOnStart)
        {
            LoadAllAnchors();
        }
    }

    /// <summary>
    /// Loads all saved spatial anchors and spawns their associated items.
    /// Call this to restore the user's room setup.
    /// </summary>
    public void LoadAllAnchors()
    {
        StartCoroutine(LoadAnchorsCoroutine());
    }

    /// <summary>
    /// Coroutine that loads anchors asynchronously.
    /// Uncomment OVR-specific code when using Meta Spatial Anchors SDK.
    /// </summary>
    private IEnumerator LoadAnchorsCoroutine()
    {
        Debug.Log("Loading saved anchors...");
        
        // Placeholder for Meta SDK anchor loading
        // Uncomment when using Meta Spatial Anchors SDK:
        /*
        // Query all saved anchors
        var options = new OVRAnchor.FetchOptions
        {
            SingleComponentType = typeof(OVRStorable)
        };
        
        var task = OVRAnchor.FetchAnchorsAsync(new List<OVRAnchor>(), options);
        yield return new WaitUntil(() => task.IsCompleted);
        
        if (task.GetResult().Success)
        {
            var anchors = task.GetResult().Value;
            foreach (var anchor in anchors)
            {
                yield return LocalizeAndSpawnAnchor(anchor);
            }
        }
        */
        
        yield return null;
        
        Debug.Log($"Loaded {currentAnchorCount} anchors");
        onAnchorsLoaded?.Invoke();
    }

    /// <summary>
    /// Localizes an anchor and spawns the associated item.
    /// Uncomment when using Meta Spatial Anchors SDK.
    /// </summary>
    /*
    private IEnumerator LocalizeAndSpawnAnchor(OVRAnchor anchor)
    {
        // Enable localization
        if (anchor.TryGetComponent<OVRLocatable>(out var locatable))
        {
            var locTask = locatable.SetEnabledAsync(true);
            yield return new WaitUntil(() => locTask.IsCompleted);
            
            if (locTask.GetResult().Success)
            {
                // Get the pose
                if (locatable.TryGetSceneAnchorPose(out var pose))
                {
                    // Determine which prefab to spawn
                    string prefabId = GetPrefabIdFromAnchor(anchor);
                    GameObject prefab = GetPrefabById(prefabId);
                    
                    // Spawn the item
                    var item = Instantiate(prefab, pose.position, pose.rotation);
                    spawnedItems[anchor.Uuid.ToString()] = item;
                    
                    currentAnchorCount++;
                    onAnchorCountChanged?.Invoke(currentAnchorCount);
                }
            }
        }
    }
    */

    /// <summary>
    /// Gets the prefab for a given ID.
    /// </summary>
    private GameObject GetPrefabById(string prefabId)
    {
        foreach (var mapping in prefabMappings)
        {
            if (mapping.prefabId == prefabId)
            {
                return mapping.prefab;
            }
        }
        return defaultPrefab;
    }

    /// <summary>
    /// Saves a new anchor with the associated prefab ID.
    /// </summary>
    public void SaveAnchor(GameObject item, string prefabId)
    {
        if (currentAnchorCount >= maxAnchors)
        {
            Debug.LogWarning($"Maximum anchor count ({maxAnchors}) reached!");
            return;
        }
        
        StartCoroutine(SaveAnchorCoroutine(item, prefabId));
    }

    private IEnumerator SaveAnchorCoroutine(GameObject item, string prefabId)
    {
        // Placeholder for Meta SDK anchor saving
        // Uncomment when using Meta Spatial Anchors SDK:
        /*
        var anchor = item.GetComponent<OVRSpatialAnchor>();
        if (anchor == null)
        {
            anchor = item.AddComponent<OVRSpatialAnchor>();
        }
        
        // Wait for anchor creation
        while (!anchor.Created)
        {
            yield return null;
        }
        
        // Save the anchor
        bool saved = false;
        anchor.Save((a, success) => { saved = success; });
        
        yield return new WaitUntil(() => saved);
        
        if (saved)
        {
            spawnedItems[anchor.Uuid.ToString()] = item;
            currentAnchorCount++;
            onAnchorCountChanged?.Invoke(currentAnchorCount);
            Debug.Log($"Anchor saved: {anchor.Uuid}");
        }
        */
        
        yield return null;
        Debug.Log($"Anchor saved for prefab: {prefabId}");
    }

    /// <summary>
    /// Deletes an anchor and removes the associated item.
    /// </summary>
    public void DeleteAnchor(string anchorId)
    {
        StartCoroutine(DeleteAnchorCoroutine(anchorId));
    }

    private IEnumerator DeleteAnchorCoroutine(string anchorId)
    {
        if (spawnedItems.TryGetValue(anchorId, out GameObject item))
        {
            // Placeholder for Meta SDK anchor deletion
            // Uncomment when using Meta Spatial Anchors SDK:
            /*
            var anchor = item.GetComponent<OVRSpatialAnchor>();
            if (anchor != null)
            {
                var task = anchor.EraseAsync();
                yield return new WaitUntil(() => task.IsCompleted);
            }
            */
            
            Destroy(item);
            spawnedItems.Remove(anchorId);
            currentAnchorCount--;
            onAnchorCountChanged?.Invoke(currentAnchorCount);
        }
        
        yield return null;
    }

    /// <summary>
    /// Clears all anchors and removes all placed items.
    /// </summary>
    public void ClearAllAnchors()
    {
        StartCoroutine(ClearAllAnchorsCoroutine());
    }

    private IEnumerator ClearAllAnchorsCoroutine()
    {
        var anchorsToDelete = new List<string>(spawnedItems.Keys);
        
        foreach (var anchorId in anchorsToDelete)
        {
            yield return DeleteAnchorCoroutine(anchorId);
        }
        
        Debug.Log("All anchors cleared");
    }

    /// <summary>
    /// Gets the current count of saved anchors.
    /// </summary>
    public int GetAnchorCount()
    {
        return currentAnchorCount;
    }

    /// <summary>
    /// Checks if we can save more anchors.
    /// </summary>
    public bool CanSaveMoreAnchors()
    {
        return currentAnchorCount < maxAnchors;
    }
}
