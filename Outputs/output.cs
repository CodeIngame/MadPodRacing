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
    using MadPodRacing.Domain.Interface;
    using System;
    using System.Collections.Generic;
    using System.Text;


    public class GameManager
        : IGameManager
    {

        public void Initialize()
        {
           
        }

        public void ReadTurn()
        {
            using (var trace = new CustomTrace(nameof(GameManager.ReadTurn), endText: "Completed", logLevel: LogLevel.Warning))
            {
               
            }

        }
        public string Play()
        {
            var inputs = Console.ReadLine().Split(' ');
            int x = int.Parse(inputs[0]);
            int y = int.Parse(inputs[1]);
            int nextCheckpointX = int.Parse(inputs[2]); // x position of the next check point
            int nextCheckpointY = int.Parse(inputs[3]); // y position of the next check point
            int nextCheckpointDist = int.Parse(inputs[4]); // distance to the next checkpoint
            int nextCheckpointAngle = int.Parse(inputs[5]); // angle between your pod orientation and the direction of the next checkpoint
            inputs = Console.ReadLine().Split(' ');
            int opponentX = int.Parse(inputs[0]);
            int opponentY = int.Parse(inputs[1]);

            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");


            // Edit this line to output the target position
            // and thrust (0 <= thrust <= 100)
            // i.e.: "x y thrust"
            return $"{nextCheckpointX} {nextCheckpointY} 80";

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
