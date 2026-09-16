#!/usr/bin/env python3
import rclpy
from rclpy.node import Node
from geometry_msgs.msg import PoseStamped
from tf2_ros import TransformException
from tf2_ros.buffer import Buffer
from tf2_ros.transform_listener import TransformListener

class TFChainLinker(Node):

    def __init__(self):
        super().__init__('tf_chain_linker', automatically_declare_parameters_from_overrides=False)

        # 1. Inizializziamo il Buffer e il Listener delle TF
        self.tf_buffer = Buffer()
        self.tf_listener = TransformListener(self.tf_buffer, self)

        # 2. Creiamo un publisher per inviare la posa finale combinata
        self.publisher_ = self.create_publisher(PoseStamped, '/magicleap_to_tag4_pose', 10)

        # 3. Timer per calcolare la trasformata a 20Hz (ogni 0.05 secondi)
        self.timer = self.create_timer(0.05, self.timer_callback)
        
        self.get_logger().info("Nodo di calcolo TF avviato. In ascolto della catena...")

    def timer_callback(self):
        # Definiamo il frame di partenza (Padre) e quello di arrivo (Figlio)
        target_frame = 'tag4'
        source_frame = 'magicleap_world'

        if not self.tf_buffer.can_transform(target_frame, source_frame, rclpy.time.Time(), rclpy.duration.Duration(seconds=0.1)):
            return

        try:
            # Chiediamo alla TF di unire i punti e calcolare la trasformata diretta
            # rclpy.time.Time() prende l'ultima trasformata disponibile
            now = rclpy.time.Time()
            trans = self.tf_buffer.lookup_transform(
                source_frame,
                target_frame,
                now)
            
            # Costruiamo il messaggio PoseStamped da inviare a Unity
            msg = PoseStamped()
            msg.header.stamp = self.get_clock().now().to_msg()
            msg.header.frame_id = source_frame # magicleap_world
            
            # Assegniamo la posizione risultante dal calcolo della TF
            msg.pose.position.x = trans.transform.translation.x
            msg.pose.position.y = trans.transform.translation.y
            msg.pose.position.z = trans.transform.translation.z
            
            # Assegniamo la rotazione risultante dal calcolo della TF
            msg.pose.orientation.x = trans.transform.rotation.x
            msg.pose.orientation.y = trans.transform.rotation.y
            msg.pose.orientation.z = trans.transform.rotation.z
            msg.pose.orientation.w = trans.transform.rotation.w

            # Pubblichiamo la posa finale
            self.publisher_.publish(msg)

        except TransformException as ex:
            # Se la TF non è ancora pronta (es. la telecamera non vede il tag), evita il crash e avvisa
            self.get_logger().warn(f'Impossibile calcolare la TF: {ex}')

def main(args=None):
    rclpy.init(args=args)
    node = TFChainLinker()
    try:
        rclpy.spin(node)
    except KeyboardInterrupt:
        pass
    finally:
        node.destroy_node()
        rclpy.shutdown()

if __name__ == '__main__':
    main()