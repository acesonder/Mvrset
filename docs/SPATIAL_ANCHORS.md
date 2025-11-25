# Spatial Anchors Guide

This guide explains how to use Meta Spatial Anchors to place and persist virtual items in your real-world environment.

## What are Spatial Anchors?

Spatial Anchors are fixed points in real-world space that allow virtual objects to:
- Stay in the same physical location across sessions
- Persist even after the app is closed
- Maintain accurate positioning relative to your room

## Setting Up Spatial Anchors

### Step 1: Add OVRAnchorManager

1. Create an empty GameObject in your scene
2. Add the **OVRAnchorManager** component
3. This manager handles anchor creation, saving, and loading

### Step 2: Add Permissions

Ensure your AndroidManifest.xml includes:
```xml
<uses-permission android:name="com.oculus.permission.USE_ANCHORS" />
```

### Step 3: Create Placeable Prefabs

1. Create your item (furniture, lamp, decoration)
2. Add the **OVRAnchor** component to the root
3. Save as a prefab

## Creating Anchors via Script

See [PlaceItemInRoom.cs](../Scripts/PlaceItemInRoom.cs) for a complete implementation.

Basic anchor creation:
```csharp
using UnityEngine;
using Meta.XR.MRUtilityKit;

public class AnchorCreator : MonoBehaviour
{
    public async void CreateAnchorAtPosition(Vector3 position, Quaternion rotation)
    {
        // Create a new spatial anchor
        var anchorResult = await OVRAnchor.CreateSpatialAnchorAsync(
            new Pose(position, rotation)
        );
        
        if (anchorResult.Success)
        {
            var anchor = anchorResult.Value;
            Debug.Log($"Anchor created: {anchor.Uuid}");
            
            // Save the anchor for persistence
            await anchor.SaveAsync();
        }
    }
}
```

## Saving and Loading Anchors

### Saving Anchors

Anchors are automatically persisted when you call `Save()` or `SaveAsync()`:

```csharp
// Save a single anchor
await anchor.SaveAsync();

// Save with local persistence
anchor.Save(); // Synchronous version
```

### Loading Saved Anchors

Use [AnchorManager.cs](../Scripts/AnchorManager.cs) for loading anchors on startup:

```csharp
using UnityEngine;
using System.Collections.Generic;

public class AnchorLoader : MonoBehaviour
{
    public GameObject itemPrefab;
    
    async void Start()
    {
        // Query all saved anchors
        var anchors = new List<OVRAnchor>();
        var result = await OVRAnchor.FetchAnchorsAsync(
            anchors,
            new OVRAnchor.FetchOptions
            {
                SingleComponentType = typeof(OVRStorable)
            }
        );
        
        if (result.Success)
        {
            foreach (var anchor in anchors)
            {
                // Localize the anchor to get its position
                if (anchor.TryGetComponent<OVRLocatable>(out var locatable))
                {
                    await locatable.SetEnabledAsync(true);
                    
                    if (locatable.TryGetSceneAnchorPose(out var pose))
                    {
                        // Spawn item at anchor position
                        Instantiate(itemPrefab, pose.position, pose.rotation);
                    }
                }
            }
        }
    }
}
```

## Anchor Lifecycle

### Creation
```
User triggers placement -> Raycast to surface -> Create anchor -> Save anchor
```

### Loading
```
App starts -> Query saved anchors -> Localize anchors -> Spawn items at positions
```

### Deletion
```csharp
// Erase an anchor
await anchor.EraseAsync();
```

## Best Practices

### 1. Handle Tracking Loss
```csharp
void Update()
{
    if (OVRPlugin.GetNodePositionTracked(OVRPlugin.Node.Head) == false)
    {
        // Tracking lost - hide or disable anchor-based content
        SetAnchoredContentVisible(false);
    }
}
```

### 2. Limit Anchor Count
- Quest devices have a limit on saved anchors
- Delete unused anchors to free space
- Consider a maximum limit in your app

### 3. Provide Visual Feedback
```csharp
// Show placement preview before confirming
void ShowPlacementPreview(Vector3 position)
{
    previewObject.SetActive(true);
    previewObject.transform.position = position;
}
```

### 4. Handle Failures Gracefully
```csharp
var result = await anchor.SaveAsync();
if (!result.Success)
{
    Debug.LogError($"Failed to save anchor: {result.Status}");
    // Show user-friendly message
    ShowMessage("Could not save item position. Please try again.");
}
```

## Troubleshooting

### Anchors Not Persisting
1. Verify USE_ANCHORS permission is granted
2. Check that `Save()` is being called
3. Ensure OVRAnchorManager is in the scene

### Anchors in Wrong Position
1. Wait for localization to complete before spawning items
2. Check that tracking is stable
3. Verify the device has been recently recalibrated

### Cannot Create Anchors
1. Check anchor count limit
2. Verify permissions
3. Ensure proper tracking (walk around the room first)

## Advanced: Cloud Anchors

For cross-device anchor sharing, you would need Meta's Cloud Anchor APIs:

```csharp
// Share anchor to cloud (requires additional setup)
var shareResult = await anchor.ShareAsync(new OVRAnchor.ShareOptions
{
    // Cloud sharing options
});
```

Note: Cloud anchors require additional backend setup and are beyond the scope of this basic guide.

## Next Steps

- Implement [PlaceItemInRoom.cs](../Scripts/PlaceItemInRoom.cs) for item placement
- Add [AnchorManager.cs](../Scripts/AnchorManager.cs) for persistence
- Implement [PassthroughDrawing.cs](../Scripts/PassthroughDrawing.cs) for drawing features
