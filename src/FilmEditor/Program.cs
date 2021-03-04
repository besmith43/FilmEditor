using System;
using System.IO;
using System.Linq;
using FilmEditor.cmd;

namespace FilmEditor
{
    class Program
    {
        public static string WindowsLocalDir = $"C:\\Users\\{ Environment.UserName }\\AppData\\Local\\FilmEditor";
        public static string LinuxLocalDir = $"/home/{ Environment.UserName }/.local/filmeditor";
        public static string MacOSLocalDir = $"/Users/{ Environment.UserName }/.local/filmeditor";
        public static Options cmdFlags;
        static void Main(string[] args)
        {
            cmdParser cmdP = new(args);

            cmdFlags = cmdP.Parse();  

            if (cmdFlags.versionFlag)
            {
                Console.WriteLine(cmdFlags.versionText);
                return;
            }

			if (!CheckForFFMpeg())
            {
                Console.WriteLine("FFMpeg is not installed");
                return;
            }

            Run();
        }

        public static void Run()
        {
            if (cmdFlags.command != null)
            {
                var success = cmdFlags.command.ExecuteAsync().Result;

                if (!success)
                {
                    Environment.Exit(-1);
                }
            }
            else
            {
                Console.WriteLine(cmdFlags.helpText);
            }
        }

        private static bool CheckForFFMpeg()
        {
            if (CommandExists("ffmpeg.exe") && CommandExists("ffprobe.exe"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CommandExists(string command)
		{
			return GetFullPath(command) != null;
		}

		public static string GetFullPath(string fileName)
		{
		    if (File.Exists(fileName))
		        return Path.GetFullPath(fileName);
		
		    var values = Environment.GetEnvironmentVariable("PATH");
		    foreach (var path in values.Split(Path.PathSeparator))
		    {
		        var fullPath = Path.Combine(path, fileName);
		        if (File.Exists(fullPath))
		            return fullPath;
		    }
		    return null;
		}

        public static string GetLocalConfig()
		{
			if (IsWindows())
			{
                if (!Directory.Exists(WindowsLocalDir))
                {
                    Directory.CreateDirectory(WindowsLocalDir);
                }

				return $"{ WindowsLocalDir }\\config.csv";
			}
			else if (IsLinux())
			{
                if (!Directory.Exists(LinuxLocalDir))
                {
                    Directory.CreateDirectory(LinuxLocalDir);
                }

				return $"{ LinuxLocalDir }/config.csv";
			}
			else
			{
                if (!Directory.Exists(MacOSLocalDir))
                {
                    Directory.CreateDirectory(MacOSLocalDir);
                }

				return $"{ MacOSLocalDir }/config.csv";
			}
		}

		public static bool IsWindows() =>
            System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);

		public static bool IsLinux() =>
            System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux);
    }
}
