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
            helpTextBuilder.AppendLine("Usage:  FilmEditor <Command> [OPTION]  ");
            helpTextBuilder.AppendLine("");
            helpTextBuilder.AppendLine("Commands");
            helpTextBuilder.AppendLine("new         Create new csv in order to edit or split an mp4 file");
            helpTextBuilder.AppendLine("edit        Use given csv to edit out sections and join the pieces into a single smaller mp4 file");
            helpTextBuilder.AppendLine("split       Use given csv to slice an mp4 into smaller episodes");
            helpTextBuilder.AppendLine("rename      Use source directory to mass rename mp4 episodes");
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