using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Exceptions;
using FFMpegCore.Helpers;
using FFMpegCore.Pipes;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FilmEditor.Commands
{
    public class SplitCommand : ICommand
    {
        public const string COMMAND_NAME = "split";
        public string csvFile;
        public string[] csvContent;

        public SplitCommand(string[] _args)
        {
            ParseArgs(_args);
            readInCSV();
        }

        public Task<bool> ExecuteAsync()
        {
            return Task.Run(() => {
                if (string.IsNullOrEmpty(csvContent[0]))
                {
                    return false;
                }

                for(int i = 1; i < csvContent.Length; i++)
                {
                    ParseCSVContent(csvContent[i]);
                }

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
            }
        }

        private void readInCSV()
        {
            if (File.Exists(csvFile))
            {
                csvContent = File.ReadAllLines(csvFile);
            }
        }

        private void ParseCSVContent(string line)
        {
            string[] content = line.Split(',');

            var vidAnalysis = FFProbe.Analyse(content[0]);

            Split(vidAnalysis, Int32.Parse(content[1]), Int32.Parse(content[2]), content[3]);
        }

        private void Split(IMediaAnalysis source, int startSeconds, int stopSeconds, string output)
		{
			FFMpegArguments
				.FromFileInput(source)
				.OutputToFile(output, false, options => options
					.Seek(TimeSpan.FromSeconds((double)startSeconds))
					.WithCustomArgument($"-to { stopSeconds }")
					.ForceFormat("mp4"))
				.ProcessSynchronously();
		}
    }
}