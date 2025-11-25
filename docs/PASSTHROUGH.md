# Passthrough Configuration Guide

This guide explains how to enable and configure Meta Passthrough for your VR experience.

## What is Passthrough?

Passthrough allows users to see their real-world environment through the Quest headset cameras, creating a mixed reality experience where virtual content overlays the real world.

## Enabling Passthrough

### Method 1: OVR Manager (Recommended)

1. Select the **OVRCameraRig** in your scene
2. Find the **OVR Manager** component
3. Locate the **Passthrough** section
4. Enable these settings:
   - **Insight Passthrough > Enabled**: ✓
   - **Enable Passthrough On Startup**: ✓

### Method 2: Via Script

```csharp
using UnityEngine;

public class PassthroughSetup : MonoBehaviour
{
    private OVRPassthroughLayer passthroughLayer;

    void Start()
    {
        // Enable passthrough on the main camera
        var cameraRig = FindObjectOfType<OVRCameraRig>();
        if (cameraRig != null)
        {
            // Add passthrough layer if not present
            passthroughLayer = cameraRig.gameObject.GetComponent<OVRPassthroughLayer>();
            if (passthroughLayer == null)
            {
                passthroughLayer = cameraRig.gameObject.AddComponent<OVRPassthroughLayer>();
            }
            
            passthroughLayer.enabled = true;
        }
    }
}
```

## Passthrough Modes

### Underlay Mode (Recommended for Home Experience)
The passthrough appears behind all virtual content:
```csharp
passthroughLayer.overlayType = OVROverlay.OverlayType.Underlay;
```

### Overlay Mode
The passthrough appears in front of virtual content (useful for specific effects):
```csharp
passthroughLayer.overlayType = OVROverlay.OverlayType.Overlay;
```

## Camera Background Configuration

For passthrough to be visible, the camera background must be transparent:

1. Select the **CenterEyeAnchor** camera in OVRCameraRig
2. Set **Clear Flags** to **Solid Color**
3. Set **Background** color to **Black with Alpha = 0** (RGBA: 0, 0, 0, 0)

Or via script:
```csharp
Camera.main.clearFlags = CameraClearFlags.SolidColor;
Camera.main.backgroundColor = new Color(0, 0, 0, 0);
```

## Passthrough Quality Settings

Configure quality in OVR Manager or via script:

```csharp
// Set passthrough quality
OVRManager.instance.SetPassthroughEyeFOVDecrease(0f); // 0 = highest quality

// Configure color LUT for stylization (optional)
passthroughLayer.SetColorLut(colorLutTexture);
```

## Styling Passthrough

You can apply effects to the passthrough view:

### Color Adjustments
```csharp
// Adjust brightness
passthroughLayer.SetBrightness(1.2f);

// Adjust contrast
passthroughLayer.SetContrast(1.1f);

// Adjust saturation
passthroughLayer.SetSaturation(0.8f);
```

### Edge Rendering
```csharp
// Enable edge rendering for stylized look
passthroughLayer.edgeRendering = true;
passthroughLayer.edgeColor = Color.white;
```

## Troubleshooting

### Passthrough Not Visible
1. Ensure camera background is transparent
2. Check that passthrough is enabled in OVR Manager
3. Verify the app has camera permissions

### Black Screen on Device
1. Check that OpenXR or Oculus provider is properly configured
2. Ensure the device has latest firmware
3. Verify passthrough permissions in app settings

### Performance Issues
1. Use Underlay mode instead of Overlay
2. Reduce passthrough quality if needed
3. Enable dynamic foveation in OVR Manager

## Best Practices

1. **Always use Underlay mode** for home-like experiences
2. **Keep virtual content simple** to maintain performance
3. **Test on device regularly** - passthrough behaves differently in Editor
4. **Consider lighting** - virtual objects should blend with real environment

## Next Steps

- [Set up Spatial Anchors](SPATIAL_ANCHORS.md)
- [Add placeable items](../Scripts/PlaceItemInRoom.cs)
