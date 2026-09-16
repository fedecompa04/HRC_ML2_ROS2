import os
from launch import LaunchDescription
from launch_ros.actions import Node

def generate_launch_description():
    return LaunchDescription([
        Node(
            package = 'robot_interaction_ml2',
            executable = 'tf_transformation_node',
            output = 'screen',
            emulate_tty = True
        )
    ])