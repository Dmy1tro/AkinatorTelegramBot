using System.Collections.Generic;
using System.Threading.Tasks;
using Akinator.Core.Models.Game;

namespace Akinator.Core.Interfaces
{
    public interface IAkinatorGame
    {
        string GetQuestion();

        ICollection<string> GetAnswers();

        Task Answer(int answerId);

        Task Back();

        bool CanGuess();

        Task<ICollection<GuessedItem>> Win();
    }
}
