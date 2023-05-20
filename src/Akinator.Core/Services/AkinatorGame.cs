using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akinator.Core.Exceptions;
using Akinator.Core.Helpers;
using Akinator.Core.Interfaces;
using Akinator.Core.Models.AkinatorResponse;
using Akinator.Core.Models.Game;
using Newtonsoft.Json;

namespace Akinator.Core.Services
{
    internal class AkinatorGame : IAkinatorGame
    {
        private readonly IAkinatorHttpClient _akinatorHttpClient;
        private readonly GameState _gameState;

        public AkinatorGame(IAkinatorHttpClient akinatorHttpClient, GameState gameState)
        {
            _akinatorHttpClient = akinatorHttpClient;
            _gameState = gameState;
        }

        public string GetQuestion()
        {
            return _gameState.StepInformation.Question;
        }

        public IList<Answer> GetAnswers()
        {
            return _gameState.StepInformation.Answers.Select((a, i) => new Answer { Text = a.Text, Id = i }).ToList();
        }

        public async Task Answer(int answerId)
        {
            var url = AkinatorUrlBuilder.Answer(_gameState, answerId);
            var stepResponse = await _akinatorHttpClient.GetCallbackResponse<StepResponse>(url);

            ValidateStepResponse(stepResponse);

            _gameState.StepInformation = stepResponse.Parameters.ToStepInformation();
        }

        public async Task Back()
        {
            var url = AkinatorUrlBuilder.Back(_gameState);
            var stepResponse = await _akinatorHttpClient.GetCallbackResponse<StepResponse>(url);

            ValidateStepResponse(stepResponse);

            _gameState.StepInformation = stepResponse.Parameters.ToStepInformation();
        }

        public int GetStep()
        {
            return int.Parse(_gameState.StepInformation.Step);
        }

        public float GetProgress()
        {
            return float.Parse(_gameState.StepInformation.Progression);
        }

        public bool CanGuess()
        {
            var progression = GetProgress();

            return progression > 90;
        }

        public async Task<IList<GuessedItem>> Win()
        {
            var url = AkinatorUrlBuilder.Win(_gameState);
            var winResponse = await _akinatorHttpClient.GetCallbackResponse<WinResponse>(url);

            var guessedItems = winResponse.Parameters.Elements
                .Select(e => e.Element)
                .Select(e => new GuessedItem
                {
                    Name = e.Name
                }).ToList();

            return guessedItems;
        }

        public GameSnapshot CreateSnapshot()
        {
            // Prevent any unintended reference changes to the original object.
            var state = _gameState.Copy();

            return new GameSnapshot(state);
        }

        private void ValidateStepResponse(StepResponse response)
        {
            if (!response.IsSuccess)
            {
                throw new AkinatorException($"Failed to proceed action. Response: {response.Completion}.");
            }
        }
    }
}
