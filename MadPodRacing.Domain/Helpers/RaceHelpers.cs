

namespace MadPodRacing.Domain.Helpers
{
    using MadPodRacing.Domain.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class RaceHelpers
    {
        public static CheckPoint NextPoint(this Race r)
        {
            return r.CheckPoint.First(p => p.IsCurrent);
        }

        /// <summary>
        /// Défini le point suivant comme étant le point courant
        /// </summary>
        /// <param name="r"></param>
        public static void SetNextPointToCurrent(this Race r)
        {
            // CheckPoint courant
            var next = r.NextPoint();
            var nn = next.Next(r);

            next.IsCurrent = false;
            nn.IsCurrent = true;

        }
    }
}
