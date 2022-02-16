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
