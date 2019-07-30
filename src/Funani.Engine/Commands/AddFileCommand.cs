

using System;
using System.IO.Abstractions;
using Funani.Api;

namespace Funani.Engine.Commands
{
    /// <summary>
    ///     Description of AddFileCommand.
    /// </summary>
    public class AddFileCommand : ActionCommand
    {
        public AddFileCommand(IEngine engine, IFileInfo file)
            : base(() => engine.AddFile(file))
        {
            Description = String.Format("Adding file '{0}'", file.FullName);
        }
    }
}