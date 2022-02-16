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
