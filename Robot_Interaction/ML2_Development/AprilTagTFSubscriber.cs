using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry; // Usiamo SOLO geometry_msgs


/**
This script is used to retrieve the pose of the april tag from the origin magicleap_world.
It has to be attached to the GameObject that represents the working plan and the AprilTag needs to be above it.
*/

public class TargetPoseSubscriber : MonoBehaviour
{
    // Cambiamo il topic con quello esposto dal tuo nuovo nodo Python launcher
    public string topicName = "/magicleap_to_tag4_pose";
    

    void Start()
    {
        // Ci iscriviamo direttamente al topic PoseStampedMsg
        ROSConnection.GetOrCreateInstance().Subscribe<PoseStampedMsg>(topicName, OnPoseReceived);
    }

    void OnPoseReceived(PoseStampedMsg msg)
    {
        // 1. Estraiamo la posizione XYZ calcolata dal buffer di ROS2
        float x = (float)msg.pose.position.x;
        float y = (float)msg.pose.position.y;
        float z = (float)msg.pose.position.z;

        // Estraiamo la rotazione (Quaternione)
        float qx = (float)msg.pose.orientation.x;
        float qy = (float)msg.pose.orientation.y;
        float qz = (float)msg.pose.orientation.z;
        float qw = (float)msg.pose.orientation.w;

        // 2. CONVERSIONE ASSI STANDARD (Da ROS standard RHS a Unity LHS)
        Vector3 unityPosition = new Vector3(-y, z, x);
        Quaternion unityRotation = new Quaternion(-qy, qz, qx, -qw);

        // 3. APPLICAZIONE DIRETTACCIA SULLA SFERA
        transform.localPosition = unityPosition;
        transform.localRotation = unityRotation;
    }
}