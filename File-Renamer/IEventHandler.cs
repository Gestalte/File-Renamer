namespace File_Renamer
{
    public interface IEventHandler<TEvent>
    {
        void Handle(TEvent e);
    }
}
