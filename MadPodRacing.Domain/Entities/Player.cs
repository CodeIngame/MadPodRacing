
namespace MadPodRacing.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Player
    {
        /// <summary>
        /// La position du joueur
        /// </summary>
        public PlayerPoint Position { get; set; }
        public int Boost { get; set; } = 1;
    }
}
