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
