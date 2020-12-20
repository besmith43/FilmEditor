using System;
using System.IO;
using System.Linq;
using FilmEditor.cmd;

namespace FilmEditor
{
    class Program
    {
        public static Options cmdFlags;
        static void Main(string[] args)
        {
            if (!CheckForFFMpeg())
            {
                Console.WriteLine("FFMpeg is not installed");
                return;
            }

            cmdParser cmdP = new(args);

            cmdFlags = cmdP.Parse();  

            if (cmdFlags.versionFlag)
            {
                Console.WriteLine(cmdFlags.versionText);
                return;
            }

            if (cmdFlags.helpFlag)
            {
                Console.WriteLine(cmdFlags.helpText);
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
    }
}
