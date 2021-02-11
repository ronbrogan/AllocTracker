using CommandLine;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AllocTracker.Tool.Commands
{
    [Verb("launch")]
    public class LaunchCommand
    {
        [Option('t', "target", HelpText = "The process to launch", Required = true)]
        public string Executable { get; set; } = null!;

        [Option('a', "args", HelpText = "The arguments to send to the process")]
        public string? Arguments { get; set; }

        public async Task Run()
        {
            if (File.Exists(this.Executable) == false)
            {
                throw new Exception($"Unable to find executable '{this.Executable}'");
            }

            var targetProc = new ProfilerLauncher(this.Executable, this.Arguments)
                .Start();

            if(targetProc == null)
            {
                throw new Exception("Unable to launch process");
            }

            await targetProc.WaitForExitAsync();
        }
    }
}
