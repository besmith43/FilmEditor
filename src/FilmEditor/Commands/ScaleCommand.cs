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
		// maybe add a timer?
		public const string COMMAND_NAME = "scale";
		public string exeFile;
		public string configPath = Program.GetLocalConfig();
		public string movieFile;
		public string seasonFolder;
		public string outputDirectory;
		public bool newFolder = false;
		public string driver = DriverEnum.anime4kcpp;
		public int processes = 16;
		public bool scale_4k = false;
		public bool scale_2k = false;
		public bool scale_1080p = false;
		public bool scale_720p = false;
		public bool scale_480p = false;
		public bool scaleFlag = false;

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
					Stopwatch stopWatch = new Stopwatch();
        			stopWatch.Start();

					DebugWriteLine($"scaling { movieFile }");
					ConvertMovie(movieFile);

					stopWatch.Stop();

					TimeSpan ts = stopWatch.Elapsed;
					string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
			            ts.Hours, ts.Minutes, ts.Seconds,
			            ts.Milliseconds / 10);
			        Console.WriteLine("RunTime " + elapsedTime);

					return true;
				}
				else if (!string.IsNullOrEmpty(seasonFolder))
				{
					Stopwatch stopWatch = new Stopwatch();
        			stopWatch.Start();

					DebugWriteLine($"scaling { seasonFolder }");
					ConvertTVSeason(seasonFolder);

					stopWatch.Stop();

					TimeSpan ts = stopWatch.Elapsed;
					string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
			            ts.Hours, ts.Minutes, ts.Seconds,
			            ts.Milliseconds / 10);
			        Console.WriteLine("RunTime " + elapsedTime);

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
					i++;
                }
                else if (args[i] == "-t" || args[i] == "--tv")
                {
                    seasonFolder = args[i+1];
					i++;
                }
				else if (args[i] == "-e" || args[i] == "--exe")
				{
					exeFile = args[i+1];
					i++;
					SaveConfig(exeFile);
				}
				else if (args[i] == "-o" || args[i] == "--output")
				{
					outputDirectory = args[i+1];
					i++;
				}
				else if (args[i] == "-n" || args[i] == "--new")
				{
					newFolder = true;
				}
				else if (args[i] == "-d" || args[i] == "--driver")
				{
					switch(args[i+1])
					{
						case DriverEnum.anime4kcpp:
							driver = DriverEnum.anime4kcpp;
							break;
						case DriverEnum.waifu2x_caffe:
							driver = DriverEnum.waifu2x_caffe;
							break;
						case DriverEnum.waifu2x_converter_cpp:
							driver = DriverEnum.waifu2x_converter_cpp;
							break;
						case DriverEnum.waifu2x_ncnn_vulkan:
							driver = DriverEnum.waifu2x_ncnn_vulkan;
							break;
						case DriverEnum.srmd_ncnn_vulkan:
							driver = DriverEnum.srmd_ncnn_vulkan;
							break;
						case DriverEnum.realsr_ncnn_vulkan:
							driver = DriverEnum.realsr_ncnn_vulkan;
							break;
					}
					i++;
				}
				else if (args[i] == "-p" || args[i] == "--processes")
				{
					string tryProcesses = args[i+1];
					i++;

					if (!int.TryParse(tryProcesses, out processes))
					{
						processes = 16;
					}
				}
				else if (args[i] == "--4k")
				{
					scale_4k = true;

					if (!scaleFlag)
					{
						scaleFlag = true;
					}
				}
				else if (args[i] == "--2k")
				{
					scale_2k = true;

					if (!scaleFlag)
					{
						scaleFlag = true;
					}
				}
				else if (args[i] == "--1080p")
				{
					scale_1080p = true;

					if (!scaleFlag)
					{
						scaleFlag = true;
					}
				}
				else if (args[i] == "--720p")
				{
					scale_720p = true;

					if (!scaleFlag)
					{
						scaleFlag = true;
					}
				}
				else if (args[i] == "--480p")
				{
					scale_480p = true;

					if (!scaleFlag)
					{
						scaleFlag = true;
					}
				}
				else
				{
					
				}
            }

			if (string.IsNullOrEmpty(exeFile) && File.Exists(configPath))
			{
				ReadConfig();
			}

			if (!scaleFlag)
			{
				scale_4k = true;
				scale_2k = true;
				scale_1080p = true;
				scale_720p = true;
				scale_480p = true;
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
			helpScreenText.AppendLine("    -d | --driver                     Select the Driver that you'd like to use for upscaling");
			helpScreenText.AppendLine("    -p | --processes                  Select the number of processes to use for upscaling");
			helpScreenText.AppendLine("    --4k                              Switch to select 4K scale option");
			helpScreenText.AppendLine("    --2k                              Switch to select 2K scale option");
			helpScreenText.AppendLine("    --1080p                           Switch to select 1080p scale option");
			helpScreenText.AppendLine("    --720p                            Switch to select 720p scale option");
			helpScreenText.AppendLine("    --480p                            Switch to select 480p scale option");
            helpScreenText.AppendLine("");

            Console.WriteLine(helpScreenText.ToString());
		}

		public void SaveConfig(string video2xPath)
		{
			if (File.Exists(configPath))
			{
				string[] lines = File.ReadAllLines(configPath);

				for (int i = 1; i < lines.Length; i++)
				{
					string[] contents = lines[i].Split(',');

					if (contents[0] == "video2x")
					{
						lines[i] = $"video2x,{ video2xPath }";
					}
				}

				File.Delete(configPath);
				File.WriteAllLines(configPath, lines);
			}
			else
			{
				StringBuilder csvConfig = new();

				csvConfig.AppendLine("Config,Value");
				csvConfig.AppendLine($"video2x,{ video2xPath }");

				File.WriteAllText(configPath, csvConfig.ToString());
			}
		}

		public void ReadConfig()
		{
			string[] lines = File.ReadAllLines(configPath);

			for (int i = 1; i < lines.Length; i++)
			{
				string[] contents = lines[i].Split(',');

				if (contents[0] == "video2x")
				{
					exeFile = contents[1];
				}
			}
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
				Console.WriteLine("Would you like to try to scale it as a 16x9 video? (Y/n)");
				string answer = Console.ReadLine();

				if (answer != "n" || answer != "N")
				{
					string[] outputNames = GenerateOutputNames(file);
					CustomScale16x9(vidAnalysis, file, outputNames);
				}
			}
		}

		public string GetAspectRatio(int width, int height)
		{			
			if (Math.Ceiling(((height * 16) / 9.0)) == width)
			{
				return "16x9";
			}
			else if (Math.Ceiling(((height * 4) / 3.0)) == width)
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
					DebugWriteLine("Upscaling 16x9 2k to 4k");
					if (scale_4k) UpScale(file, (int)Scale._4kwidth16x9, (int)Scale._4kheight16x9, outputNames[0]);
					break;
				case (int)Scale._1080width16x9:
					DebugWriteLine("Upscaling 16x9 1080p to 2k & 4k");
					if (scale_4k) UpScale(file, (int)Scale._4kwidth16x9, (int)Scale._4kheight16x9, outputNames[0]);
					if (scale_2k) UpScale(file, (int)Scale._2kwidth16x9, (int)Scale._2kheight16x9, outputNames[1]);
					break;
				case (int)Scale._720width16x9:
					DebugWriteLine("Upscaling 16x9 720p to 1080p, 2k, & 4k");
					if (scale_4k) UpScale(file, (int)Scale._4kwidth16x9, (int)Scale._4kheight16x9, outputNames[0]);
					if (scale_2k) UpScale(file, (int)Scale._2kwidth16x9, (int)Scale._2kheight16x9, outputNames[1]);
					if (scale_1080p) UpScale(file, (int)Scale._1080width16x9, (int)Scale._1080height16x9, outputNames[2]);
					break;
				case (int)Scale._480width16x9:
					DebugWriteLine("Upscaling 16x9 480p to 720p, 1080p, 2k, & 4k");
					if (scale_4k) UpScale(file, (int)Scale._4kwidth16x9, (int)Scale._4kheight16x9, outputNames[0]);
					if (scale_2k) UpScale(file, (int)Scale._2kwidth16x9, (int)Scale._2kheight16x9, outputNames[1]);
					if (scale_1080p) UpScale(file, (int)Scale._1080width16x9, (int)Scale._1080height16x9, outputNames[2]);
					if (scale_720p) UpScale(file, (int)Scale._720width16x9, (int)Scale._720height16x9, outputNames[3]);
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
					DebugWriteLine("Downscaling 16x9 4k to 2k, 1080p, 720p, & 480p");
					if (scale_2k) DownScale(ffprobeAnalysis, (int)Scale._2kwidth16x9, (int)Scale._2kheight16x9, outputNames[1]);
					if (scale_1080p) DownScale(ffprobeAnalysis, (int)Scale._1080width16x9, (int)Scale._1080height16x9, outputNames[2]);
					if (scale_720p) DownScale(ffprobeAnalysis, (int)Scale._720width16x9, (int)Scale._720height16x9, outputNames[3]);
					if (scale_480p) DownScale(ffprobeAnalysis, (int)Scale._480width16x9, (int)Scale._480height16x9, outputNames[4]);
					break;
				case (int)Scale._2kwidth16x9:
					DebugWriteLine("Downscaling 16x9 2k to 1080p, 720p, & 480p");
					if (scale_1080p) DownScale(ffprobeAnalysis, (int)Scale._1080width16x9, (int)Scale._1080height16x9, outputNames[2]);
					if (scale_720p) DownScale(ffprobeAnalysis, (int)Scale._720width16x9, (int)Scale._720height16x9, outputNames[3]);
					if (scale_480p) DownScale(ffprobeAnalysis, (int)Scale._480width16x9, (int)Scale._480height16x9, outputNames[4]);
					break;
				case (int)Scale._1080width16x9:
					DebugWriteLine("Downscaling 16x9 1080p to 720p, & 480p");
					if (scale_720p) DownScale(ffprobeAnalysis, (int)Scale._720width16x9, (int)Scale._720height16x9, outputNames[3]);
					if (scale_480p) DownScale(ffprobeAnalysis, (int)Scale._480width16x9, (int)Scale._480height16x9, outputNames[4]);
					break;
				case (int)Scale._720width16x9:
					DebugWriteLine("Downscaling 16x9 720p to 480p");
					if (scale_480p) DownScale(ffprobeAnalysis, (int)Scale._480width16x9, (int)Scale._480height16x9, outputNames[4]);
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
			DebugWriteLine("Starting Custom Scale Upscale and Downscale");

			if (source.PrimaryVideoStream.Height > (int)Scale._4kheight16x9)
			{
				if (scale_4k) DownScale(source, (int)Scale._4kwidth16x9, (int)Scale._4kheight16x9, outputNames[0]);
				if (scale_2k) DownScale(source, (int)Scale._2kwidth16x9, (int)Scale._2kheight16x9, outputNames[1]);
				if (scale_1080p) DownScale(source, (int)Scale._1080width16x9, (int)Scale._1080height16x9, outputNames[2]);
				if (scale_720p) DownScale(source, (int)Scale._720width16x9, (int)Scale._720height16x9, outputNames[3]);
				if (scale_480p) DownScale(source, (int)Scale._480width16x9, (int)Scale._480height16x9, outputNames[4]);
			}
			else if (source.PrimaryVideoStream.Height < (int)Scale._4kheight16x9 && source.PrimaryVideoStream.Height < (int)Scale._2kheight16x9)
			{
				if (scale_4k) UpScale(file, (int)Scale._4kwidth16x9, (int)Scale._4kheight16x9, outputNames[0]);
				if (scale_2k) DownScale(source, (int)Scale._2kwidth16x9, (int)Scale._2kheight16x9, outputNames[1]);
				if (scale_1080p) DownScale(source, (int)Scale._1080width16x9, (int)Scale._1080height16x9, outputNames[2]);
				if (scale_720p) DownScale(source, (int)Scale._720width16x9, (int)Scale._720height16x9, outputNames[3]);
				if (scale_480p) DownScale(source, (int)Scale._480width16x9, (int)Scale._480height16x9, outputNames[4]);
			}
			else if (source.PrimaryVideoStream.Height < (int)Scale._2kheight16x9 && source.PrimaryVideoStream.Height > (int)Scale._1080height16x9)
			{
				if (scale_4k) UpScale(file, (int)Scale._4kwidth16x9, (int)Scale._4kheight16x9, outputNames[0]);
				if (scale_2k) UpScale(file, (int)Scale._2kwidth16x9, (int)Scale._2kheight16x9, outputNames[1]);
			}
			else if (source.PrimaryVideoStream.Height < (int)Scale._1080height16x9 && source.PrimaryVideoStream.Height > (int)Scale._720height16x9)
			{
				if (scale_4k) UpScale(file, (int)Scale._4kwidth16x9, (int)Scale._4kheight16x9, outputNames[0]);
				if (scale_2k) UpScale(file, (int)Scale._2kwidth16x9, (int)Scale._2kheight16x9, outputNames[1]);
				if (scale_1080p) UpScale(file, (int)Scale._1080width16x9, (int)Scale._1080height16x9, outputNames[2]);
				if (scale_720p) DownScale(source, (int)Scale._720width16x9, (int)Scale._720height16x9, outputNames[3]);
				if (scale_480p) DownScale(source, (int)Scale._480width16x9, (int)Scale._480height16x9, outputNames[4]);
			}
			else if (source.PrimaryVideoStream.Height < (int)Scale._720height16x9 && source.PrimaryVideoStream.Height > (int)Scale._480height16x9)
			{
				if (scale_4k) UpScale(file, (int)Scale._4kwidth16x9, (int)Scale._4kheight16x9, outputNames[0]);
				if (scale_2k) UpScale(file, (int)Scale._2kwidth16x9, (int)Scale._2kheight16x9, outputNames[1]);
				if (scale_1080p) UpScale(file, (int)Scale._1080width16x9, (int)Scale._1080height16x9, outputNames[2]);
				if (scale_720p) UpScale(file, (int)Scale._720width16x9, (int)Scale._720height16x9, outputNames[3]);
				if (scale_480p) DownScale(source, (int)Scale._480width16x9, (int)Scale._480height16x9, outputNames[4]);
			}
			else
			{
				if (scale_4k) UpScale(file, (int)Scale._4kwidth16x9, (int)Scale._4kheight16x9, outputNames[0]);
				if (scale_2k) UpScale(file, (int)Scale._2kwidth16x9, (int)Scale._2kheight16x9, outputNames[1]);
				if (scale_1080p) UpScale(file, (int)Scale._1080width16x9, (int)Scale._1080height16x9, outputNames[2]);
				if (scale_720p) UpScale(file, (int)Scale._720width16x9, (int)Scale._720height16x9, outputNames[3]);
				if (scale_480p) UpScale(file, (int)Scale._480width16x9, (int)Scale._480height16x9, outputNames[4]);
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
					if (scale_4k) UpScale(file, (int)Scale._4kwidth4x3, (int)Scale._4kheight4x3, outputNames[0]);
					break;
				case (int)Scale._1080width4x3:
					if (scale_4k) UpScale(file, (int)Scale._4kwidth4x3, (int)Scale._4kheight4x3, outputNames[0]);
					if (scale_2k) UpScale(file, (int)Scale._2kwidth4x3, (int)Scale._2kheight4x3, outputNames[1]);
					break;
				case (int)Scale._720width4x3:
					if (scale_4k) UpScale(file, (int)Scale._4kwidth4x3, (int)Scale._4kheight4x3, outputNames[0]);
					if (scale_2k) UpScale(file, (int)Scale._2kwidth4x3, (int)Scale._2kheight4x3, outputNames[1]);
					if (scale_1080p) UpScale(file, (int)Scale._1080width4x3, (int)Scale._1080height4x3, outputNames[2]);
					break;
				case (int)Scale._480width4x3:
					if (scale_4k) UpScale(file, (int)Scale._4kwidth4x3, (int)Scale._4kheight4x3, outputNames[0]);
					if (scale_2k) UpScale(file, (int)Scale._2kwidth4x3, (int)Scale._2kheight4x3, outputNames[1]);
					if (scale_1080p) UpScale(file, (int)Scale._1080width4x3, (int)Scale._1080height4x3, outputNames[2]);
					if (scale_720p) UpScale(file, (int)Scale._720width4x3, (int)Scale._720height4x3, outputNames[3]);
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
					if (scale_2k) DownScale(ffprobeAnalysis, (int)Scale._2kwidth4x3, (int)Scale._2kheight4x3, outputNames[1]);
					if (scale_1080p) DownScale(ffprobeAnalysis, (int)Scale._1080width4x3, (int)Scale._1080height4x3, outputNames[2]);
					if (scale_720p) DownScale(ffprobeAnalysis, (int)Scale._720width4x3, (int)Scale._720height4x3, outputNames[3]);
					if (scale_480p) DownScale(ffprobeAnalysis, (int)Scale._480width4x3, (int)Scale._480height4x3, outputNames[4]);
					break;
				case (int)Scale._2kwidth4x3:
					if (scale_1080p) DownScale(ffprobeAnalysis, (int)Scale._1080width4x3, (int)Scale._1080height4x3, outputNames[2]);
					if (scale_720p) DownScale(ffprobeAnalysis, (int)Scale._720width4x3, (int)Scale._720height4x3, outputNames[3]);
					if (scale_480p) DownScale(ffprobeAnalysis, (int)Scale._480width4x3, (int)Scale._480height4x3, outputNames[4]);
					break;
				case (int)Scale._1080width4x3:
					if (scale_720p) DownScale(ffprobeAnalysis, (int)Scale._720width4x3, (int)Scale._720height4x3, outputNames[3]);
					if (scale_480p) DownScale(ffprobeAnalysis, (int)Scale._480width4x3, (int)Scale._480height4x3, outputNames[4]);
					break;
				case (int)Scale._720width4x3:
					if (scale_480p) DownScale(ffprobeAnalysis, (int)Scale._480width4x3, (int)Scale._480height4x3, outputNames[4]);
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
				if (scale_4k) DownScale(source, (int)Scale._4kwidth4x3, (int)Scale._4kheight4x3, outputNames[0]);
				if (scale_2k) DownScale(source, (int)Scale._2kwidth4x3, (int)Scale._2kheight4x3, outputNames[1]);
				if (scale_1080p) DownScale(source, (int)Scale._1080width4x3, (int)Scale._1080height4x3, outputNames[2]);
				if (scale_720p) DownScale(source, (int)Scale._720width4x3, (int)Scale._720height4x3, outputNames[3]);
				if (scale_480p) DownScale(source, (int)Scale._480width4x3, (int)Scale._480height4x3, outputNames[4]);
			}
			else if (source.PrimaryVideoStream.Height < (int)Scale._4kheight4x3 && source.PrimaryVideoStream.Height < (int)Scale._2kheight4x3)
			{
				if (scale_4k) UpScale(file, (int)Scale._4kwidth4x3, (int)Scale._4kheight4x3, outputNames[0]);
				if (scale_2k) DownScale(source, (int)Scale._2kwidth4x3, (int)Scale._2kheight4x3, outputNames[1]);
				if (scale_1080p) DownScale(source, (int)Scale._1080width4x3, (int)Scale._1080height4x3, outputNames[2]);
				if (scale_720p) DownScale(source, (int)Scale._720width4x3, (int)Scale._720height4x3, outputNames[3]);
				if (scale_480p) DownScale(source, (int)Scale._480width4x3, (int)Scale._480height4x3, outputNames[4]);
			}
			else if (source.PrimaryVideoStream.Height < (int)Scale._2kheight4x3 && source.PrimaryVideoStream.Height > (int)Scale._1080height4x3)
			{
				if (scale_4k) UpScale(file, (int)Scale._4kwidth4x3, (int)Scale._4kheight4x3, outputNames[0]);
				if (scale_2k) UpScale(file, (int)Scale._2kwidth4x3, (int)Scale._2kheight4x3, outputNames[1]);
			}
			else if (source.PrimaryVideoStream.Height < (int)Scale._1080height4x3 && source.PrimaryVideoStream.Height > (int)Scale._720height4x3)
			{
				if (scale_4k) UpScale(file, (int)Scale._4kwidth4x3, (int)Scale._4kheight4x3, outputNames[0]);
				if (scale_2k) UpScale(file, (int)Scale._2kwidth4x3, (int)Scale._2kheight4x3, outputNames[1]);
				if (scale_1080p) UpScale(file, (int)Scale._1080width4x3, (int)Scale._1080height4x3, outputNames[2]);
				if (scale_720p) DownScale(source, (int)Scale._720width4x3, (int)Scale._720height4x3, outputNames[3]);
				if (scale_480p) DownScale(source, (int)Scale._480width4x3, (int)Scale._480height4x3, outputNames[4]);
			}
			else if (source.PrimaryVideoStream.Height < (int)Scale._720height4x3 && source.PrimaryVideoStream.Height > (int)Scale._480height4x3)
			{
				if (scale_4k) UpScale(file, (int)Scale._4kwidth4x3, (int)Scale._4kheight4x3, outputNames[0]);
				if (scale_2k) UpScale(file, (int)Scale._2kwidth4x3, (int)Scale._2kheight4x3, outputNames[1]);
				if (scale_1080p) UpScale(file, (int)Scale._1080width4x3, (int)Scale._1080height4x3, outputNames[2]);
				if (scale_720p) UpScale(file, (int)Scale._720width4x3, (int)Scale._720height4x3, outputNames[3]);
				if (scale_480p) DownScale(source, (int)Scale._480width4x3, (int)Scale._480height4x3, outputNames[4]);
			}
			else
			{
				if (scale_4k) UpScale(file, (int)Scale._4kwidth4x3, (int)Scale._4kheight4x3, outputNames[0]);
				if (scale_2k) UpScale(file, (int)Scale._2kwidth4x3, (int)Scale._2kheight4x3, outputNames[1]);
				if (scale_1080p) UpScale(file, (int)Scale._1080width4x3, (int)Scale._1080height4x3, outputNames[2]);
				if (scale_720p) UpScale(file, (int)Scale._720width4x3, (int)Scale._720height4x3, outputNames[3]);
				if (scale_480p) UpScale(file, (int)Scale._480width4x3, (int)Scale._480height4x3, outputNames[4]);
			}
		}

		private void UpScale(string file, int upscaledWidth, int upscaledHeight, string output)
		{
			string yamlPath = $"{ Path.GetDirectoryName(exeFile) }\\video2x.yaml";
			DebugWriteLine($"Yaml Path: { yamlPath }");

			string commandArgs = $"-c { yamlPath } -i \"{ file }\" -w { upscaledWidth } -h { upscaledHeight } -d { driver } -p { processes } -o \"{ output }\"";

			//Console.WriteLine("Running Video2x with the following command: ");
			//Console.WriteLine($"{ exeFile } { commandArgs }");

			using (var video2xProcess = new Process())
			{
				video2xProcess.StartInfo.FileName = exeFile;
				video2xProcess.StartInfo.Arguments = commandArgs;

				video2xProcess.StartInfo.RedirectStandardInput = true;
				video2xProcess.StartInfo.RedirectStandardOutput = true;
				video2xProcess.StartInfo.RedirectStandardError = true;
				video2xProcess.StartInfo.UseShellExecute = false;
				video2xProcess.StartInfo.CreateNoWindow = true;
				//video2xProcess.StartInfo.WorkingDirectory = Environment.CurrentDirectory;

				video2xProcess.OutputDataReceived += (s, e) =>
				{
					if (e != null && string.IsNullOrWhiteSpace(e.Data) == false)
					{
						Console.WriteLine(e.Data);
					}
				};

				video2xProcess.ErrorDataReceived += (s, e) => 
				{
					if (e != null && string.IsNullOrWhiteSpace(e.Data) == false)
					{
						Console.WriteLine(e.Data);
					}
				};

				video2xProcess.Start();
				video2xProcess.BeginOutputReadLine();
				video2xProcess.BeginErrorReadLine();

				video2xProcess.StandardInput.WriteLine($"{ exeFile } { commandArgs }");
                video2xProcess.StandardInput.Flush();
                video2xProcess.StandardInput.Close();

				video2xProcess.WaitForExit();
			}
		}

		private void DownScale(IMediaAnalysis source, int downscaledWidth, int downscaledHeight, string output)
		{
			DebugWriteLine($"Source File: { source.Path }");
			DebugWriteLine($"DownScaled Width: { downscaledWidth }");
			DebugWriteLine($"DownScaled Height: { downscaledHeight }");
			DebugWriteLine($"Output: { output }");

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
					if (scale_4k) File.Copy(file, outputNames[0]);
					break;
				case (int)Scale._2kheight16x9:
					if (scale_2k) File.Copy(file, outputNames[1]);
					break;
				case (int)Scale._1080height16x9:
					if (scale_1080p) File.Copy(file, outputNames[2]);
					break;
				case (int)Scale._720height16x9:
					if (scale_720p) File.Copy(file, outputNames[3]);
					break;
				case (int)Scale._480height16x9:
					if (scale_480p) File.Copy(file, outputNames[4]);
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