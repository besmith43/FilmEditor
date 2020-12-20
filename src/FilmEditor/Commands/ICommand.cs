using System.Threading.Tasks;

namespace FilmEditor.Commands
{
    public interface ICommand
    {
        Task<bool> ExecuteAsync();
    }
}