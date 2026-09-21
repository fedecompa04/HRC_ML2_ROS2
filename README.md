# Bridge between Magic Leap 2 and ROS 2 with HRC (Human-Robot Collaboration) Extension

Official repository of the Bachelor's degree thesis in Computer Engineering at the **University of Padua (Department of Information Engineering)**.

* **Candidate:** Federico Compagno
* **Advisor:** Prof. Stefano Ghidoni
* **Co-advisor:** Matteo Terreran, PhD

---

## Project Overview
This project presents the design, development, and experimental validation of a high-performance, modular software infrastructure for **real-time bi-directional communication** between the **Magic Leap 2** Augmented Reality (AR) headset and the **ROS 2** robotic middleware. The goal is to enable natural and intuitive human-robot collaboration (HRC) interfaces, overcoming the limitations of traditional 2D monitors.

The architecture integrates:
1. **A Unity application (C#)** running on the headset for multi-sensor data acquisition (RGB video, World Cameras, Depth, IMU, Eye-Tracking)[cite: 2].
2. **An asynchronous TCP bridge (`ROS-TCP-Connector`)** for low-latency data transfer.
3. **A ROS 2 ecosystem** for spatial transformations management (`TF2`), optical tracking of **AprilTag** markers, and collision-free trajectory planning via **MoveIt 2** on a **Franka Emika Panda** cobot.

---

## Technology Stack
* **Robotic Framework:** ROS 2 (Jazzy)
* **3D Engine:** Unity (LTS 2022.3.62f3) with Magic Leap 2 OpenXR support
* **Motion Planning:** MoveIt 2
* **Spatial Tracking:** AprilTag (`apriltag_ros`)
* **Communication:** ROS-TCP-Connector

---

## Repository Structure
* [`ROS2_Development/ROS2_development`](https://github.com/fedecompa04/HRC_ML2_ROS2/tree/main/ML2_ROS2_Bridge/ROS2_development): ROS 2 packages for bridge management and dynamic configuration via YAML files.
* [`Robot_Interaction/ROS2_Development/robot_interaction_ml2`](https://github.com/fedecompa04/HRC_ML2_ROS2/tree/main/Robot_Interaction/ROS2_Development/robot_interaction_ml2): ROS 2 package for TF transformation nodes.
* **External Dependencies:**
  * [ROS-TCP-Endpoint](https://github.com/Unity-Technologies/ROS-TCP-Endpoint)
  * [apriltag_ros](https://github.com/christianrauch/apriltag_ros)

---

### ROS 2 Environment Setup
Clone the packages into your ROS 2 workspace and build them:
```bash
cd ~/magicleap_ws/src
# Clone or place the required packages here
cd ~/magicleap_ws
colcon build --packages-select ros_tcp_endpoint config_ml2_stream robot_interaction_ml2
source install/setup.bash