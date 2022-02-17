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

        public GameManager()
        {
            Race = new Race();
        }

        public void Initialize()
        {
           
        }

        public void ReadTurn()
        {
            using (var trace = new CustomTrace(nameof(GameManager.ReadTurn), endText: "Completed", logLevel: LogLevel.Warning))
            {
                var inputs = SystemHelpers.ReadLine(loglevel: LogLevel.Warning).Split(' ');
                Me = new Player { Position = new PlayerPoint { X = int.Parse(inputs[0]), Y = int.Parse(inputs[1]), Distance = int.Parse(inputs[4]), Angle = int.Parse(inputs[5]) } };
                var currentRacePoint = new RacePoint { X = int.Parse(inputs[2]), Y = int.Parse(inputs[3]), IsCurrent = false };
                Race.Points.Add(currentRacePoint);
                inputs = SystemHelpers.ReadLine(loglevel: LogLevel.Warning).Split(' ');
                Opponent = new Player { Position = new PlayerPoint { X = int.Parse(inputs[0]), Y = int.Parse(inputs[1]) } };

                // Set the current next point to current in race
                Race.Points.ToList().ForEach(x => x.IsCurrent = false);
                Race.Points.First(p => p.IsEquals(currentRacePoint)).IsCurrent = true;

                trace.AddText($"Race with '{Race.Points.Count}' points - next is {Race.NextPoint()}'");
            }

        }
        public string Play()
        {
            using (var trace = new CustomTrace(nameof(GameManager.Play), endText: "Completed", logLevel: LogLevel.Warning))
            {


                // Edit this line to output the target position
                // and thrust (0 <= thrust <= 100)
                // i.e.: "x y thrust"
                var next = Race.NextPoint();
                var thurst = Me.Position.Angle > 90 || Me.Position.Angle < -90 ? 0 : 100;
                return $"{next.X} {next.Y} {thurst}";
            }
           

        }
    }
}
