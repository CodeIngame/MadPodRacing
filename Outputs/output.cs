namespace MadPodRacing.Domain.Common
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public static class Constantes
    {
        public static int MAX_POWER = 100;
        public static int HALF_POWER = 50;
        public static int MIN_DISTANCE = 2500;
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

        public override string ToString()
        {
            return $"[{X}:{Y}]";
        }
    }

    public class PlayerPoint
        : Point
    {

        /// <summary>
        /// distance to the next checkpoint
        /// </summary>
        public int Distance { get; set; }
        /// <summary>
        /// angle between your pod orientation and the direction of the next checkpoint
        /// </summary>
        public int Angle { get; set; }
    }

    public class RacePoint
        : Point
    {
        public bool IsCurrent { get; set; }
        public int Id { get; set; }
        public int DistanceWithNextPoint { get; set; }

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
            Points = new HashSet<RacePoint>(new RacePointComparer());
        }

        public HashSet<RacePoint> Points { get; set; }
        /// <summary>
        /// Nombre de tours
        /// </summary>
        public int Lap { get; set; } = 1;
    }

    public class RacePointComparer : IEqualityComparer<RacePoint>
    {
        public bool Equals([AllowNull] RacePoint p1, [AllowNull] RacePoint p2)
        {
            return p1.IsEquals(p2);
        }

        public int GetHashCode([DisallowNull] RacePoint obj)
        {
            return obj.X ^ obj.Y;

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
        public static RacePoint NextPoint(this Race r)
        {
            return r.Points.First(p => p.IsCurrent);
        }

        public static RacePoint PreviousPoint(this Race r)
        {
            var currentIndex = r.Points.ToList().IndexOf(r.NextPoint());
            var index = currentIndex == 0 ? r.Points.Count - 1 : currentIndex - 1;
            return r.Points.ToList()[index];
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

    public class GameManager
        : IGameManager
    {

        public Player Me { get; set; }
        public Player Opponent { get; set; }
        public Race Race { get; set; }

        private RacePoint _maxDistancePoint;
        private RacePoint _previousPoint;

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
                var nbPoint = Race.Points.Count;
                var inputs = SystemHelpers.ReadLine(loglevel: LogLevel.Verbose).Split(' ');
                Me.Position =  new PlayerPoint { X = int.Parse(inputs[0]), Y = int.Parse(inputs[1]), Distance = int.Parse(inputs[4]), Angle = int.Parse(inputs[5]) };
                var currentRacePoint = new RacePoint { X = int.Parse(inputs[2]), Y = int.Parse(inputs[3]), IsCurrent = false, Id = Race.Points.Count + 1};
                Race.Points.Add(currentRacePoint);
                inputs = SystemHelpers.ReadLine(loglevel: LogLevel.Verbose).Split(' ');
                Opponent.Position = new PlayerPoint { X = int.Parse(inputs[0]), Y = int.Parse(inputs[1]) };

                // Set the current next point to current in race
                Race.Points.ToList().ForEach(x => x.IsCurrent = false);
                Race.Points.First(p => p.IsEquals(currentRacePoint)).IsCurrent = true;


                Compute();

                // This condition is for count the real new lap..
                if (nbPoint == Race.Points.Count && Me.NextPoint.Id == 1 && Race.Points.Count > 1 && _previousPoint?.Id != Me.NextPoint.Id)
                {
                    trace.AddText("New loop ");
                    Race.Lap++;
                    _previousPoint = Me.NextPoint;
                }
                trace.AddText($"Race with '{Race.Points.Count}' points - next is {Race.NextPoint()} {{ID: {Race.NextPoint().Id}}}'");

                Analyse();
            }

        }
        public string Play()
        {
            using (var trace = new CustomTrace(nameof(GameManager.Play), endText: "Completed", logLevel: LogLevel.Warning))
            {
                // Edit this line to output the target position
                // and thrust (0 <= thrust <= 100)
                // i.e.: "x y thrust"
                var next = Me.NextPoint;
                //var thurst = Me.Position.Angle > 90 || Me.Position.Angle < -90 ? 0 : 100;
                var thurst = Me.Power;

                var boost = Me.Boost > 0 
                    && Me.PreviousPoint == _maxDistancePoint 
                    && Race.Lap >= 2 && thurst == Constantes.MAX_POWER
                    && Me.Position.Distance > _maxDistancePoint.DistanceWithNextPoint / 2;

                var result = boost ? "BOOST" : thurst.ToString();
                if (result == "BOOST")
                    Me.Boost -= 1;

                trace.AddText($"Previous point : {Me.PreviousPoint?.Id} - ");
                trace.AddText($"Best distance start at point : {_maxDistancePoint?.Id} - dist :  {_maxDistancePoint?.DistanceWithNextPoint} - ");
                trace.AddText($"Lap {Race.Lap} - Boost : {Me.Boost} - Angle {Me.Position.Angle} - Distance {Me.Position.Distance}");

                return $"{next.X} {next.Y} {result}";
            }
           

        }

        private void Compute()
        {
            using (var trace = new CustomTrace(nameof(GameManager.Compute), endText: "Completed", logLevel: LogLevel.Verbose))
            {
                Me.NextPoint = Race.NextPoint();
                Me.PreviousPoint = Race.PreviousPoint();

                if(Me.Position.Angle > 90 || Me.Position.Angle < -90)
                {
                    Me.Power = 0;
                    return;
                }

                Me.Power = Me.Position.Distance > Constantes.MIN_DISTANCE * 2
                    ? 100
                    : (int)(Math.Abs(Math.Cos(Me.Position.Angle.ToRadian()) * 100));

                if (Me.Power >= 90)
                    Me.Power = Constantes.MAX_POWER;

                if (Me.Position.Distance <= Constantes.MIN_DISTANCE && Me.Power > Constantes.HALF_POWER)
                    Me.Power /= 2;

            }
        }

        private void Analyse()
        {
            using (var trace = new CustomTrace(nameof(GameManager.Analyse), endText: "Completed", logLevel: LogLevel.Verbose))
            {
                if (Race.Lap == 2)
                {
                    for (var i = 0; i < Race.Points.Count; i++)
                    {
                        var current = Race.Points.ToList()[i];
                        RacePoint next;
                        if (i == Race.Points.Count - 1)
                            next = Race.Points.ToList()[0];
                        else
                            next = Race.Points.ToList()[i + 1];

                        current.DistanceWithNextPoint = (int)current.ToManhattanDistance(next);

                    }

                    _maxDistancePoint = Race.Points.OrderByDescending(p => p.DistanceWithNextPoint).First();
                }
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
