using System;

namespace File_Renamer
{
    public class ConsoleWriter : IMessageWriter
    {
        public void WriteColorMessage(string message, MessageType type)
        {
            switch (type)
            {
                case MessageType.correct:
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;
                case MessageType.incorrect:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                case MessageType.success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case MessageType.failure:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case MessageType.information:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                default:
                    break;
            }

            Console.WriteLine(message);
            Console.ResetColor();
        }

        public void WriteMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
