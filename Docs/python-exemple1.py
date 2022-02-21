"""
Top 2% Submission by Alexandre Sajus and Fabien Roger
Coders Strike Back Challenge: https://www.codingame.com/multiplayer/bot-programming/coders-strike-back
More information at:
https://github.com/AlexandreSajus
"""

"""
This submission is for the Gold league, our strategy is to have one pod focusing on completing the race and one pod focusing on disturbing the opponents
"""

# Imports

import sys
import math
import numpy as np
from math import sqrt, atan2

# This maps a value from a range to another range
def map_value(v, frommin, frommax, tomin, tomax):
    return tomin + (tomax - tomin)*(v-frommin)/(frommax - frommin)

# This calculates the absolute angle between two vectors


def calc_absolute_angle(x1, y1, x2, y2):
    vector_1 = [x1, y1]
    vector_2 = [x2, y2]
    # np.maximum(1e-8, np. linalg. norm(vector_1))
    unit_vector_1 = vector_1 / \
        np.clip(np. linalg. norm(vector_1), 0.00001, 100000000)
    # np.maximum(1e-8, np. linalg. norm(vector_2))
    unit_vector_2 = vector_2 / \
        np.clip(np. linalg. norm(vector_2), 0.00001, 100000000)
    dot_product = np.dot(unit_vector_1, unit_vector_2)
    return np.degrees(np.arccos(dot_product))

# This calculates the angle of a vector compared to the X-axis


def calc_angle(x1, y1, x2, y2):
    return np.degrees(np.arctan2(y2-y1, x2-x1))


# Inputs, explained on Codingame
laps = int(input())
checkpoint_count = int(input())
checkpoints = []
for i in range(checkpoint_count):
    checkpoint_x, checkpoint_y = [int(j) for j in input().split()]
    checkpoints.append((checkpoint_x, checkpoint_y))

last_checkpoint_id1 = -1    # We save the last checkpoint we went through
loop1 = 0   # We count the number of laps we did
boost1 = 1  # Do we have a boost left?
last_checkpoint_id2 = -1    # Same thing for pod 2
loop2 = 0
boost2 = 1

old_next_check_point_id_1 = 0   # We save the last checkpoint they went through
old_next_check_point_id_2 = 0
progression_1 = 0   # The amount of checkpoints the opponents went through
progression_2 = 0   # For pod 2

"""
This defines the behavior of the runner pod, focusing on completing the race
"""


def behavior1(x, y, vx, vy, angle, next_check_point_id, loop, last_checkpoint_id, boost):
    """
    PARAMETERS
    """
    direction_compensation = 1.4    # The amount to overcompensate the direction to next checkpoint
    dist_slowdown = 2400    # At what distance from checkpoint we slow down
    thrust_slowdown = 20    # What thrust when we slow down
    thrust_amplitude = 140  # Amplitude of the gaussian defining the thrust
    thrust_deviation = 5000  # Deviation of the gaussian defining the thrust
    thrust_min = 50  # Thrust when we are going in the wrong direction
    dist_boost = 2500   # At what distance from checkpoint is it safe to boost
    angle_boost = 10    # At what angle from checkpoint is it safe to boost
    boost = 1   # Do we have a boost
    radius = 100   # The radius of a checkpoint
    # At what distance from checkpoint we start to turn to the next checkpoint
    dist_turn = 2400
    max_angle = 120  # Clamp the target angle
    map_center = [8000, 4500]   # Center of the map

    """
    DEFINING METRICS
    """
    # Counts the laps
    if last_checkpoint_id != next_check_point_id:
        loop += 1

    # Update checkpoints
    last_checkpoint_id = next_check_point_id

    next_checkpoint_x, next_checkpoint_y = checkpoints[next_check_point_id]
    next_next_checkpoint_x, next_next_checkpoint_y = checkpoints[(
        next_check_point_id+1) % len(checkpoints)]

    # Compute the distance to next checkpoint
    dist_checkpoint = sqrt((x - next_checkpoint_x) **
                           2 + (y - next_checkpoint_y)**2)

    # Compute our current speed
    speed = sqrt(vx**2 + vy**2)

    # Compute the angle between the speed vector and the vector MC
    # where M the the position and C is the next checkpoint
    to_checkpoint_x = next_checkpoint_x - x
    to_checkpoint_y = next_checkpoint_y - y
    speed_checkpoint_angle = calc_absolute_angle(vx, vy,
                                                 to_checkpoint_x, to_checkpoint_y)

    # Compute the angle between the current angle and the vector CC'
    next_checkpoint_angle_abs = calc_angle(
        next_checkpoint_x, next_checkpoint_y, next_next_checkpoint_x, next_next_checkpoint_y)
    turning_strength_after_checkpoint = abs(
        (next_checkpoint_angle_abs - angle + 180) % 360 - 180)

    # Compute the angle between the direciton and the direction to the next checkpoint
    next_checkpoint_angle_abs = calc_angle(
        x, y, next_checkpoint_x, next_checkpoint_y)
    next_checkpoint_angle = (
        next_checkpoint_angle_abs - angle + 180) % 360 - 180

    # Define a target angle by overcompensating
    target_angle = next_checkpoint_angle*direction_compensation
    #print(target_angle, file=sys.stderr, flush=True)
    # Clamp the target angle
    target_angle = min(max_angle, target_angle)
    target_angle = max(-max_angle, target_angle)

    # Compute the difference between checkpoint and target angle
    # We will use this to define a target point as (X,Y)
    diff_angle = target_angle - next_checkpoint_angle

    #print("next_checkpoint_angle", next_checkpoint_angle_abs, file=sys.stderr, flush=True)
    #print("next_checkpoint_angle", next_checkpoint_angle, file=sys.stderr, flush=True)

    # Compute the angle between MC and MC'
    # Where C' is the side of the checkpoint
    limit_angle = np.degrees(np.arctan(600/dist_checkpoint))
    switch_checkpoint = False

    frame_advance = 5
    dist_turn_adjusted = map_value(turning_strength_after_checkpoint,
                                   0, 180, frame_advance*speed*0.7, frame_advance*speed*1.5)

    """
    THRUST BEHAVIOUR
    """
    thrust = 0

    # If the next checkpoint is far and we are aiming at it, BOOST
    if dist_checkpoint > 8000 and abs(next_checkpoint_angle) < angle_boost and boost == 1:
        thrust = "BOOST"
        boost = 0
    # If its the last checkpoint of the race, BOOST if you can
    elif next_checkpoint_angle < angle_boost and loop == laps and next_check_point_id == 0:
        print("final !", loop, laps, file=sys.stderr, flush=True)
        if boost == -1:
            thurst = 100
        else:
            thrust = "BOOST"
    # If we are close to the next checkpoint and racing toward it at full speed
    # Then start turning toward the checkpoint after that
    elif dist_checkpoint < dist_turn_adjusted and speed_checkpoint_angle < limit_angle:

        thrust = 0
        switch_checkpoint = True
        next_checkpoint = checkpoints[(
            next_check_point_id+1) % len(checkpoints)]
        next_checkpoint_x, next_checkpoint_y = next_checkpoint

    # If we are going in the wrong direction, low thrust
    elif next_checkpoint_angle > 90 or next_checkpoint_angle < -90 and dist_checkpoint < 1200:
        thrust = 0
    elif next_checkpoint_angle > 90 or next_checkpoint_angle < -90:
        thrust = thrust_min
    # If we are close to the next checkpoint with a high speed, slow down
    elif dist_checkpoint < dist_slowdown and speed > 400:
        thrust = thrust_slowdown
    # Else, the thrust is defined by a gaussian according to the checkpoint angle
    # If we are aiming at the checkpoint go fast, if not go slow
    else:
        thrust = thrust_amplitude * \
            np.exp(-next_checkpoint_angle**2/thrust_deviation)
        thrust = min(100, thrust)

    """
    DIRECTION BEHAVIOUR
    """
    # Defining a target (X,Y)

    # We wont aim for the center of the checkpoint
    # Instead well aim for the part of the checkpoint closest to the center of the map
    checkpoint_to_center = np.asarray(
        [next_checkpoint_x - map_center[0], next_checkpoint_y - map_center[1]])
    checkpoint_to_center_norm = sqrt(
        checkpoint_to_center[0]**2 + checkpoint_to_center[1]**2)
    # Unit vector from checkpoint to center
    checkpoint_to_center_unit = checkpoint_to_center/checkpoint_to_center_norm
    # Update the target (X,Y)
    next_checkpoint_x -= checkpoint_to_center_unit[0]*radius
    next_checkpoint_y -= checkpoint_to_center_unit[1]*radius

    # This is the vector to the checkpoint
    to_checkpoint = np.asarray(
        [[next_checkpoint_x - x], [next_checkpoint_y - y]])
    #print("to_checkpoint", file=sys.stderr, flush=True)
    #print(to_checkpoint, file=sys.stderr, flush=True)

    # We rotate to_checkpoint using the target_angle
    theta = np.radians(diff_angle)
    c, s = np.cos(theta), np.sin(theta)
    R = np.array(((c, -s), (s, c)))
    #print("R", file=sys.stderr, flush=True)
    #print(R, file=sys.stderr, flush=True)

    # To target is the vector oriented by the target_angle
    to_target = np.dot(R, to_checkpoint)
    #print("to_target", file=sys.stderr, flush=True)
    #print(to_target, file=sys.stderr, flush=True)

    # This is our target (X,Y)
    target_x = to_target[0][0] + x
    target_y = to_target[1][0] + y

    # See the case in which we aim at the checkpoint after the next one
    if switch_checkpoint:
        target_x = next_checkpoint_x
        target_y = next_checkpoint_y

    map_center = [8000, 4500]   # The center of the map

    r = None

    """
    OUTPUT
    """
    if thrust == "BOOST":
        boost1 = 0
        r = str(int(target_x)) + " " + str(int(target_y)) + " " + thrust
    else:
        r = str(int(target_x)) + " " + \
            str(int(target_y)) + " " + str(int(thrust))

    return loop, last_checkpoint_id, boost, r


# game loop
while True:
    # Inputs, explained in CodinGame
    x1, y1, vx1, vy1, angle1, next_check_point_id1 = [
        int(j) for j in input().split()]
    x2, y2, vx2, vy2, angle2, next_check_point_id2 = [
        int(j) for j in input().split()]

    x_1, y_1, vx_1, vy_1, angle_1, next_check_point_id_1 = [
        int(j) for j in input().split()]
    x_2, y_2, vx_2, vy_2, angle_2, next_check_point_id_2 = [
        int(j) for j in input().split()]

    """
    OBSERVING OPPONENTS
    """
    # We track how many checkpoints our oppponents have passed
    if next_check_point_id_1 != old_next_check_point_id_1:
        progression_1 += 1
        old_next_check_point_id_1 = next_check_point_id_1
    if next_check_point_id_2 != old_next_check_point_id_2:
        progression_2 += 1
        old_next_check_point_id_2 = next_check_point_id_2

    """
    This is the hunter pod, it chases after the winning opponent and tries to bump him
    """
    def hunter():
        global boost2
        """
        PARAMETERS
        """
        direction_compensation = 1    # The amount to overcompensate the direction to next checkpoint
        thrust_amplitude = 140  # Amplitude of the gaussian defining the thrust
        thrust_deviation = 5000  # Deviation of the gaussian defining the thrust
        thrust_min = 50  # Thrust when we are going in the wrong direction
        dist_boost = 6000   # At what distance from target is it safe to boost
        angle_boost = 20    # At what angle from target is it safe to boost
        dist_shield = 1500  # At what distance from target is it safe to shield
        angle_shield = 360  # At what angle from target is it safe to shield
        # We wont aim for where the target is but where the target will be
        time_steps_ahead = 6   # How many steps ahead are we aiming at

        """
        DEFINING METRICS
        """
        # We choose the winning opponent as the target
        if progression_2 < progression_1:
            target = np.array([[x_1 + time_steps_ahead*vx_1],
                              [y_1 + time_steps_ahead*vy_1]])
            to_target = target - np.array([[x2], [y2]])
        else:
            target = np.array([[x_2 + time_steps_ahead*vx_2],
                              [y_2 + time_steps_ahead*vy_2]])
            to_target = target - np.array([[x2], [y2]])

        dist_target = np.linalg.norm(to_target)

        vect_speed = np.array([[vx2], [vy2]])
        speed = np.linalg.norm(vect_speed)

        def calc_angle(x1, y1, x2, y2):
            return np.degrees(np.arctan2(y2-y1, x2-x1))

        # Calculating the target angle
        to_target_abs = calc_angle(0, 0, to_target[0][0], to_target[1][0])
        target_angle = (to_target_abs - angle2 + 180) % 360 - 180

        target_angle_before = target_angle
        # We overcompensate the target angle
        target_angle = target_angle*direction_compensation
        target_angle = min(target_angle, 179)
        target_angle = max(target_angle, -179)
        # This is the angle we want to aim at

        """
        THRUST BEHAVIOUR
        """
        # If we are going in the wrong direction, low thrust
        if target_angle > 90 or target_angle < -90:
            thrust = thrust_min
        # If we are close and aiming at the target, BOOST to try to bump him
        elif abs(target_angle) < angle_boost and dist_target < dist_boost and boost2 == 1:
            thrust = "BOOST"
            boost2 = 0
        # Else, we use a gaussian to define thrust, the more we aim at the target, the more we thrust
        else:
            thrust = thrust_amplitude * \
                np.exp(-target_angle**2/thrust_deviation)
            thrust = int(min(100, thrust))

        """
        DIRECTION BEHAVIOUR
        """
        # We convert our target angle to an (X,Y) target using a rotation matrix
        diff_angle = target_angle - target_angle_before

        theta = np.radians(diff_angle)
        c, s = np.cos(theta), np.sin(theta)
        R = np.array(((c, -s), (s, c)))

        to_target = np.dot(R, to_target)

        target_x = to_target[0][0] + x2
        target_y = to_target[1][0] + y2

        """
        OUTPUT
        """
        print(str(int(target_x)) + " " + str(int(target_y)) + " " + str(thrust))

    # Execute both pods
    loop1, last_checkpoint_id1, boost1, r1 =\
        behavior1(x1, y1, vx1, vy1, angle1, next_check_point_id1,
                  loop1, last_checkpoint_id1, boost1)
    print(r1)
    '''loop2, last_checkpoint_id2, boost1, r2 =\
        behavior1(x2,y2,vx2,vy2,angle2, next_check_point_id2, loop2, last_checkpoint_id2, boost2)
    print(r2)'''
    hunter()