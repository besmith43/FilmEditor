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

        private void ParseArgs(string[] args)
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

        private void setCSVContents()
        {
            csvContents = new ConsoleTable("inputFile","start", "stop","outputFile");
            csv.AppendLine("inputFile,start,stop,outputFile");
        }

        private void getCsvFileName()
        {
            csvFilename = $"{ Path.GetFileNameWithoutExtension(inputFile) }.csv";
        }

        private void getNextRow()
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

        private void getNextRowSplit()
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

            Console.WriteLine("What is the start time? (minutes.seconds)");
            string startInput = Console.ReadLine();
            int startTime = convertTime(startInput);

            Console.WriteLine("What is the stop time? (minutes.seconds)");
            string stopInput = Console.ReadLine();
            int stopTime = convertTime(stopInput);

            string episodeFilename = getEpisodeFilename(episodeNumber);

            csvContents.AddRow(inputFile, startTime, stopTime, episodeFilename);
            csv.AppendLine($"{ inputFile },{ startTime },{ stopTime },{ episodeFilename }");

            csvContents.Write();
            Console.WriteLine();
        }

        private string getEpisodeFilename(string episode)
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

        private void getNextRowEdit()
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

        private int convertTime(string timeString)
        {
            string[] parts = timeString.Split('.');
            int minutes = Int32.Parse(parts[0]);
            int seconds = Int32.Parse(parts[1]);

            int total = (minutes * 60) + seconds;

            return total;
        }

        private bool exitCheck()
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

        private void saveCSV()
        {
            File.WriteAllText(csvFilename, csv.ToString());
        }
    }
}