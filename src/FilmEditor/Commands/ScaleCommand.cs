using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Exceptions;
using FFMpegCore.Helpers;
using FFMpegCore.Pipes;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using FilmEditor.Data;

namespace FilmEditor.Commands
{
    public class ScaleCommand : ICommand
    {
		public const string COMMAND_NAME = "scale";
		public string exeFile;
		public string movieFile;
		public string seasonFolder;
		public string outputDirectory;
		public bool newFolder = false;

		public ScaleCommand(string[] _args)
		{
			ParseArgs(_args);
		}

        public Task<bool> ExecuteAsync()
		{
			return Task.Run(() => {
				if (Program.cmdFlags.helpFlag)
                {
                    helpScreen();
                    return false;
                }

				if (string.IsNullOrEmpty(exeFile))
				{
					Console.WriteLine("Video2x executable needs to be passed in with the --exe flag");
					return false;
				}

				DebugWriteLine("starting scale");

				if (!string.IsNullOrEmpty(movieFile))
				{
					DebugWriteLine($"scaling { movieFile }");
					ConvertMovie(movieFile);
					return true;
				}
				else if (!string.IsNullOrEmpty(seasonFolder))
				{
					DebugWriteLine($"scaling { seasonFolder }");
					ConvertTVSeason(seasonFolder);
					return true;
				}

				string[] arguments = Environment.GetCommandLineArgs();

				DebugWriteLine($"movie file is empty: { movieFile }");
				DebugWriteLine($"tv season folder is empty: { seasonFolder }");
				DebugWriteLine($"{ string.Join(", ", arguments) }");

				return false;
			});
		}

		private void ParseArgs(string[] args)
		{
			for(int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-m" || args[i] == "--movie")
                {
                    movieFile = args[i+1];
                }
                else if (args[i] == "-t" || args[i] == "--tv")
                {
                    seasonFolder = args[i+1];
                }
				else if (args[i] == "-e" || args[i] == "--exe")
				{
					exeFile = args[i+1];
				}
				else if (args[i] == "-o" || args[i] == "--output")
				{
					outputDirectory = args[i+1];
				}
				else if (args[i] == "-n" || args[i] == "--new")
				{
					newFolder = true;
				}
            }
		}

		private void helpScreen()
		{
			StringBuilder helpScreenText = new();

            helpScreenText.AppendLine($"FilmEditor { Program.cmdFlags.versionText }");
            helpScreenText.AppendLine("Usage:  FilmEditor scale [OPTION]  ");
            helpScreenText.AppendLine("");
            helpScreenText.AppendLine("    -e | --exe <video2x executable file>            Pass in the path to the video2x executable file");
			helpScreenText.AppendLine("    -m | --movie <single video file>            Pass in the file to be scaled up and down accordingly");
			helpScreenText.AppendLine("    -t | -tv <folder path>            Path in the folder to be scaled up and down accordingly");
			helpScreenText.AppendLine("");
            helpScreenText.AppendLine("    -o | --output <destionation folder>  (Optional) Set Destionation Folder");
			helpScreenText.AppendLine("    -n | --new                        Switch statement to make a new folder per video converted");
            helpScreenText.AppendLine("");

            Console.WriteLine(helpScreenText.ToString());
		}

		public void ConvertMovie(string file)
		{
			DebugWriteLine($"Probing { file }");
			var vidAnalysis = FFProbe.Analyse(file);

			TestAndMakeNewFolder(file);

			if (GetAspectRatio(vidAnalysis.PrimaryVideoStream.Width, vidAnalysis.PrimaryVideoStream.Height) == "16x9")
			{
				DebugWriteLine($"{ file } is 16x9 aspect ratio");
				UpScale16x9(vidAnalysis, file);
				DownScale16x9(vidAnalysis, file);
				CopyOriginal(vidAnalysis, file);
			}
			else if (GetAspectRatio(vidAnalysis.PrimaryVideoStream.Width, vidAnalysis.PrimaryVideoStream.Height) == "4x3")
			{
				DebugWriteLine($"{ file } is 4x3 aspect ratio");
				UpScale4x3(vidAnalysis, file);
				DownScale4x3(vidAnalysis, file);
				CopyOriginal(vidAnalysis, file);
			}
			else
			{
				Console.WriteLine($"{ file } is a nonstandard aspect ratio");
			}
		}

		private string GetAspectRatio(int width, int height)
		{			
			if (((height * 16) / 9) == width)
			{
				return "16x9";
			}
			else if (((height * 4) / 3) == width)
			{
				return "4x3";
			}
			else
			{
				return "Aspect Ratio not found";
			}
		}

		private void UpScale16x9(IMediaAnalysis ffprobeAnalysis, string file)
		{
			DebugWriteLine("starting the upscaling process");

			string[] outputNames = GenerateOutputNames(file);

			switch (ffprobeAnalysis.PrimaryVideoStream.Width)
			{
				case (int)Scale._4kwidth16x9:
					break;
				case (int)Scale._2kwidth16x9:
					UpScale(file, (int)Scale._4kwidth16x9, (int)Scale._4kheight16x9, outputNames[0]);
					break;
				case (int)Scale._1080width16x9:
					UpScale(file, (int)Scale._4kwidth16x9, (int)Scale._4kheight16x9, outputNames[0]);
					UpScale(file, (int)Scale._2kwidth16x9, (int)Scale._2kheight16x9, outputNames[1]);
					break;
				case (int)Scale._720width16x9:
					UpScale(file, (int)Scale._4kwidth16x9, (int)Scale._4kheight16x9, outputNames[0]);
					UpScale(file, (int)Scale._2kwidth16x9, (int)Scale._2kheight16x9, outputNames[1]);
					UpScale(file, (int)Scale._1080width16x9, (int)Scale._1080height16x9, outputNames[2]);
					break;
				case (int)Scale._480width16x9:
					UpScale(file, (int)Scale._4kwidth16x9, (int)Scale._4kheight16x9, outputNames[0]);
					UpScale(file, (int)Scale._2kwidth16x9, (int)Scale._2kheight16x9, outputNames[1]);
					UpScale(file, (int)Scale._1080width16x9, (int)Scale._1080height16x9, outputNames[2]);
					UpScale(file, (int)Scale._720width16x9, (int)Scale._720height16x9, outputNames[3]);
					break;
			}
		}

		private void DownScale16x9(IMediaAnalysis ffprobeAnalysis, string file)
		{
			DebugWriteLine("starting the downscaling process");

			string[] outputNames = GenerateOutputNames(file);

			switch (ffprobeAnalysis.PrimaryVideoStream.Width)
			{
				case (int)Scale._4kwidth16x9:
					DownScale(ffprobeAnalysis, (int)Scale._2kwidth16x9, (int)Scale._2kheight16x9, outputNames[1]);
					DownScale(ffprobeAnalysis, (int)Scale._1080width16x9, (int)Scale._1080height16x9, outputNames[2]);
					DownScale(ffprobeAnalysis, (int)Scale._720width16x9, (int)Scale._720height16x9, outputNames[3]);
					DownScale(ffprobeAnalysis, (int)Scale._480width16x9, (int)Scale._480height16x9, outputNames[4]);
					break;
				case (int)Scale._2kwidth16x9:
					DownScale(ffprobeAnalysis, (int)Scale._1080width16x9, (int)Scale._1080height16x9, outputNames[2]);
					DownScale(ffprobeAnalysis, (int)Scale._720width16x9, (int)Scale._720height16x9, outputNames[3]);
					DownScale(ffprobeAnalysis, (int)Scale._480width16x9, (int)Scale._480height16x9, outputNames[4]);
					break;
				case (int)Scale._1080width16x9:
					DownScale(ffprobeAnalysis, (int)Scale._720width16x9, (int)Scale._720height16x9, outputNames[3]);
					DownScale(ffprobeAnalysis, (int)Scale._480width16x9, (int)Scale._480height16x9, outputNames[4]);
					break;
				case (int)Scale._720width16x9:
					DownScale(ffprobeAnalysis, (int)Scale._480width16x9, (int)Scale._480height16x9, outputNames[4]);
					break;
				case (int)Scale._480width16x9:
					break;
				default:
					Console.WriteLine($"{ file } is not one of the 5 expected resolutions");
					CustomScale16x9(ffprobeAnalysis, file, outputNames);
					break;
			}
		}

		private void CustomScale16x9(IMediaAnalysis source, string file, string[] outputNames)
		{
			if (source.PrimaryVideoStream.Height > (int)Scale._4kheight16x9)
			{
				DownScale(source, (int)Scale._4kwidth16x9, (int)Scale._4kheight16x9, outputNames[0]);
				DownScale(source, (int)Scale._2kwidth16x9, (int)Scale._2kheight16x9, outputNames[1]);
				DownScale(source, (int)Scale._1080width16x9, (int)Scale._1080height16x9, outputNames[2]);
				DownScale(source, (int)Scale._720width16x9, (int)Scale._720height16x9, outputNames[3]);
				DownScale(source, (int)Scale._480width16x9, (int)Scale._480height16x9, outputNames[4]);
			}
			else if (source.PrimaryVideoStream.Height < (int)Scale._4kheight16x9 && source.PrimaryVideoStream.Height < (int)Scale._2kheight16x9)
			{
				UpScale(file, (int)Scale._4kwidth16x9, (int)Scale._4kheight16x9, outputNames[0]);
				DownScale(source, (int)Scale._2kwidth16x9, (int)Scale._2kheight16x9, outputNames[1]);
				DownScale(source, (int)Scale._1080width16x9, (int)Scale._1080height16x9, outputNames[2]);
				DownScale(source, (int)Scale._720width16x9, (int)Scale._720height16x9, outputNames[3]);
				DownScale(source, (int)Scale._480width16x9, (int)Scale._480height16x9, outputNames[4]);
			}
			else if (source.PrimaryVideoStream.Height < (int)Scale._2kheight16x9 && source.PrimaryVideoStream.Height > (int)Scale._1080height16x9)
			{
				UpScale(file, (int)Scale._4kwidth16x9, (int)Scale._4kheight16x9, outputNames[0]);
				UpScale(file, (int)Scale._2kwidth16x9, (int)Scale._2kheight16x9, outputNames[1]);
			}
			else if (source.PrimaryVideoStream.Height < (int)Scale._1080height16x9 && source.PrimaryVideoStream.Height > (int)Scale._720height16x9)
			{
				UpScale(file, (int)Scale._4kwidth16x9, (int)Scale._4kheight16x9, outputNames[0]);
				UpScale(file, (int)Scale._2kwidth16x9, (int)Scale._2kheight16x9, outputNames[1]);
				UpScale(file, (int)Scale._1080width16x9, (int)Scale._1080height16x9, outputNames[2]);
				DownScale(source, (int)Scale._720width16x9, (int)Scale._720height16x9, outputNames[3]);
				DownScale(source, (int)Scale._480width16x9, (int)Scale._480height16x9, outputNames[4]);
			}
			else if (source.PrimaryVideoStream.Height < (int)Scale._720height16x9 && source.PrimaryVideoStream.Height > (int)Scale._480height16x9)
			{
				UpScale(file, (int)Scale._4kwidth16x9, (int)Scale._4kheight16x9, outputNames[0]);
				UpScale(file, (int)Scale._2kwidth16x9, (int)Scale._2kheight16x9, outputNames[1]);
				UpScale(file, (int)Scale._1080width16x9, (int)Scale._1080height16x9, outputNames[2]);
				UpScale(file, (int)Scale._720width16x9, (int)Scale._720height16x9, outputNames[3]);
				DownScale(source, (int)Scale._480width16x9, (int)Scale._480height16x9, outputNames[4]);
			}
			else
			{
				UpScale(file, (int)Scale._4kwidth16x9, (int)Scale._4kheight16x9, outputNames[0]);
				UpScale(file, (int)Scale._2kwidth16x9, (int)Scale._2kheight16x9, outputNames[1]);
				UpScale(file, (int)Scale._1080width16x9, (int)Scale._1080height16x9, outputNames[2]);
				UpScale(file, (int)Scale._720width16x9, (int)Scale._720height16x9, outputNames[3]);
				UpScale(file, (int)Scale._480width16x9, (int)Scale._480height16x9, outputNames[4]);
			}
		}

		private void UpScale4x3(IMediaAnalysis ffprobeAnalysis, string file)
		{
			DebugWriteLine("starting the upscale process");

			string[] outputNames = GenerateOutputNames(file);

			switch (ffprobeAnalysis.PrimaryVideoStream.Width)
			{
				case (int)Scale._4kwidth4x3:
					break;
				case (int)Scale._2kwidth4x3:
					UpScale(file, (int)Scale._4kwidth4x3, (int)Scale._4kheight4x3, outputNames[0]);
					break;
				case (int)Scale._1080width4x3:
					UpScale(file, (int)Scale._4kwidth4x3, (int)Scale._4kheight4x3, outputNames[0]);
					UpScale(file, (int)Scale._2kwidth4x3, (int)Scale._2kheight4x3, outputNames[1]);
					break;
				case (int)Scale._720width4x3:
					UpScale(file, (int)Scale._4kwidth4x3, (int)Scale._4kheight4x3, outputNames[0]);
					UpScale(file, (int)Scale._2kwidth4x3, (int)Scale._2kheight4x3, outputNames[1]);
					UpScale(file, (int)Scale._1080width4x3, (int)Scale._1080height4x3, outputNames[2]);
					break;
				case (int)Scale._480width4x3:
					UpScale(file, (int)Scale._4kwidth4x3, (int)Scale._4kheight4x3, outputNames[0]);
					UpScale(file, (int)Scale._2kwidth4x3, (int)Scale._2kheight4x3, outputNames[1]);
					UpScale(file, (int)Scale._1080width4x3, (int)Scale._1080height4x3, outputNames[2]);
					UpScale(file, (int)Scale._720width4x3, (int)Scale._720height4x3, outputNames[3]);
					break;
			}
		}

		private void DownScale4x3(IMediaAnalysis ffprobeAnalysis, string file)
		{
			DebugWriteLine("starting the downscaling process");

			string[] outputNames = GenerateOutputNames(file);

			switch (ffprobeAnalysis.PrimaryVideoStream.Width)
			{
				case (int)Scale._4kwidth4x3:
					DownScale(ffprobeAnalysis, (int)Scale._2kwidth4x3, (int)Scale._2kheight4x3, outputNames[1]);
					DownScale(ffprobeAnalysis, (int)Scale._1080width4x3, (int)Scale._1080height4x3, outputNames[2]);
					DownScale(ffprobeAnalysis, (int)Scale._720width4x3, (int)Scale._720height4x3, outputNames[3]);
					DownScale(ffprobeAnalysis, (int)Scale._480width4x3, (int)Scale._480height4x3, outputNames[4]);
					break;
				case (int)Scale._2kwidth4x3:
					DownScale(ffprobeAnalysis, (int)Scale._1080width4x3, (int)Scale._1080height4x3, outputNames[2]);
					DownScale(ffprobeAnalysis, (int)Scale._720width4x3, (int)Scale._720height4x3, outputNames[3]);
					DownScale(ffprobeAnalysis, (int)Scale._480width4x3, (int)Scale._480height4x3, outputNames[4]);
					break;
				case (int)Scale._1080width4x3:
					DownScale(ffprobeAnalysis, (int)Scale._720width4x3, (int)Scale._720height4x3, outputNames[3]);
					DownScale(ffprobeAnalysis, (int)Scale._480width4x3, (int)Scale._480height4x3, outputNames[4]);
					break;
				case (int)Scale._720width4x3:
					DownScale(ffprobeAnalysis, (int)Scale._480width4x3, (int)Scale._480height4x3, outputNames[4]);
					break;
				case (int)Scale._480width4x3:
					break;
				default:
					Console.WriteLine($"{ file } is not one of the 5 expected resolutions");
					CustomScale4x3(ffprobeAnalysis, file, outputNames);
					break;
			}
		}

		private void CustomScale4x3(IMediaAnalysis source, string file, string[] outputNames)
		{
			if (source.PrimaryVideoStream.Height > (int)Scale._4kheight4x3)
			{
				DownScale(source, (int)Scale._4kwidth4x3, (int)Scale._4kheight4x3, outputNames[0]);
				DownScale(source, (int)Scale._2kwidth4x3, (int)Scale._2kheight4x3, outputNames[1]);
				DownScale(source, (int)Scale._1080width4x3, (int)Scale._1080height4x3, outputNames[2]);
				DownScale(source, (int)Scale._720width4x3, (int)Scale._720height4x3, outputNames[3]);
				DownScale(source, (int)Scale._480width4x3, (int)Scale._480height4x3, outputNames[4]);
			}
			else if (source.PrimaryVideoStream.Height < (int)Scale._4kheight4x3 && source.PrimaryVideoStream.Height < (int)Scale._2kheight4x3)
			{
				UpScale(file, (int)Scale._4kwidth4x3, (int)Scale._4kheight4x3, outputNames[0]);
				DownScale(source, (int)Scale._2kwidth4x3, (int)Scale._2kheight4x3, outputNames[1]);
				DownScale(source, (int)Scale._1080width4x3, (int)Scale._1080height4x3, outputNames[2]);
				DownScale(source, (int)Scale._720width4x3, (int)Scale._720height4x3, outputNames[3]);
				DownScale(source, (int)Scale._480width4x3, (int)Scale._480height4x3, outputNames[4]);
			}
			else if (source.PrimaryVideoStream.Height < (int)Scale._2kheight4x3 && source.PrimaryVideoStream.Height > (int)Scale._1080height4x3)
			{
				UpScale(file, (int)Scale._4kwidth4x3, (int)Scale._4kheight4x3, outputNames[0]);
				UpScale(file, (int)Scale._2kwidth4x3, (int)Scale._2kheight4x3, outputNames[1]);
			}
			else if (source.PrimaryVideoStream.Height < (int)Scale._1080height4x3 && source.PrimaryVideoStream.Height > (int)Scale._720height4x3)
			{
				UpScale(file, (int)Scale._4kwidth4x3, (int)Scale._4kheight4x3, outputNames[0]);
				UpScale(file, (int)Scale._2kwidth4x3, (int)Scale._2kheight4x3, outputNames[1]);
				UpScale(file, (int)Scale._1080width4x3, (int)Scale._1080height4x3, outputNames[2]);
				DownScale(source, (int)Scale._720width4x3, (int)Scale._720height4x3, outputNames[3]);
				DownScale(source, (int)Scale._480width4x3, (int)Scale._480height4x3, outputNames[4]);
			}
			else if (source.PrimaryVideoStream.Height < (int)Scale._720height4x3 && source.PrimaryVideoStream.Height > (int)Scale._480height4x3)
			{
				UpScale(file, (int)Scale._4kwidth4x3, (int)Scale._4kheight4x3, outputNames[0]);
				UpScale(file, (int)Scale._2kwidth4x3, (int)Scale._2kheight4x3, outputNames[1]);
				UpScale(file, (int)Scale._1080width4x3, (int)Scale._1080height4x3, outputNames[2]);
				UpScale(file, (int)Scale._720width4x3, (int)Scale._720height4x3, outputNames[3]);
				DownScale(source, (int)Scale._480width4x3, (int)Scale._480height4x3, outputNames[4]);
			}
			else
			{
				UpScale(file, (int)Scale._4kwidth4x3, (int)Scale._4kheight4x3, outputNames[0]);
				UpScale(file, (int)Scale._2kwidth4x3, (int)Scale._2kheight4x3, outputNames[1]);
				UpScale(file, (int)Scale._1080width4x3, (int)Scale._1080height4x3, outputNames[2]);
				UpScale(file, (int)Scale._720width4x3, (int)Scale._720height4x3, outputNames[3]);
				UpScale(file, (int)Scale._480width4x3, (int)Scale._480height4x3, outputNames[4]);
			}
		}

		private void UpScale(string file, int upscaledWidth, int upscaledHeight, string output)
		{
			string yamlPath = $"{ Path.GetDirectoryName(exeFile) }\\video2x.yaml";
			DebugWriteLine($"Yaml Path: { yamlPath }");

			Console.WriteLine("Running Video2x with the following command: ");
			Console.WriteLine($"{ exeFile } -c { yamlPath } -i \"{ file }\" -w { upscaledWidth } -h { upscaledHeight } -d anime4kcpp -o \"{ output }\"");

			using (var video2xProcess = new Process())
			{
				video2xProcess.StartInfo.FileName = exeFile;
				video2xProcess.StartInfo.Arguments = $"-c { yamlPath } -i \"{ file }\" -w { upscaledWidth } -h { upscaledHeight } -d anime4kcpp -o \"{ output }\"";

				video2xProcess.StartInfo.RedirectStandardOutput = true;
				video2xProcess.StartInfo.RedirectStandardError = true;
				video2xProcess.StartInfo.UseShellExecute = false;
				video2xProcess.StartInfo.CreateNoWindow = true;

				video2xProcess.Start();
				string result = video2xProcess.StandardError.ReadToEnd();
				video2xProcess.WaitForExit();

				if (!string.IsNullOrEmpty(result))
				{
					Console.WriteLine(result);
				}
			}
		}

		private void DownScale(IMediaAnalysis source, int downscaledWidth, int downscaledHeight, string output)
		{
			FFMpegArguments
				.FromFileInput(source)
				.OutputToFile(output, true, options => options
					.Scale(downscaledWidth, downscaledHeight))
				.ProcessSynchronously();
		}

		private void CopyOriginal(IMediaAnalysis source, string file)
		{
			string[] outputNames = GenerateOutputNames(file);

			switch (source.PrimaryVideoStream.Height)
			{
				case (int)Scale._4kheight16x9:
					File.Copy(file, outputNames[0]);
					break;
				case (int)Scale._2kheight16x9:
					File.Copy(file, outputNames[1]);
					break;
				case (int)Scale._1080height16x9:
					File.Copy(file, outputNames[2]);
					break;
				case (int)Scale._720height16x9:
					File.Copy(file, outputNames[3]);
					break;
				case (int)Scale._480height16x9:
					File.Copy(file, outputNames[4]);
					break;
			}
		}

		public string[] GenerateOutputNames(string file)
		{
			List<string> outputNamesList = new();

			if (newFolder)
			{
				if (string.IsNullOrEmpty(outputDirectory))
				{
					outputNamesList.Add($"{ Path.GetFileNameWithoutExtension(file) }\\{ Path.GetFileNameWithoutExtension(file) } - 4K{ Path.GetExtension(file) }");
					outputNamesList.Add($"{ Path.GetFileNameWithoutExtension(file) }\\{ Path.GetFileNameWithoutExtension(file) } - 2K{ Path.GetExtension(file) }");
					outputNamesList.Add($"{ Path.GetFileNameWithoutExtension(file) }\\{ Path.GetFileNameWithoutExtension(file) } - 1080p{ Path.GetExtension(file) }");
					outputNamesList.Add($"{ Path.GetFileNameWithoutExtension(file) }\\{ Path.GetFileNameWithoutExtension(file) } - 720p{ Path.GetExtension(file) }");
					outputNamesList.Add($"{ Path.GetFileNameWithoutExtension(file) }\\{ Path.GetFileNameWithoutExtension(file) } - 480p{ Path.GetExtension(file) }");
				}
				else
				{
					if (PathEndsWithSlash(outputDirectory))
					{
						outputNamesList.Add($"{ outputDirectory }{ Path.GetFileNameWithoutExtension(file) }\\{ Path.GetFileNameWithoutExtension(file) } - 4K{ Path.GetExtension(file) }");
						outputNamesList.Add($"{ outputDirectory }{ Path.GetFileNameWithoutExtension(file) }\\{ Path.GetFileNameWithoutExtension(file) } - 2K{ Path.GetExtension(file) }");
						outputNamesList.Add($"{ outputDirectory }{ Path.GetFileNameWithoutExtension(file) }\\{ Path.GetFileNameWithoutExtension(file) } - 1080p{ Path.GetExtension(file) }");
						outputNamesList.Add($"{ outputDirectory }{ Path.GetFileNameWithoutExtension(file) }\\{ Path.GetFileNameWithoutExtension(file) } - 720p{ Path.GetExtension(file) }");
						outputNamesList.Add($"{ outputDirectory }{ Path.GetFileNameWithoutExtension(file) }\\{ Path.GetFileNameWithoutExtension(file) } - 480p{ Path.GetExtension(file) }");
					}
					else
					{
						outputNamesList.Add($"{ outputDirectory }\\{ Path.GetFileNameWithoutExtension(file) }\\{ Path.GetFileNameWithoutExtension(file) } - 4K{ Path.GetExtension(file) }");
						outputNamesList.Add($"{ outputDirectory }\\{ Path.GetFileNameWithoutExtension(file) }\\{ Path.GetFileNameWithoutExtension(file) } - 2K{ Path.GetExtension(file) }");
						outputNamesList.Add($"{ outputDirectory }\\{ Path.GetFileNameWithoutExtension(file) }\\{ Path.GetFileNameWithoutExtension(file) } - 1080p{ Path.GetExtension(file) }");
						outputNamesList.Add($"{ outputDirectory }\\{ Path.GetFileNameWithoutExtension(file) }\\{ Path.GetFileNameWithoutExtension(file) } - 720p{ Path.GetExtension(file) }");
						outputNamesList.Add($"{ outputDirectory }\\{ Path.GetFileNameWithoutExtension(file) }\\{ Path.GetFileNameWithoutExtension(file) } - 480p{ Path.GetExtension(file) }");
					}
				}
			}
			else
			{
				if (string.IsNullOrEmpty(outputDirectory))
				{
					outputNamesList.Add($"{ Path.GetFileNameWithoutExtension(file) } - 4K{ Path.GetExtension(file) }");
					outputNamesList.Add($"{ Path.GetFileNameWithoutExtension(file) } - 2K{ Path.GetExtension(file) }");
					outputNamesList.Add($"{ Path.GetFileNameWithoutExtension(file) } - 1080p{ Path.GetExtension(file) }");
					outputNamesList.Add($"{ Path.GetFileNameWithoutExtension(file) } - 720p{ Path.GetExtension(file) }");
					outputNamesList.Add($"{ Path.GetFileNameWithoutExtension(file) } - 480p{ Path.GetExtension(file) }");
				}
				else
				{
					if (PathEndsWithSlash(outputDirectory))
					{
						outputNamesList.Add($"{ outputDirectory }{ Path.GetFileNameWithoutExtension(file) } - 4K{ Path.GetExtension(file) }");
						outputNamesList.Add($"{ outputDirectory }{ Path.GetFileNameWithoutExtension(file) } - 2K{ Path.GetExtension(file) }");
						outputNamesList.Add($"{ outputDirectory }{ Path.GetFileNameWithoutExtension(file) } - 1080p{ Path.GetExtension(file) }");
						outputNamesList.Add($"{ outputDirectory }{ Path.GetFileNameWithoutExtension(file) } - 720p{ Path.GetExtension(file) }");
						outputNamesList.Add($"{ outputDirectory }{ Path.GetFileNameWithoutExtension(file) } - 480p{ Path.GetExtension(file) }");
					}
					else
					{
						outputNamesList.Add($"{ outputDirectory }\\{ Path.GetFileNameWithoutExtension(file) } - 4K{ Path.GetExtension(file) }");
						outputNamesList.Add($"{ outputDirectory }\\{ Path.GetFileNameWithoutExtension(file) } - 2K{ Path.GetExtension(file) }");
						outputNamesList.Add($"{ outputDirectory }\\{ Path.GetFileNameWithoutExtension(file) } - 1080p{ Path.GetExtension(file) }");
						outputNamesList.Add($"{ outputDirectory }\\{ Path.GetFileNameWithoutExtension(file) } - 720p{ Path.GetExtension(file) }");
						outputNamesList.Add($"{ outputDirectory }\\{ Path.GetFileNameWithoutExtension(file) } - 480p{ Path.GetExtension(file) }");
					}
				}
			}

			return outputNamesList.ToArray();
		}

		private bool PathEndsWithSlash(string TestPath)
		{
			if (TestPath[TestPath.Length-1] == '\\')
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private void TestAndMakeNewFolder(string file)
		{
			if (newFolder)
			{
				if (!string.IsNullOrEmpty(outputDirectory))
				{
					Directory.CreateDirectory($"{ outputDirectory }\\{ Path.GetFileNameWithoutExtension(file) }");
				}
				else
				{
					Directory.CreateDirectory($"{ Path.GetDirectoryName(file) }\\{ Path.GetFileNameWithoutExtension(file) }");
				}
			}
		}

		public void ConvertTVSeason(string folder)
		{
			string[] files = Directory.GetFiles(folder);

			foreach (string file in files)
			{
				ConvertMovie(file);
			}
		}

		private void DebugWriteLine(string text)
		{
			#if DEBUG
				Console.WriteLine(text);
			#endif
		}
    }
}