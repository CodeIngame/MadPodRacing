namespace MadPodRacing.Domain.Common
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public static class Constantes
    {
        public static int MAX_POWER = 100;
        public static int HALF_POWER = 50;
        public static int SLOW_POWER = 20;
        public static int STOP_POWER = 0;


        public static int MIN_DISTANCE = 2400;

        public static int ALLOWED_BOOST = 1;
        public static int ALLOWED_SHIELD = 3;

        public static double DIRECTION_COMPENSATION = 1.4;
        public static int THRUST_AMPLITUDE = 140;
        public static int THRUST_DEVIATION = 5000;

        public static int SAFE_DISTANCE_BOOST = 2500;
        public static int SAFE_ANGLE_BOOST = 10;

        public static int CHECKPOINT_RADIUS = 600;
        public static int CHECKPOINT_DISTANCE_MOVE_NEXT = 1800;
        public static int CHECKPOINT_DISTANCE_IS_FAR = 8000;

        public static int MAP_SIZE_X = 16000;
        public static int MAP_SIZE_Y = 9000;

        public static int SPEED_FAST = 400;

    }
}
