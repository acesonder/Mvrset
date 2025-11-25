# Unity Project Setup Guide

This guide walks you through creating a Unity project for Meta Quest VR with passthrough and spatial anchors.

## Step 1 — Create the Project

1. Open Unity Hub
2. Create a new project using **Unity 2022.3 LTS**
3. Select the **3D (URP)** template (recommended for Quest performance)
4. Name your project and create it

## Step 2 — Import Meta XR SDKs

### Option A: Asset Store
1. Open the Asset Store in Unity (Window > Asset Store)
2. Search for and import:
   - **Meta XR Core SDK** - Required for passthrough and XR features
   - **Meta Spatial Anchors** - Required for anchor functionality
   - **Meta XR Interaction SDK** (Optional) - For hand/controller interactions

### Option B: OpenUPM
Add packages via Package Manager or manifest.json:
```json
{
  "dependencies": {
    "com.meta.xr.sdk.core": "version",
    "com.meta.xr.sdk.interaction": "version"
  }
}
```

## Step 3 — Configure Platform Settings

1. Go to **File > Build Settings**
2. Select **Android** platform
3. Click **Switch Platform**

## Step 4 — Configure XR Settings

1. Go to **Edit > Project Settings > XR Plug-in Management**
2. Select the **Android** tab
3. Enable **Oculus** or **OpenXR** provider
4. Check **Initialize XR on Startup**

### OpenXR Configuration (Recommended)
If using OpenXR:
1. Go to **Project Settings > XR Plug-in Management > OpenXR**
2. Add **Meta Quest Support** feature
3. Enable **Meta XR Feature** under Features

## Step 5 — Configure Player Settings

1. Go to **Edit > Project Settings > Player**
2. Select the **Android** tab
3. Configure the following:

### Other Settings
- **Color Space**: Linear (recommended)
- **Auto Graphics API**: Disabled
- **Graphics APIs**: OpenGLES3 or Vulkan
- **Scripting Backend**: IL2CPP (required for Quest)
- **Target Architectures**: ARM64

### Publishing Settings
Add required permissions by creating `AndroidManifest.xml` or using Meta's automatic manifest settings:
```xml
<uses-permission android:name="com.oculus.permission.USE_ANCHORS" />
<uses-permission android:name="android.permission.CAMERA" />
```

## Step 6 — Scene Setup

1. Delete the default Main Camera
2. Add the **OVRCameraRig** or **Meta XR Rig** to your scene
3. Add **OVRAnchorManager** to your scene (for spatial anchors)
4. Configure the rig:
   - Enable **Passthrough** settings
   - Configure tracking origin type

## Next Steps

- [Configure Passthrough](PASSTHROUGH.md)
- [Set up Spatial Anchors](SPATIAL_ANCHORS.md)
