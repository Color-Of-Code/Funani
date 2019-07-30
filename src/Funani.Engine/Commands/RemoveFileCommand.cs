

using System;
using System.IO;
using System.IO.Abstractions;
using Funani.Api;

namespace Funani.Engine.Commands
{
    /// <summary>
    ///     Description of RemoveFileCommand.
    /// </summary>
    public class RemoveFileCommand : ActionCommand
    {
        public RemoveFileCommand(IEngine engine, IFileInfo file)
            : base(() => engine.RemoveFile(file))
        {
            Description = String.Format("Removing file '{0}'", file.FullName);
        }
    }
}