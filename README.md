# Mvrset - Meta Quest VR Home Experience

A Unity-based VR experience for Meta Quest 2/3/Pro that allows users to create their own "home-like" environment with passthrough background and place items (furniture, lamps, decorations) anchored to their real room using Meta's Passthrough and Spatial Anchors.

## Features

- **Passthrough World**: See your real room as the background for VR content
- **Spatial Anchors**: Place items that persist across sessions and stay anchored to real-world positions
- **Drawing in Passthrough**: Stylize and draw on your passthrough view
- **Item Placement**: Place furniture, lamps, and decorations in your room

## Prerequisites

- Unity 2022.3 LTS (URP recommended)
- Meta Quest 2/3/Pro with latest OS
- Meta XR Core SDK
- Meta Spatial Anchors plugin
- Optional: Meta XR Interaction SDK for hand/controller interactions

## Quick Start

1. See [Setup Guide](docs/SETUP.md) for project configuration
2. Import the scripts from the `Scripts/` folder
3. Follow the [Passthrough Setup](docs/PASSTHROUGH.md) guide
4. Configure [Spatial Anchors](docs/SPATIAL_ANCHORS.md)

## Project Structure

```
Mvrset/
├── README.md                 # This file
├── docs/
│   ├── SETUP.md             # Project setup guide
│   ├── PASSTHROUGH.md       # Passthrough configuration
│   └── SPATIAL_ANCHORS.md   # Spatial anchors guide
├── Scripts/
│   ├── PlaceItemInRoom.cs   # Item placement with anchors
│   ├── AnchorManager.cs     # Anchor persistence management
│   └── PassthroughDrawing.cs # Drawing in passthrough
└── ProjectSettings/
    └── XRSettings.md        # XR configuration reference
```

## Documentation

- [Setup Guide](docs/SETUP.md) - Create project and import SDKs
- [Passthrough Guide](docs/PASSTHROUGH.md) - Enable passthrough background
- [Spatial Anchors Guide](docs/SPATIAL_ANCHORS.md) - Item placement and persistence

## Scripts

### PlaceItemInRoom.cs
Main script for placing items in the room using raycasting and spatial anchors.

### AnchorManager.cs
Manages anchor persistence and loading saved anchors on startup.

### PassthroughDrawing.cs
Enables drawing features in passthrough mode for stylizing the environment.

## Building for Quest

1. Switch platform to Android in Build Settings
2. Ensure IL2CPP scripting backend is selected
3. Add required permissions in Publishing Settings
4. Build and deploy to your Quest device

## License

MIT License - See LICENSE file for details