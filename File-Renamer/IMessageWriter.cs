namespace File_Renamer
{
    public interface IMessageWriter
    {
        void WriteMessage(string message);

        void WriteColorMessage(string message, MessageType color);
    }
}
