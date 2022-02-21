

namespace MadPodRacing.Domain.Helpers
{
    using MadPodRacing.Domain.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class ListHelper
    {
        /// <summary>
        /// Essaie d'ajouter un élément dans la liste en fonction du X, Y
        /// </summary>
        /// <param name="l"></param>
        /// <param name="newItem"></param>
        /// <returns></returns>
        public static bool TryAdd(this List<CheckPoint> l, CheckPoint newItem)
        {
            var exist = l.Any(i => i.X == newItem.X && i.Y == newItem.Y);
            if (!exist)
            {
                l.Add(newItem);
                return true;
            }
            return false;
        }
    }
}
