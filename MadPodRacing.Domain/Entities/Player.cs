
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

        /// <summary>
        /// Défini l'accélération (0-100)
        /// </summary>
        public int Power { get; set; } = 0;
        /// <summary>
        /// Prochain point de passage
        /// </summary>
        public RacePoint NextPoint { get; set; }
        /// <summary>
        /// Le point qu'on vient de passer
        /// </summary>
        public RacePoint PreviousPoint { get; set; }

    }
}
