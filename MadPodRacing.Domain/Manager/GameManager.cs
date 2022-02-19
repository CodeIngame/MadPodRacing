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
