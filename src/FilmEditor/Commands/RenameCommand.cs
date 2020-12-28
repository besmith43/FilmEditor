using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using ConsoleTables;

namespace FilmEditor.Commands
{
    public class RenameCommand : ICommand
    {
		public const string COMMAND_NAME = "rename";
		public ConsoleTable renameContents;
		public Dictionary<string, string> rename;
		public string showName;
		public string sourceFolder;
		public string outputFolder;
		public string[] fileNames;
		public bool destructive = false;

		public RenameCommand(string[] _args)
		{
			ParseArgs(_args);

			renameContents = new("Old Name", "New Name");
			rename = new();

			if (String.IsNullOrEmpty(outputFolder))
			{
				outputFolder = Environment.CurrentDirectory;
			}

			ReadFolderContents();
		}

        public Task<bool> ExecuteAsync()
		{
			return Task.Run(() => {

				if (Program.cmdFlags.helpFlag)
                {
					helpScreen();
                    return false;
                }

				if (String.IsNullOrEmpty(showName))
				{
					Console.WriteLine("What is the name of the TV Show?");
					showName = Console.ReadLine();
				}

				foreach(var episode in fileNames)
				{
					getNewName(episode);
				}

				confirmInput();

				Console.Clear();
				return true;
			});
		}

		private void ParseArgs(string[] args)
        {
            for(int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-s" || args[i] == "--source")
                {
                    sourceFolder = args[i+1];
                }
                else if (args[i] == "-o" || args[i] == "--output")
                {
                    outputFolder = args[i+1];
                }
                else if (args[i] == "--showname")
                {
                    showName = args[i+1];
                }
				else if (args[i] == "-d" || args[i] == "--delete")
				{
					destructive = true;
				}
            }
        }

		public void ReadFolderContents()
		{
			if (Directory.Exists(sourceFolder))
			{
				fileNames = Directory.GetFiles(sourceFolder);
			}
		}

		public void getNewName(string old_name)
		{
			Console.WriteLine(old_name);
			Console.WriteLine("What season is this episode from?");
			string seasonNumber = Console.ReadLine();

			Console.WriteLine("What is the episode number?");
			string episodeNumber = Console.ReadLine();

			string episodeFilename = getEpisodeFilename(episodeNumber, seasonNumber);

			renameContents.AddRow(old_name, episodeFilename);
			rename.Add(old_name, episodeFilename);
		}

		private string getEpisodeFilename(string episode, string season)
        {
            if (season.Length < 2)
            {
                if (episode.Length < 2)
                {
                    return $"{ showName } s0{ season }e0{ episode }.mp4";
                }
                else
                {
                    return $"{ showName } s0{ season }e{ episode }.mp4";
                }
            }
            else
            {
                if (episode.Length < 2)
                {
                    return $"{ showName } s{ season }e0{ episode }.mp4";
                }
                else
                {
                    return $"{ showName } s{ season }e{ episode }.mp4";
                }
            }
        }

		public void confirmInput()
		{
			//print console table
			renameContents.Write();
			//get user confirmation
			Console.WriteLine("Is this correct? (Y/n)");
			string response = Console.ReadLine();
			
			if (response != "n" || response != "N")
			{
				//call makeChanges
				makeChanges();
			}
		}

		public void makeChanges()
		{
			foreach(var old_name in fileNames)
			{
				string new_name = "";
				if (rename.TryGetValue(old_name, out new_name))
				{
					if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
					{
						makeChange(old_name, $"{ outputFolder }\\{ new_name }");
					}
					else
					{
						makeChange(old_name, $"{ outputFolder }/{ new_name }");
					}
				}
			}
		}

		public void makeChange(string old_name, string new_name)
		{
			Console.WriteLine($"Copying { new_name }");
			try
			{
				File.Copy(old_name, new_name, false);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Copy of { old_name } to { new_name } failed with the following error:{ Environment.NewLine }{ e }");
			}

			if (destructive)
			{
				File.Delete(old_name);
			}
		}

		private void helpScreen()
        {
            StringBuilder helpScreenText = new();

            helpScreenText.AppendLine($"FilmEditor { Program.cmdFlags.versionText }");
            helpScreenText.AppendLine("Usage:  FilmEditor new [OPTION]  ");
            helpScreenText.AppendLine("");
			helpScreenText.AppendLine("    -s | --source <source folder>		Pass in the folder with the original files that need to be renamed");
			helpScreenText.AppendLine("    -o | --output <destination folder>	(Optional) Specify the destination folder for the renamed files");
			helpScreenText.AppendLine("    --showname <TV show name>			Pass in the TV Show episode files being renamed");
			helpScreenText.AppendLine("    -d | --delete						Delete the original copy of the files");
			helpScreenText.AppendLine("");

			Console.WriteLine(helpScreenText.ToString());
        }
    }
}