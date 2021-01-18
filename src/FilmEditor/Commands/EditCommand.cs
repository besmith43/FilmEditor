using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Exceptions;
using FFMpegCore.Helpers;
using FFMpegCore.Pipes;
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace FilmEditor.Commands
{
    public class EditCommand : ICommand
    {
        public const string COMMAND_NAME = "edit";
        public string outputFile;
        public string editTempDirectory;
        public string csvFile;
        public string[] csvContent;
        public List<string> tempClips;

        public EditCommand(string[] _args)
        {
            tempClips = new();
            ParseArgs(_args);
            setEditTempDirectory();
            readInCSV();
        }

        public Task<bool> ExecuteAsync()
        {
            return Task.Run(() => {

                if (Program.cmdFlags.helpFlag)
                {
                    helpScreen();
                    return false;
                }

                if (string.IsNullOrEmpty(csvContent[0]))
                {
                    return false;
                }

                if (string.IsNullOrEmpty(outputFile))
                {
                    Console.WriteLine("What would you like to call the output file?");
                    outputFile = Console.ReadLine();
                }

                for(int i = 1; i < csvContent.Length; i++)
                {
                    ParseCSVContent(csvContent[i]);
                }

                Concat();

                return true;
            });
        }

        private void ParseArgs(string[] args)
        {
            for(int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-c" || args[i] == "--csv")
                {
                    csvFile = args[i+1];
                }
                else if (args[i] == "-o" || args[i] == "--output")
                {
                    outputFile = args[i+1];
                }
            }
        }

        private void readInCSV()
        {
            if (File.Exists(csvFile))
            {
                csvContent = File.ReadAllLines(csvFile);
            }
        }

        private void setEditTempDirectory()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                editTempDirectory = $"C:\\Users\\{ Environment.UserName }\\AppData\\Local\\Temp\\FilmEditor";
            }
            else
            {
                editTempDirectory = "/tmp/FilmEditor";
            }

            Directory.CreateDirectory(editTempDirectory);
        }

        private void ParseCSVContent(string line)
        {
            string[] content = line.Split(',');

            var vidAnalysis = FFProbe.Analyse(content[0]);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Split(vidAnalysis, Int32.Parse(content[1]), Int32.Parse(content[2]), $"{ editTempDirectory }\\{ content[3] }");

                tempClips.Add($"{ editTempDirectory }\\{ content[3] }");
            }
            else
            {
                Split(vidAnalysis, Int32.Parse(content[1]), Int32.Parse(content[2]), $"{ editTempDirectory }/{ content[3] }");

                tempClips.Add($"{ editTempDirectory }/{ content[3] }");
            }
        }

        private void Split(IMediaAnalysis source, int startSeconds, int stopSeconds, string output)
		{
			FFMpegArguments
				.FromFileInput(source)
				.OutputToFile(output, true, options => options
					.Seek(TimeSpan.FromSeconds((double)startSeconds))
					.WithCustomArgument($"-to { stopSeconds }")
					.ForceFormat("mp4"))
				.ProcessSynchronously();
		}

        private void Concat()
        {
            FFMpeg.Join(outputFile, tempClips.ToArray());
        }

        private void helpScreen()
        {
            StringBuilder helpScreenText = new();

            helpScreenText.AppendLine($"FilmEditor { Program.cmdFlags.versionText }");
            helpScreenText.AppendLine("Usage:  FilmEditor edit [OPTION]  ");
            helpScreenText.AppendLine("");
            helpScreenText.AppendLine("    -c | --csv <csv file>            Pass in csv containing edit information");
            helpScreenText.AppendLine("    -o | --output <destionation folder>  (Optional) Set Destionation Folder");
            helpScreenText.AppendLine("");

            Console.WriteLine(helpScreenText.ToString());
        }
    }
}