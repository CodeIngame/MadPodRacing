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
