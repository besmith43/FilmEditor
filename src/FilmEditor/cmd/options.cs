using System;
using System.Text;
using System.Reflection;
using FilmEditor.Commands;

#nullable enable

namespace FilmEditor.cmd
{
    public class Options
    {
        public bool helpFlag { get; set; }
        public string helpText { get; set; }
        public bool versionFlag { get; set; }
        public string versionText { get; set; }
        public bool verbose { get; set; }
        public ICommand? command { get; set; }

        public Options()
        {
            helpFlag = false;
            versionFlag = false;
            versionText = $"FilmEditor { Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version }";
            verbose = false;
            StringBuilder helpTextBuilder = new StringBuilder();
            helpTextBuilder.AppendLine($"FilmEditor { versionText }");
            helpTextBuilder.AppendLine("Usage:  FilmEditor [OPTION]  ");
            helpTextBuilder.AppendLine("");
            helpTextBuilder.AppendLine("    -V | --verbose          Set verbose mode");
            helpTextBuilder.AppendLine("    -v | --version          Display version message");
            helpTextBuilder.AppendLine("    -h | --help             Display this help message");
            helpTextBuilder.AppendLine("");
            helpText = helpTextBuilder.ToString();

            command = null;
        }
    }
}