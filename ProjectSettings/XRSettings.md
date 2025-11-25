# XR Settings Reference

This document provides a reference for Unity XR settings required for Meta Quest development.

## XR Plug-in Management Settings

### Android Tab

| Setting | Value | Notes |
|---------|-------|-------|
| Initialize XR on Startup | ✓ Enabled | Required for automatic XR initialization |
| Oculus | ✓ Enabled | Legacy provider, still works |
| OpenXR | ✓ Enabled (Recommended) | Modern, preferred provider |

### OpenXR Features (if using OpenXR)

| Feature | Status | Notes |
|---------|--------|-------|
| Meta Quest Support | ✓ Enabled | Required for Quest devices |
| Hand Tracking Subsystem | ✓ Enabled | For hand tracking support |
| Eye Gaze Interaction | Optional | For eye tracking |

## Player Settings (Android)

### Other Settings

| Setting | Value | Notes |
|---------|-------|-------|
| Color Space | Linear | Better visual quality |
| Auto Graphics API | Disabled | Manual control preferred |
| Graphics APIs | OpenGLES3 / Vulkan | Quest supports both |
| Multithreaded Rendering | ✓ Enabled | Better performance |
| Static Batching | ✓ Enabled | Reduces draw calls |
| Dynamic Batching | ✓ Enabled | For small objects |
| Scripting Backend | IL2CPP | Required for Quest |
| API Compatibility Level | .NET Standard 2.1 | Recommended |
| Target Architectures | ARM64 | Quest uses ARM64 |
| Active Input Handling | Both / New | Depends on your input approach |

### Publishing Settings

#### Required Permissions
```xml
<!-- Spatial Anchors -->
<uses-permission android:name="com.oculus.permission.USE_ANCHORS" />

<!-- Passthrough Camera Access -->
<uses-permission android:name="android.permission.CAMERA" />

<!-- Optional: Manage external camera -->
<uses-permission android:name="com.oculus.permission.MANAGE_CAMERA" />
```

## OVR Manager Settings

Configure these in the OVRCameraRig's OVR Manager component:

### Tracking

| Setting | Value | Notes |
|---------|-------|-------|
| Tracking Origin Type | Floor Level | For room-scale experiences |
| Use Position Tracking | ✓ Enabled | Always enabled |
| Use IPD In Position Tracking | ✓ Enabled | For accurate positioning |

### Performance

| Setting | Value | Notes |
|---------|-------|-------|
| CPU Level | 3-4 | Balance power and performance |
| GPU Level | 3-4 | Balance power and performance |
| Display Refresh Rate | 90 Hz | Quest 2/3/Pro default |
| Foveated Rendering Level | High | For better performance |
| Enable Dynamic Foveated Rendering | ✓ Enabled | Adaptive quality |

### Passthrough

| Setting | Value | Notes |
|---------|-------|-------|
| Insight Passthrough Enabled | ✓ Enabled | Required for passthrough |
| Enable Passthrough on Startup | ✓ Enabled | For home-like experience |

### Quest Features

| Setting | Value | Notes |
|---------|-------|-------|
| Anchor Support | ✓ Enabled | For spatial anchors |
| Shared Anchor Support | Optional | For multiplayer |
| Scene Support | Optional | For scene mesh |

## Quality Settings

Create a Quest-specific quality level:

| Setting | Value |
|---------|-------|
| Pixel Light Count | 1-2 |
| Texture Quality | Half Res |
| Anisotropic Textures | Per Texture |
| Anti Aliasing | 4x MSAA |
| Soft Particles | Disabled |
| Real-time Reflection Probes | Disabled |
| Shadows | Hard Shadows Only |
| Shadow Resolution | Low |
| Shadow Distance | 20-30 |

## URP Settings (if using Universal Render Pipeline)

### URP Asset Settings

| Setting | Value |
|---------|-------|
| Render Scale | 1.0 |
| Max Vertex Lights | 2 |
| Main Light | Per Pixel |
| Additional Lights | Per Vertex |
| Cast Shadows | Optional (performance cost) |
| HDR | Disabled (for performance) |
| Anti Aliasing (MSAA) | 4x |

## Build Settings

| Setting | Value |
|---------|-------|
| Platform | Android |
| Texture Compression | ASTC |
| ETC2 Fallback | 32-bit |
| Build System | Gradle |
| Export Project | Optional |
| Development Build | For testing |

## Recommended Layer Setup

| Layer | Purpose |
|-------|---------|
| Default | Standard objects |
| UI | UI elements |
| RealWorld | Physical surfaces for raycasting |
| VirtualItems | Placed virtual items |
| Passthrough | Passthrough-specific layers |

## Input Action Mapping Reference

### Controller Buttons (OVRInput)

| Button | Code | Common Use |
|--------|------|------------|
| Primary Index Trigger | `OVRInput.Button.PrimaryIndexTrigger` | Select/Place |
| Secondary Index Trigger | `OVRInput.Button.SecondaryIndexTrigger` | Alternative select |
| One (X/A) | `OVRInput.Button.One` | Confirm |
| Two (Y/B) | `OVRInput.Button.Two` | Cancel/Menu |
| Primary Thumbstick | `OVRInput.Button.PrimaryThumbstick` | Movement |
| Menu | `OVRInput.Button.Start` | Pause menu |

### Hand Tracking Gestures

| Gesture | Detection | Common Use |
|---------|-----------|------------|
| Pinch | Index + Thumb | Select |
| Point | Index extended | Point/Raycast |
| Fist | All fingers closed | Grab |
| Open Palm | All fingers extended | Menu |
