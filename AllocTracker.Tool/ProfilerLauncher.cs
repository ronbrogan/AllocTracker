using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace AllocTracker.Tool
{
    public class ProfilerLauncher
    {
        private const string EnvVar_ProfileEnable = "CORECLR_ENABLE_PROFILING";
        private const string EnvVar_Profiler = "CORECLR_PROFILER";
        private const string EnvVar_ProfilerDllPath64 = "CORECLR_PROFILER_PATH_64";
        private const string EnvVar_ProfilerDllPath32 = "CORECLR_PROFILER_PATH_32";
        private const string EnvVar_LogDirectory = "AllocTracker_LOGDIR";

        private const string ProfilerPathFormat = @"Profilers\{0}\{1}\AllocTracker.Profiler.dll";

        private readonly string executable;
        private readonly string arguments;
        private string workingDirectory;
        private Dictionary<string, string> environmentVariables = new ();

        private Process? process = null;

        public ProfilerLauncher(string executable, string? args = null)
        {
            this.executable = executable ?? throw new ArgumentNullException(nameof(executable));
            this.arguments = args ?? string.Empty;
            this.workingDirectory = Path.GetDirectoryName(executable) ?? Directory.GetCurrentDirectory();
        }

        public ProfilerLauncher AddEnvironmentVariable(string key, string value)
        {
            this.environmentVariables[key] = value;
            return this;
        }

        public ProfilerLauncher WithEnvironmentVariables(Dictionary<string, string> newValues)
        {
            this.environmentVariables = newValues;
            return this;
        }

        public ProfilerLauncher WithWorkingDirectory(string directory)
        {
            this.workingDirectory = directory;
            return this;
        }

        public Process? Start()
        {
            var startInfo = new ProcessStartInfo(this.executable, this.arguments)
            {
                UseShellExecute = false,
                WorkingDirectory = this.workingDirectory
            };

            foreach (var (key,value) in this.environmentVariables)
            {
                startInfo.EnvironmentVariables[key] = value;
            }

            startInfo.EnvironmentVariables[EnvVar_ProfileEnable] = "1";
            startInfo.EnvironmentVariables[EnvVar_Profiler] = $"{{{ProfilerInfo.Id}}}";
            startInfo.EnvironmentVariables[EnvVar_ProfilerDllPath64] = 
                Path.Combine(Directory.GetCurrentDirectory(), string.Format(ProfilerPathFormat, "Windows", "x64"));
            startInfo.EnvironmentVariables[EnvVar_ProfilerDllPath32] =
                Path.Combine(Directory.GetCurrentDirectory(), string.Format(ProfilerPathFormat, "Windows", "x86"));
            startInfo.EnvironmentVariables[EnvVar_LogDirectory] = this.workingDirectory;

            this.process = Process.Start(startInfo);

            return this.process;
        }
    }
}
