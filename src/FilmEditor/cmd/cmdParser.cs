using System.Linq;
using FilmEditor.Commands;

namespace FilmEditor.cmd
{
    public class cmdParser
    {
        private string[] _args;
        private Options opts;

        public cmdParser(string[] Args)
        {
            _args = Args;
        }

        public Options Parse()
        {
            opts = new();

            if (_args != null)
            {
                switch (_args[0])
                {
                    case NewCommand.COMMAND_NAME:
                        opts.command = new NewCommand(_args.Skip(1).ToArray());
                        break;
                    case SplitCommand.COMMAND_NAME:
                        opts.command = new SplitCommand(_args.Skip(1).ToArray());
                        break;
                    case EditCommand.COMMAND_NAME:
                        opts.command = new EditCommand(_args.Skip(1).ToArray());
                        break;
                    case RenameCommand.COMMAND_NAME:
                        opts.command = new RenameCommand(_args.Skip(1).ToArray());
                        break;
                    case ScaleCommand.COMMAND_NAME:
                        opts.command = new ScaleCommand(_args.Skip(1).ToArray());
                        break;
                    default:
                        opts.helpFlag = true;
                        break;
                }

                for (int i = 0; i < _args.Length; i++)
                {
                    if (_args[i] == "-h" || _args[i] == "--help")
                    {
                        opts.helpFlag = true;
                    }
                    else if (_args[i] == "-V" || _args[i] == "--version")
                    {
                        opts.versionFlag = true;
                    }
                    else if (_args[i] == "-v" || _args[i] == "--verbose")
                    {
                        opts.verbose = true;
                    }
                    else
                    {
                        // place any unnamed or positional value checks
                    }
                }
            }

            return opts;
        }
    }
}