﻿using System.Threading.Tasks;
using Akinator.Core.Helpers;
using Akinator.Core.Interfaces;
using Akinator.Core.Models.AkinatorResponse;
using Akinator.Core.Models.Game;
using Akinator.Core.Models.Options;
using Microsoft.Extensions.Options;

namespace Akinator.Core.Services
{
    internal class AkinatorClient : IAkinatorClient
    {
        private readonly AkinatorOptions _options;
        private readonly IAkinatorHttpClient _akinatorHttpClient;

        public AkinatorClient(IAkinatorHttpClient akinatorHttpClient, IOptions<AkinatorOptions> options)
        {
            _options = options.Value;
            _akinatorHttpClient = akinatorHttpClient;
        }

        public async Task<IAkinatorGame> StartNewGame()
        {
            var gameState = new GameState();

            gameState.BaseUrl = AkinatorUrlBuilder.BaseUrl(_options.Region.ToString().ToLower());
            gameState.RegionUrl = await _akinatorHttpClient.GetRegionUrl(gameState.BaseUrl);
            gameState.Session = await _akinatorHttpClient.GetSession(AkinatorUrlBuilder.Session());

            var url = AkinatorUrlBuilder.StartGame(gameState);
            var startGameResponse = await _akinatorHttpClient.GetCallbackResponse<StartGameResponse>(url);

            gameState.Identification = startGameResponse.Parameters.Identification;
            gameState.StepInformation = startGameResponse.Parameters.StepInformation;

            return new AkinatorGame(_akinatorHttpClient, gameState);
        }
    }
}
