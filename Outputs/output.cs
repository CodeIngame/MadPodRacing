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
namespace MadPodRacing.Domain.Common
{
    using System;
    using System.Diagnostics;
    using MadPodRacing.Domain.Helpers;

    public class CustomTrace
        : IDisposable
    {
        private Stopwatch Sw;
        private string Method;
        private string Start;
        private string End;
        private string Text;
        private LogLevel LogLevel;


        public CustomTrace(string method, string startText = "", string endText = "", LogLevel logLevel = LogLevel.Warning)
        {
            Method = method;
            Start = startText;
            End = endText;
            LogLevel = logLevel;
            Sw = new Stopwatch();
            Sw.Start();
            if (!string.IsNullOrEmpty(startText))
                SystemHelpers.WriteLineLog($"[{Method}]({Start})", logLevel);
        }

        public void AddText(string text)
        {
            Text += text;
        }

        public void ChangeLogLevel(LogLevel loglevel)
        {
            LogLevel = loglevel;
        }

        public void WriteLog(string msg)
        {
            SystemHelpers.WriteLineLog($"[{Method}] - {msg}", LogLevel);
        }

        public void Dispose()
        {
            Sw.Stop();
            var isMs = Sw.Elapsed.TotalMilliseconds < 1000;
            var unit = isMs ? "ms" : "s";
            var value = isMs ? Sw.Elapsed.TotalMilliseconds : Sw.Elapsed.TotalSeconds;
            var withMoreText = string.IsNullOrEmpty(Text) ? string.Empty : $" - {Text}";
            if (!string.IsNullOrEmpty(withMoreText))
                SystemHelpers.WriteLineLog($"[{Method}] ({withMoreText})", LogLevel);

            if (!string.IsNullOrEmpty(End))
                SystemHelpers.WriteLineLog($"[{Method}]({End}) took [{value}{unit}]", LogLevel);
        }
    }
}
namespace MadPodRacing.Domain.Common
{
    public enum LogLevel
    {
        Verbose = 0,
        Debug,
        Information,
        Warning,
        Critical
    }

    /// <summary>
    /// La direction
    /// Gauche +
    /// Droite -
    /// </summary>
    public enum Direction
    {
        Left90,
        Left60,
        Left45,
        Left30,
        Front0,
        Right30,
        Right45,
        Right60,
        Right90,
    }
}

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
namespace MadPodRacing.Domain.Entities
{
    using MadPodRacing.Domain.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;

    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point()
        {
        }

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public override string ToString()
        {
            return $"[{X}:{Y}]";
        }
    }

    public class PlayerPoint
        : Point
    {
        public PlayerPoint()
        {
        }

        public PlayerPoint(int x, int y) : base(x, y)
        {

        }

        /// <summary>
        /// distance to the next checkpoint
        /// </summary>
        public int Distance { get; set; }
        /// <summary>
        /// angle between your pod orientation and the direction of the next checkpoint
        /// </summary>
        public int Angle { get; set; }
    }

    public class CheckPoint
        : Point
    {

        public CheckPoint()
        {

        }

        public CheckPoint(int x, int y) : base(x, y)
        {

        }

        public bool IsCurrent { get; set; }
        public int Id { get; set; }

        /// <summary>
        /// Computed X
        /// </summary>
        public int NewX { get; set; }
        /// <summary>
        /// Computed Y
        /// </summary>
        public int NewY { get; set; }

    }
}
namespace MadPodRacing.Domain.Entities
{
    using MadPodRacing.Domain.Helpers;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    public class Race
    {
        public Race()
        {
            CheckPoint = new List<CheckPoint>();
        }

        public List<CheckPoint> CheckPoint { get; set; }

        /// <summary>
        /// Nombre de tours
        /// </summary>
        public int Lap { get; set; } = 1;
    }

    public class RacePointComparer : IEqualityComparer<CheckPoint>
    {
        public bool Equals([AllowNull] CheckPoint p1, [AllowNull] CheckPoint p2)
        {
            return p1.IsEquals(p2);
        }

        public int GetHashCode([DisallowNull] CheckPoint obj)
        {
            return obj.X ^ obj.Y;

        }
    }
}
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
namespace MadPodRacing.Domain.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public static class DegreesHelpers
    {

        public static double ToRadian(this float r)
        {
            return r * Math.PI / 180;
        }

        public static double ToRadian(this double r)
        {
            return r * Math.PI / 180;
        }

        public static double ToRadian(this int r)
        {
            return r * Math.PI / 180;
        }

        public static double ToDegree(this double d)
        {
            return d * 180 / Math.PI;
        }

        public static double ToDegree(this int d)
        {
            return d * 180 / Math.PI;
        }

        public static double ToDegree(this float d)
        {
            return d * 180 / Math.PI;
        }
    }
}


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


namespace MadPodRacing.Domain.Helpers
{
    using MadPodRacing.Domain.Entities;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public static class PointHelpers
    {
        public static bool IsEquals(this Point p1, Point p2)
        {
            return (p1.X == p2.X && p1.Y == p2.Y);
        }

        public static double ToManhattanDistance(this Point p1, Point p2)
        {
            return Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);
        }

        public static double ToChebyshevDistance(this Point p1, Point p2)
        {
            return Math.Max(Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y));
        }

        public static double ToEuclideDistance(this Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        public static double ToPythagore(this Point p1)
        {
            return Math.Sqrt(Math.Pow(p1.X, 2) + Math.Pow(p1.Y, 2));
        }

        public static double ToPythagore(this int[] p1)
        {
            if (p1.Length == 0 || p1.Length > 2) throw new ArgumentException($"{nameof(p1)} length is bad");
            return Math.Sqrt(Math.Pow(p1[0], 2) + Math.Pow(p1[1], 2));
        }
    }
}


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
namespace MadPodRacing.Domain.Helpers
{
    using System;
    using MadPodRacing.Domain.Common;

    public class SystemHelpers
    {
        /// <summary>
        /// Permet de lire la ligne et de retourner son contenu
        /// Log si nécessaire l'information lue
        /// </summary>
        /// <param name="newLine"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static string ReadLine(bool newLine = true, bool debug = true, LogLevel loglevel = LogLevel.Information)
        {
            var msg = Console.ReadLine();
            if (debug)
            {
                if (newLine)
                    WriteLineLog($"\"{msg}\"", loglevel);
                else
                    WriteLog($"\"{msg}\",", loglevel);
            }

            return msg;
        }

        /// <summary>
        /// Write a new line of log
        /// </summary>
        /// <param name="log"></param>
        /// <param name="logLevel"></param>
        public static void WriteLineLog(string log, LogLevel logLevel = LogLevel.Verbose)
        {
            Write(log, logLevel, true);
        }

        /// <summary>
        /// Write log on same line
        /// </summary>
        /// <param name="log"></param>
        /// <param name="logLevel"></param>
        public static void WriteLog(string log, LogLevel logLevel = LogLevel.Verbose)
        {
            Write(log, logLevel, false);
        }

        private static void Write(string log, LogLevel logLevel = LogLevel.Verbose, bool newLine = false)
        {
            if (logLevel >= LogLevel.Warning)
            {
                if (newLine)
                    Console.Error.WriteLine(log);
                else
                    Console.Error.Write(log);
            }
        }
    }
}
namespace MadPodRacing.Domain.Interface
{
    public interface IGameManager
    {
        void Initialize();
        void ReadTurn();
        string Play();
    }
}
namespace MadPodRacing.Domain.Manager
{
    using MadPodRacing.Domain.Common;
    using MadPodRacing.Domain.Entities;
    using MadPodRacing.Domain.Helpers;
    using MadPodRacing.Domain.Interface;
    using System;
    using System.Linq;
    using System.Numerics;

    public class GameManager
        : IGameManager
    {

        public Player Me { get; set; }
        public Player Opponent { get; set; }
        public Race Race { get; set; }

        private CheckPoint _maxDistancePoint;
        private CheckPoint _previousPoint;
        private int _tick = 1;
        private bool _goNextCheckPoint = false;

        public GameManager()
        {
            Race = new Race();
        }

        public void Initialize()
        {
            Me = new Player { };
            Opponent = new Player { };
        }

        public void ReadTurn()
        {
            using (var trace = new CustomTrace(nameof(GameManager.ReadTurn), endText: "Completed", logLevel: LogLevel.Warning))
            {
                _goNextCheckPoint = false;
                var nbPoint = Race.CheckPoint.Count;
                Me.PreviousPosition = Me.Position;
                Opponent.PreviousPosition = Opponent.Position;


                var inputs = SystemHelpers.ReadLine(loglevel: LogLevel.Verbose).Split(' ');
                Me.Position =  new PlayerPoint { X = int.Parse(inputs[0]), Y = int.Parse(inputs[1]), Distance = int.Parse(inputs[4]), Angle = int.Parse(inputs[5]) };
                var currentRacePoint = new CheckPoint { X = int.Parse(inputs[2]), Y = int.Parse(inputs[3]), IsCurrent = false, Id = Race.CheckPoint.Count + 1};
                var addedCheckPoint = Race.CheckPoint.TryAdd(currentRacePoint);

                inputs = SystemHelpers.ReadLine(loglevel: LogLevel.Verbose).Split(' ');
                Opponent.Position = new PlayerPoint { X = int.Parse(inputs[0]), Y = int.Parse(inputs[1]) };

                // Set the current next point to current in race
                Race.CheckPoint.ToList().ForEach(x => x.IsCurrent = false);
                Race.CheckPoint.First(p => p.IsEquals(currentRacePoint)).IsCurrent = true;
                Me.NextPoint = Race.NextPoint();


                Compute();

                // This condition is for count the real new lap..
                if (!addedCheckPoint && Me.NextPoint.Id == 1 && Race.CheckPoint.Count > 1 && _previousPoint?.Id != Me.NextPoint.Id)
                {
                    trace.AddText("New loop ");
                    Race.Lap++;
                    _previousPoint = Me.NextPoint;
                }

                Analyse();
                _tick += 1;

                trace.AddText($"Total point '{Race.CheckPoint.Count}' -> {{ID: {Race.NextPoint().Id}}}'");

            }

        }
        public string Play()
        {
            using (var trace = new CustomTrace(nameof(GameManager.Play), endText: "Completed", logLevel: LogLevel.Warning))
            {
                
                var next = Me.NextPoint;
                var thurst = Me.Power > 100 ? "BOOST" : $"{Me.Power}";


                trace.AddText($"Lap {Race.Lap} - Boost : {Me.Boost} - Angle {Me.Position.Angle} - Distance {Me.Position.Distance}");
                if(_goNextCheckPoint)
                    return $"{next.X} {next.Y} {thurst}";
                return $"{next.NewX} {next.NewY} {thurst}";
            }
           

        }

        private void Compute()
        {
            using (var trace = new CustomTrace(nameof(GameManager.Compute), endText: "Completed", logLevel: LogLevel.Verbose))
            {
                ComputeSpeed();
                ComputePath();
            }
        }

        private void ComputeSpeed()
        {
            using (var trace = new CustomTrace(nameof(GameManager.ComputeSpeed), endText: "Completed", logLevel: LogLevel.Warning))
            {
                // On est à l'envers
                if (Me.Position.Angle > 90 || Me.Position.Angle < -90)
                {
                    trace.AddText("A l'envers");
                    Me.Power = Constantes.HALF_POWER;
                    return;
                }

                // On est loin, on a du boost et dans la bonne direction
                if(Me.ComputeDistanceNextCheckPoint() > Constantes.CHECKPOINT_DISTANCE_IS_FAR 
                    && Math.Abs(Me.Position.Angle) < Constantes.SAFE_ANGLE_BOOST && Me.Boost > 0)
                {
                    trace.AddText("Fonce!");
                    Me.Power = 101;
                    Me.Boost -= 1;
                    return;
                }

                // On a pas utiliser le boost, l'angle est bon, on est au dernier tours sur le dernier point
                if(Me.Position.Angle < Constantes.SAFE_ANGLE_BOOST && Race.Lap == 3 && Me.NextPoint.Id == Race.CheckPoint.Count - 1)
                {
                    if (Me.Boost > 0)
                    {
                        trace.AddText("Vite!");
                        Me.Power = 102;
                        Me.Boost -= 1;
                    } 
                    else
                    {
                        trace.AddText("100!");
                        Me.Power = Constantes.MAX_POWER;
                    }

                    return;
                }

                // On est proche du point, on arrive vite, on va ralentir en ciblant le prochain point
                if(Me.Position.Distance < Constantes.CHECKPOINT_DISTANCE_MOVE_NEXT 
                    && Me.ComputeSpeed() > Constantes.SPEED_FAST
                    && Me.AngleWithNextPoint() < Me.LimitAngle() && Race.Lap > 1
                    )
                {
                    trace.AddText("Freine!");
                    Me.Power = Constantes.STOP_POWER;
                    Race.SetNextPointToCurrent();
                    _goNextCheckPoint = true;
                    return;
                }

                // On est proche, on ralenti
                if(Me.Position.Distance < Constantes.MIN_DISTANCE 
                    && Me.ComputeSpeed() > Constantes.SPEED_FAST)
                {
                    trace.AddText("Ralenti!");
                    Me.Power = Constantes.SLOW_POWER;
                    return;
                }

                // Calcul de gauss à partir de l'angle
                trace.AddText("Gauss!");
                var gauss = Constantes.THRUST_AMPLITUDE * Math.Exp(-Math.Pow(Me.Position.Angle, 2) / Constantes.THRUST_DEVIATION);
                Me.Power = Math.Min(100, (int)gauss);
            }
        }

        private void ComputePath()
        {
            using (var trace = new CustomTrace(nameof(GameManager.ComputePath), endText: "Completed"))
            {
                int[] mapCenter = { Constantes.MAP_SIZE_X / 2, Constantes.MAP_SIZE_Y / 2 };

                // L'idée est de viser le checkpoint coté centre de la carte
                var next = Race.NextPoint();
                int[] checkPoint = { next.X - mapCenter[0], next.Y - mapCenter[1] };
                var checkPointNorm = checkPoint.ToPythagore();
                int[] checkPointUnit = { (int)(checkPoint[0] / checkPointNorm), (int)(checkPoint[1] / checkPointNorm) };

                next.NewX = next.X - (checkPointUnit[0] * Constantes.CHECKPOINT_RADIUS);
                next.NewY = next.Y - (checkPointUnit[1] * Constantes.CHECKPOINT_RADIUS);

                var toCheckPoint = new Vector2(next.X - Me.Position.X, next.Y - Me.Position.Y);
                var theta = Me.DiffAngle().ToRadian();
                var cos = (float)Math.Cos(theta);
                var sin = (float)Math.Sin(theta);
                var r = new Vector2[] { new Vector2( cos, -sin ), new Vector2(sin, cos) };
                var toTargetPoint = new Vector2(Vector2.Dot(r[0], toCheckPoint), Vector2.Dot(r[1], toCheckPoint));

                next.NewX = (int)toTargetPoint.X + Me.Position.X;
                next.NewY = (int)toTargetPoint.Y + Me.Position.Y;

            }
        }

        private void Analyse()
        {
            using (var trace = new CustomTrace(nameof(GameManager.Analyse), endText: "Completed", logLevel: LogLevel.Verbose))
            {
                var distanceWithNextPoint = Me.ComputeDistanceNextCheckPoint();
                var speed = Me.ComputeSpeed();
                var toCheckPoint = Me.ToCheckPoint();
                var angleWithNextPoint = Me.AngleWithNextPoint();
                var limitAngle = Me.LimitAngle();
                var targetAngle = Me.TargetAngle();
                var diff = Me.DiffAngle();
            }
        }
    }
}
namespace MadPodRacing.Domain
{
    using MadPodRacing.Domain.Common;
    using MadPodRacing.Domain.Interface;
    using MadPodRacing.Domain.Manager;
    using System;

    internal class Program
    {
        static void Main(string[] args)
        {
            using (var trace = new CustomTrace(nameof(Main)))
            {
                IGameManager gm = new GameManager();
                gm.Initialize();
                // game loop
                while (true)
                {
                    gm.ReadTurn();
                    Console.WriteLine(gm.Play());
                }
            }
        }
    }
}
