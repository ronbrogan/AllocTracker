using AllocTracker.Tool.Commands;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AllocTracker.Tool
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Task task = Parser.Default.ParseArguments<LaunchCommand>(args)
                .MapResult(
                    async (LaunchCommand c) => await c.Run(),
                    async (IEnumerable<Error> e) => await Help(e)
                );

            await task;
        }

        static async Task Help(IEnumerable<Error> errors)
        {
            foreach (var e in errors) Console.WriteLine(e.ToString());
        }
    }
}
