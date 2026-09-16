using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using RosMessageTypes.Geometry;

/**
In this script we dealed with the raycast of the controller and the pointing of the target position.
The target position is published on a ROS 2 topic and then it will be processed with MoveIt 2 library.
*/

public class MagicLeapTriggerPointer : MonoBehaviour
{
    [Header("Riferimenti Scene Unity")]
    [Tooltip("Trascina qui la Sfera/Pallino verde che deve muoversi sulla lavagna")]
    public Transform targetBall;          

    [Header("Impostazioni Raggio")]
    [Tooltip("La distanza massima a cui può arrivare il puntatore laser")]
    public float maxDistance = 20f;

    // Actions per leggere la posa del puntatore nativo di Magic Leap 2
    private InputAction pointerPosAction;
    private InputAction pointerRotAction;
    
    // Action per rilevare il click del grilletto (Trigger)
    private InputAction triggerAction;

    [Header("Configurazione ROS")]
    [Tooltip("Nome del topic ROS su cui pubblicare la posa")]
    public string rosTopicName = "target_ball_pose";
    [Tooltip("Il frame di riferimento ROS (es. 'world', 'map', 'base_link')")]
    public string frameId = "magicleap_world"; //magicleap_world
    private ROSConnection ros;

    void Start()
    {
        // Inizializza la connessione e registra il topic
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<PoseStampedMsg>(rosTopicName);
    }
    void OnEnable()
    {
        // Inizializziamo i binding OpenXR nativi di Magic Leap 2
        pointerPosAction = new InputAction(binding: "<MagicLeapController>/pointer/position");
        pointerRotAction = new InputAction(binding: "<MagicLeapController>/pointer/rotation");
        triggerAction = new InputAction(binding: "<MagicLeapController>/triggerPressed");

        pointerPosAction.Enable();
        pointerRotAction.Enable();
        triggerAction.Enable();
    }

    void OnDisable()
    {
        pointerPosAction.Disable();
        pointerRotAction.Disable();
        triggerAction.Disable();
    }

    void Update()
    {
        if (targetBall == null) return;

        // Controlliamo se il grilletto è stato premuto esattamente in questo frame
        if (triggerAction.WasPressedThisFrame())
        {
            // 1. Leggiamo la posizione e la rotazione attuali del controller
            Vector3 controllerPos = pointerPosAction.ReadValue<Vector3>();
            Quaternion controllerRot = pointerRotAction.ReadValue<Quaternion>();

            // 2. Calcoliamo la direzione in avanti del controller (asse Z locale)
            Vector3 rayDirection = controllerRot * Vector3.forward;

            // 3. Creiamo il raggio matematico (Raycast)
            Ray ray = new Ray(controllerPos, rayDirection);
            RaycastHit hit;

            // 4. Spariamo il raggio nel mondo virtuale di Unity
            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                // CONTROLLO DI SICUREZZA: Muoviamo il pallino SOLO se colpiamo la lavagna
                if (hit.collider.CompareTag("Player"))
                {
                    // Spostiamo la pallina verde nel punto esatto di intersezione sul Box
                    targetBall.position = hit.point;

                    // Opzionale: Orienta la pallina in modo che rimanga "piatta" sulla superficie
                    targetBall.rotation = Quaternion.LookRotation(hit.normal);

                    // Calcoliamo al volo la distanza tra il visore (Main Camera) e il punto salvato
                    if (Camera.main != null)
                    {
                        float distanzaDagliOcchi = Vector3.Distance(Camera.main.transform.position, hit.point);
                        Debug.Log($"Pallina posizionata! Distanza dal visore: {distanzaDagliOcchi:F2} metri.");
                    }
                }
            }
            PublishPoseToROS();
        }
    }

    private void PublishPoseToROS()
    {
        if (ros == null) return;

        System.DateTimeOffset now = System.DateTimeOffset.UtcNow;
        long unixTimeMilliseconds = now.ToUnixTimeMilliseconds();

        // 2. Separa in secondi e nanosecondi (i millisecondi rimanenti moltiplicati per 1.000.000)
        uint sec = (uint)(unixTimeMilliseconds / 1000);
        uint nanosec = (uint)((unixTimeMilliseconds % 1000) * 1000000);

        // Costruisci il messaggio PoseStamped
        PoseStampedMsg poseMessage = new PoseStampedMsg
        {
            header = new HeaderMsg
            {
                frame_id = frameId,
                stamp = new RosMessageTypes.BuiltinInterfaces.TimeMsg((int)sec, nanosec)
            },
            pose = new PoseMsg
            {
                // Conversione da coordinate Unity (Mano sinistra) a ROS (Mano destra)
                position = new PointMsg(targetBall.position.z, -targetBall.position.x, targetBall.position.y),
                orientation = new QuaternionMsg(-targetBall.rotation.z, targetBall.rotation.x, -targetBall.rotation.y, targetBall.rotation.w)

            }
        };

        // Invia il messaggio
        ros.Publish(rosTopicName, poseMessage);
    }
}
