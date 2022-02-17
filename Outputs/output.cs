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
            var index = currentIndex == 0 ? r.Points.Count - 1 : currentIndex;
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
                var inputs = SystemHelpers.ReadLine(loglevel: LogLevel.Warning).Split(' ');
                Me.Position =  new PlayerPoint { X = int.Parse(inputs[0]), Y = int.Parse(inputs[1]), Distance = int.Parse(inputs[4]), Angle = int.Parse(inputs[5]) };
                var currentRacePoint = new RacePoint { X = int.Parse(inputs[2]), Y = int.Parse(inputs[3]), IsCurrent = false, Id = Race.Points.Count + 1};
                Race.Points.Add(currentRacePoint);
                inputs = SystemHelpers.ReadLine(loglevel: LogLevel.Warning).Split(' ');
                Opponent.Position = new PlayerPoint { X = int.Parse(inputs[0]), Y = int.Parse(inputs[1]) };

                // Set the current next point to current in race
                Race.Points.ToList().ForEach(x => x.IsCurrent = false);
                Race.Points.First(p => p.IsEquals(currentRacePoint)).IsCurrent = true;

                // This condition is for count the real new lap..
                if (nbPoint == Race.Points.Count && Race.NextPoint().Id == 1 && Race.Points.Count > 1 && _previousPoint?.Id != Race.NextPoint().Id)
                {
                    trace.AddText("New loop ");
                    Race.Lap++;
                    _previousPoint = Race.NextPoint();
                }

                trace.AddText($"Race with '{Race.Points.Count}' points - next is {Race.NextPoint()} {{ID: {Race.NextPoint().Id}}}'");
            }

        }
        public string Play()
        {
            using (var trace = new CustomTrace(nameof(GameManager.Play), endText: "Completed", logLevel: LogLevel.Warning))
            {
                Analyse();
                // Edit this line to output the target position
                // and thrust (0 <= thrust <= 100)
                // i.e.: "x y thrust"
                var next = Race.NextPoint();
                var thurst = Me.Position.Angle > 90 || Me.Position.Angle < -90 ? 0 : 100;

                var boost = Me.Boost > 0 
                    && Race.PreviousPoint() == _maxDistancePoint 
                    && Race.Lap >= 2 && thurst == 100 
                    && Me.Position.Distance < _maxDistancePoint.DistanceWithNextPoint / 1.5;

                var result = boost ? "BOOST" : thurst.ToString();
                if (result == "BOOST")
                    Me.Boost -= 1;

                trace.AddText($"Lap {Race.Lap} - Spell | Boost : {Me.Boost}");

                return $"{next.X} {next.Y} {result}";
            }
           

        }

        private void Analyse()
        {
            if(Race.Lap == 2)
            {
               for(var i = 0; i < Race.Points.Count; i++)
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
