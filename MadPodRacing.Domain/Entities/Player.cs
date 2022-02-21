
namespace MadPodRacing.Domain.Entities
{
    using MadPodRacing.Domain.Common;
    using MadPodRacing.Domain.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Player
    {
        public Player()
        {
        }
        /// <summary>
        /// La position précédante du joueur
        /// </summary>
        public PlayerPoint PreviousPosition { get; set; }
        /// <summary>
        /// La position courante joueur
        /// </summary>
        public PlayerPoint Position { get; set; }
        /// <summary>
        /// Le nombre de boost autorisé
        /// </summary>
        public int Boost { get; set; } = Constantes.ALLOWED_BOOST;

        /// <summary>
        /// Défini l'accélération (0-100)
        /// </summary>
        public int Power { get; set; } = 0;
        /// <summary>
        /// Prochain point de passage
        /// </summary>
        public CheckPoint NextPoint { get; set; }
        /// <summary>
        /// Le point qu'on vient de passer
        /// </summary>
        public CheckPoint PreviousPoint { get; set; }

        /// <summary>
        /// Retourne la distance jusqu'au prochaine CP
        /// </summary>
        /// <returns></returns>
        public double ComputeDistanceNextCheckPoint()
        {
            return Math.Sqrt(Math.Pow(Position.X - NextPoint.X, 2) + Math.Pow(Position.Y - NextPoint.Y, 2));
        }

        /// <summary>
        /// Calcul la vitesse actuelle
        /// </summary>
        /// <returns></returns>
        public double ComputeSpeed()
        {
            return Math.Sqrt(Math.Pow(Position.X - PreviousPosition?.X ?? 0, 2) + Math.Pow(Position.Y - PreviousPosition?.Y ?? 0, 2));
        }

        /// <summary>
        /// Retourne le point entre le prochain CP et ma position
        /// </summary>
        /// <returns></returns>
        public Point ToCheckPoint()
        {
            return new Point(NextPoint.X - Position.X, NextPoint.Y - Position.Y);
        }

        public Point ToSpeed()
        {
            return new Point(Position.X - PreviousPosition?.X ?? 0, Position.Y - PreviousPosition?.Y ?? 0);
        }

        public double LimitAngle()
        {
            return Math.Atan(Constantes.CHECKPOINT_RADIUS / ComputeDistanceNextCheckPoint())
                .ToDegree();
        }

        public double TargetAngle()
        {
            return Math.Max(-179, Math.Min(179, Position.Angle * Constantes.DIRECTION_COMPENSATION));
        }

        public double DiffAngle()
        {
            return TargetAngle() - Position.Angle;
        }

        public double AngleWithNextPoint()
        {
            return AngleHelpers.ComputeAngle(ToSpeed(), ToCheckPoint());
        }
    }


  
}
