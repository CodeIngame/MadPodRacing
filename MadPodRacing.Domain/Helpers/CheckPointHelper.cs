

namespace MadPodRacing.Domain.Helpers
{
    using MadPodRacing.Domain.Entities;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public static class CheckPointHelper
    {
        /// <summary>
        /// Retourne le point de passage suivant
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static CheckPoint Next(this CheckPoint cp, Race r)
        {
            var currentIndex = r.CheckPoint.IndexOf(cp);
            var index = currentIndex == r.CheckPoint.Count - 1 ? 0 : currentIndex + 1;
            return r.CheckPoint[index];
        }

        /// <summary>
        /// Retourne le point de passage précédant
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static CheckPoint Previous(this CheckPoint cp, Race r)
        {
            var currentIndex = r.CheckPoint.IndexOf(cp);
            var index = currentIndex == 0 ? r.CheckPoint.Count - 1 : currentIndex - 1;
            return r.CheckPoint[index];
        }
    }
}
