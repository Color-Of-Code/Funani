

using System;
using System.IO;
using Funani.Api;

namespace Funani.Engine.Commands
{
    /// <summary>
    ///     Description of AddFileCommand.
    /// </summary>
    public class AddFileCommand : ActionCommand
    {
        public AddFileCommand(IEngine engine, FileInfo file)
            : base(() => engine.AddFile(file))
        {
            Description = String.Format("Adding file '{0}'", file.FullName);
        }
    }
}