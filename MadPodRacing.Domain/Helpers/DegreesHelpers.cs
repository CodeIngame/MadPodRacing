namespace MadPodRacing.Domain.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public static class DegreesHelpers
    {
        public static double ToRadian(this double r)
        {
            return r * Math.PI / 180;
        }

        public static double ToRadian(this int r)
        {
            return r * Math.PI / 180;
        }
    }
}
