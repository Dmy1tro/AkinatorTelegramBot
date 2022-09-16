using Akinator.Core.Interfaces;
using Akinator.Core.Models.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Akinator.Core.Tests
{
    public class AkinatorClientTests
    {
        private static readonly Random _random = new Random();

        [Fact]
        public async Task Should_Proceed_Full_Flow_Successfully_When_EN_Region()
        {
            // Arrange
            var services = new ServiceCollection()
                .AddAkinator(options => options.Region = Region.En);
            var provider = services.BuildServiceProvider();
            var client = provider.GetRequiredService<IAkinatorClient>();
            var totalSteps = 12;

            // Let's go!
            Assert.NotNull(client);

            var game = await client.StartNewGame();
            Assert.NotNull(game);

            // Simulating real game
            for (int i = 0; i < totalSteps; i++)
            {
                var question = game.GetQuestion();
                Assert.False(string.IsNullOrEmpty(question));

                var answers = game.GetAnswers();
                Assert.NotEmpty(answers);
                Assert.Equal(5, answers.Count);

                // Check that answers Id are sortered by ascending and starting from zero
                // Yes = 0
                // No = 1
                // I dont know = 2
                // Probably = 3
                // Probably not = 4
                for (int answerIndex = 0; answerIndex < answers.Count; answerIndex++)
                {
                    var answer = answers[answerIndex];
                    var expectedText = GetExpectedAnswerText(answerIndex);
                    var expectedId = answerIndex;

                    Assert.Equal(expectedText, answer.Text, ignoreCase: true, ignoreWhiteSpaceDifferences: true);
                    Assert.Equal(expectedId, answer.Id);

                    string GetExpectedAnswerText(int id)
                    {
                        return id switch
                        {
                            0 => "Yes",
                            1 => "No",
                            2 => "Don't know",
                            3 => "Probably",
                            4 => "Probably not"
                        };
                    }
                }

                // Simulating user choice
                await game.Answer(_random.Next(0, 5));
            }

            // Check that Back works
            await game.Back();
            var step = game.GetStep();
            Assert.Equal(totalSteps - 1, step);

            await game.Answer(_random.Next(0, 5));
            step = game.GetStep();
            Assert.Equal(totalSteps, step);

            // I hope 12 steps enough for akinator to guess some items =)
            var guessedItems = await game.Win();
            Assert.NotEmpty(guessedItems);
        }
    }
}