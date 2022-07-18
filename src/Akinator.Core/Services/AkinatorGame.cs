using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Akinator.Core.Exceptions;
using Akinator.Core.Helpers;
using Akinator.Core.Interfaces;
using Akinator.Core.Models.AkinatorResponse;
using Akinator.Core.Models.Game;

namespace Akinator.Core.Services
{
    internal class AkinatorGame : IAkinatorGame
    {
        private readonly HttpClient _httpClient;
        private readonly GameState _gameState;

        public AkinatorGame(HttpClient httpClient, GameState gameState)
        {
            _httpClient = httpClient;
            _gameState = gameState;
        }

        public string GetQuestion()
        {
            return _gameState.StepInformation.Question;
        }

        public ICollection<string> GetAnswers()
        {
            return _gameState.StepInformation.Answers.Select(a => a.Text).ToList();
        }

        public async Task Answer(int answerId)
        {
            var url = AkinatorUrlBuilder.Answer(_gameState, answerId);
            var stepResponse = await _httpClient.GetAkinatorCallbackResponse<StepResponse>(url);

            Validate(stepResponse.Completion);

            _gameState.StepInformation = stepResponse.Parameters.ToStepInformation();
        }

        public async Task Back()
        {
            var url = AkinatorUrlBuilder.Back(_gameState);
            var stepResponse = await _httpClient.GetAkinatorCallbackResponse<StepResponse>(url);

            Validate(stepResponse.Completion);

            _gameState.StepInformation = stepResponse.Parameters.ToStepInformation();
        }

        public bool CanGuess()
        {
            var progression = float.Parse(_gameState.StepInformation.Progression);

            return progression > 90;
        }

        public async Task<ICollection<GuessedItem>> Win()
        {
            var url = AkinatorUrlBuilder.Win(_gameState);
            var winResponse = await _httpClient.GetAkinatorCallbackResponse<WinResponse>(url);

            var guessedItems = winResponse.Parameters.Elements
                .Select(e => e.Element)
                .Select(e => new GuessedItem
                {
                    Name = e.Name
                }).ToList();

            return guessedItems;
        }

        private void Validate(string completion)
        {
            if (completion.ToUpper() != "OK")
            {
                // TODO: provide message.
                throw new AkinatorException();
            }
        }
    }
}
