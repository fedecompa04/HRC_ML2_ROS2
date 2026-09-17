# Operational Session Startup Guide (ROS2 Side)

This README outlines the complete procedure required to initiate and execute an operational session using the ROS2-based development environment for the Magic Leap 2 integration.

## Prerequisites & Setup Instructions

1. **Bridge Configuration:**  
   Follow the initial bridge setup procedure detailed in the [ROS2 Development README](https://github.com/fedecompa04/HRC_ML2_ROS2/blob/main/ML2_ROS2_Bridge/ROS2_development/README.md) to establish and verify communication between ROS2 and the Magic Leap 2 headset.

2. **Workspace Integration:**  
   Integrate the following packages into your active ROS2 workspace:
   
   * **`robot_interaction_ml2`:**  
     This custom package is responsible for computing the spatial pose of detected AprilTags relative to the `magicleap_world` frame (the origin point established when the Magic Leap 2 headset is powered on/initialized).
   
   * **`apriltag_ros`:**  
     Clone and configure the [christianrauch/apriltag_ros](https://github.com/christianrauch/apriltag_ros) repository. With proper configuration, this package allows the Magic Leap 2 optical camera streams to accurately estimate the relative pose of AprilTags directly within the camera sensor frame.

## Next Steps

Once all required packages have been successfully added to your workspace, build your workspace (`colcon build`) and source your environment before launching the respective nodes.