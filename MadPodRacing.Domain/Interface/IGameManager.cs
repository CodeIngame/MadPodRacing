namespace MadPodRacing.Domain.Interface
{
    public interface IGameManager
    {
        void Initialize();
        void ReadTurn();
        string Play();
    }
}
