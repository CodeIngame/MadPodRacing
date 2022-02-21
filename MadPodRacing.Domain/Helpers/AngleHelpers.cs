namespace MadPodRacing.Domain.Helpers
{
    using MadPodRacing.Domain.Entities;
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    using System.Text;

    public static class AngleHelpers
    {
        /// <summary>
        /// Calcul l'angle en degrés entre 2 points
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double ComputeAngle(Point p1, Point p2)
        {

            var v1 = new Vector2(p1.X, p1.Y);
            var v2 = new Vector2(p2.X, p2.Y);

            var normV1 = Vector2.Normalize(v1);
            var normV2 = Vector2.Normalize(v2);

            var product = Vector2.Dot(normV1 ,normV2);
            return Math.Acos(product).ToDegree();
        }
    }
}
