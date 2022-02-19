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
