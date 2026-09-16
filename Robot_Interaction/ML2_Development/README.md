# Unity ROS2 Magic Leap 2 Integration Scripts

This repository contains core Unity C# scripts designed to interface **Magic Leap 2** with a **ROS 2** ecosystem using the `Unity.Robotics.ROSTCPConnector`. These scripts handle spatial tracking via AprilTags and controller-based interaction/command publishing.

---

## Scripts Overview

### 1. `AprilTagTFSubscriber.cs`
* **Purpose**: Subscribes to a ROS 2 `PoseStamped` topic containing the relative transformation of an AprilTag with respect to the `magicleap_world` frame. It dynamically updates the target's position and orientation in the Unity scene.
* **Key Features**:
  * Listens to `/magicleap_to_tag4_pose` by default.
  * Performs coordinate system conversion between **ROS standard (Right-Handed System - RHS)** and **Unity (Left-Handed System - LHS)**.
  * Attaches directly to the GameObject representing the working plane/surface.

### 2. `MagicLeapTriggerPointer.cs`
* **Purpose**: Manages Magic Leap 2 OpenXR controller inputs, performs raycasting against designated interactive surfaces (e.g., a whiteboard/tag tagged as `"Player"`), visualizes target placement, and publishes the selected 3D coordinate back to ROS 2.
* **Key Features**:
  * Reads native Magic Leap 2 OpenXR controller actions (`pointer/position`, `pointer/rotation`, `triggerPressed`).
  * Validates raycast hits to ensure interaction occurs strictly on target surfaces.
  * Converts Unity LHS coordinates to ROS RHS coordinates.
  * Publishes a `PoseStamped` message containing a timestamped `magicleap_world` frame ID to a ROS 2 topic (default: `target_ball_pose`), intended for downstream processing (e.g., MoveIt 2).

---

## Prerequisites & Setup

1. **Unity Version**: Compatible with Unity projects configured for Magic Leap 2 (OpenXR Plugin).
2. **Dependencies**:
   * [Unity Robotics ROS-TCP-Connector](https://github.com/Unity-Technologies/ROS-TCP-Connector)
   * Unity OpenXR Plugin / Magic Leap Provider
3. **Setup Instructions**:
   * Attach `AprilTagTFSubscriber.cs` to your working surface GameObject.
   * Attach `MagicLeapTriggerPointer.cs` to your controller/manager GameObject and assign the target indicator ball transform in the inspector.
   * Ensure your ROS-TCP-Connector endpoint IP is properly configured within the Unity editor.