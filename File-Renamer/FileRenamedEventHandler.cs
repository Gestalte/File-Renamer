using System;
using System.IO;

namespace File_Renamer
{
    public class FileRenamedEventHandler : IEventHandler<FileRenamed>
    {
        private readonly IMessageWriter messageWriter;

        public FileRenamedEventHandler(IMessageWriter messageWriter)
        {
            this.messageWriter = messageWriter ?? throw new ArgumentNullException(nameof(messageWriter));
        }

        public void Handle(FileRenamed e)
        {
            messageWriter.WriteColorMessage(Path.GetFileName(e.OldName), MessageType.incorrect);
            messageWriter.WriteColorMessage("converted to:", MessageType.information);
            messageWriter.WriteColorMessage(Path.GetFileName(e.NewName), MessageType.correct);
            messageWriter.WriteMessage("");
        }
    }
}
