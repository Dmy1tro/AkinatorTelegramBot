using System.Collections.Generic;
using System.Threading.Tasks;
using Akinator.Core.Models.Game;

namespace Akinator.Core.Interfaces
{
    public interface IAkinatorGame
    {
        string GetQuestion();

        IList<Answer> GetAnswers();

        Task Answer(int answerId);

        Task Back();

        bool CanGuess();

        float GetProgress();

        int GetStep();

        Task<IList<GuessedItem>> Win();

        GameSnapshot CreateSnapshot();
    }
}
