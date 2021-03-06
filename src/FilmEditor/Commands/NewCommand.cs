using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ConsoleTables;

namespace FilmEditor.Commands
{
    public class NewCommand : ICommand
    {
        public const string COMMAND_NAME = "new";
        public ConsoleTable csvContents;
        public string command;
        public StringBuilder csv;
        public string inputFile;
        public string csvFilename;
        public string showName;
        public string seasonNum;
        public int editCount = 0;

        public NewCommand(string[] _args)
        {
            ParseArgs(_args);

            csv = new();

            getCsvFileName();

            setCSVContents();
        }

        public Task<bool> ExecuteAsync()
        {
            return Task.Run(() => {

                if (Program.cmdFlags.helpFlag)
                {
                    helpScreen();
                    return false;
                }

                bool quit = false;

                do
                {
                    getNextRow();
                    quit = exitCheck();
                } while (!quit);

                saveCSV();

                return true;
            });
        }

        public void ParseArgs(string[] args)
        {
            for(int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-e" || args[i] == "--edit")
                {
                    command = "edit";
                    inputFile = args[i+1];
                }
                else if (args[i] == "-s" || args[i] == "--split")
                {
                    command = "split";
                    inputFile = args[i+1];
                }
                else if (args[i] == "--season")
                {
                    seasonNum = args[i+1];
                }
            }
        }

        public void setCSVContents()
        {
            csvContents = new ConsoleTable("inputFile","start", "stop","outputFile");
            csv.AppendLine("inputFile,start,stop,outputFile");
        }

        public void getCsvFileName()
        {
            csvFilename = $"{ Path.GetFileNameWithoutExtension(inputFile) }.csv";
        }

        public void getNextRow()
        {
            if (command == "split")
            {
                getNextRowSplit();
            }
            else
            {
                getNextRowEdit();
            }
        }

        public void getNextRowSplit()
        {
            if (String.IsNullOrEmpty(showName))
            {
                Console.WriteLine("What is the name of the TV Show?");
                showName = Console.ReadLine();
            }
            
            if (String.IsNullOrEmpty(seasonNum))
            {
                Console.WriteLine("What season is it?");
                seasonNum = Console.ReadLine();
            }

            Console.WriteLine("What is the episode number?");
            string episodeNumber = Console.ReadLine();

            Console.WriteLine("What is the start time? (hours.minutes.seconds)");
            string startInput = Console.ReadLine();
            int startTime = convertTime(startInput);

            Console.WriteLine("What is the stop time? (hours.minutes.seconds)");
            string stopInput = Console.ReadLine();
            int stopTime = convertTime(stopInput);

            string episodeFilename = getEpisodeFilename(episodeNumber);

            csvContents.AddRow(inputFile, startTime, stopTime, episodeFilename);
            csv.AppendLine($"{ inputFile },{ startTime },{ stopTime },{ episodeFilename }");

            csvContents.Write();
            Console.WriteLine();
        }

        public string getEpisodeFilename(string episode)
        {
            if (seasonNum.Length < 2)
            {
                if (episode.Length < 2)
                {
                    return $"{ showName } s0{ seasonNum }e0{ episode }.mp4";
                }
                else
                {
                    return $"{ showName } s0{ seasonNum }e{ episode }.mp4";
                }
            }
            else
            {
                if (episode.Length < 2)
                {
                    return $"{ showName } s{ seasonNum }e0{ episode }.mp4";
                }
                else
                {
                    return $"{ showName } s{ seasonNum }e{ episode }.mp4";
                }
            }
        }

        public void getNextRowEdit()
        {
            editCount++;

            Console.WriteLine("What is the start time? (minutes.seconds)");
            string startInput = Console.ReadLine();
            int startTime = convertTime(startInput);

            Console.WriteLine("What is the stop time? (minutes.seconds)");
            string stopInput = Console.ReadLine();
            int stopTime = convertTime(stopInput);

            csvContents.AddRow(inputFile, startTime, stopTime, $"{ Path.GetFileNameWithoutExtension(inputFile) }{ editCount }.mp4");
            csv.AppendLine($"{ inputFile },{ startTime },{ stopTime },{ Path.GetFileNameWithoutExtension(inputFile) }{ editCount }.mp4");

            csvContents.Write();
            Console.WriteLine();
        }

        public int convertTime(string timeString)
        {
            int total = 0;
            int hours = 0;
            int minutes = 0;
            int seconds = 0;

            string[] parts = timeString.Split('.');

            switch (parts.Length)
            {
                case 1:
                    total = Int32.Parse(parts[0]);
                    break;
                case 2:
                    minutes = Int32.Parse(parts[0]);
                    seconds = Int32.Parse(parts[1]);

                    total = (minutes * 60) + seconds;
                    break;
                case 3:
                    hours = Int32.Parse(parts[0]);
                    minutes = Int32.Parse(parts[1]);
                    seconds = Int32.Parse(parts[2]);

                    total = (hours * 60 * 60) + (minutes * 60) + seconds;
                    break;
            }

            return total;
        }

        public bool exitCheck()
        {
            Console.WriteLine("Quit adding entries? (y/n)");
            string answer = Console.ReadLine();

            if (answer == "y" || answer == "Y")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void saveCSV()
        {
            File.WriteAllText(csvFilename, csv.ToString());
        }

        public void helpScreen()
        {
            StringBuilder helpScreenText = new();

            helpScreenText.AppendLine($"FilmEditor { Program.cmdFlags.versionText }");
            helpScreenText.AppendLine("Usage:  FilmEditor new [OPTION]  ");
            helpScreenText.AppendLine("");
            helpScreenText.AppendLine("    -e | --edit <mp4 file>           Create csv for edit command");
            helpScreenText.AppendLine("    -s | --split <mp4 file>          Create csv for split command");
            helpScreenText.AppendLine("    --season <int>                   (Optional) Pass in the season number");
            helpScreenText.AppendLine("");

            Console.WriteLine(helpScreenText.ToString());
        }
    }
}