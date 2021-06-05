namespace Funani.Core
{
    using CommandLine;

    public static class Program
    {
        private class Options
        {
            [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
            public bool Verbose { get; set; }
        }

        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       Run(o);
                   });
        }

        private static void Run(Options o)
        {
        }
    }
}
