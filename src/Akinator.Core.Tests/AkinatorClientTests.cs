using Akinator.Core.Interfaces;
using Akinator.Core.Models.Game;
using Microsoft.Extensions.DependencyInjection;

namespace Akinator.Core.Tests
{
    public class AkinatorClientTests
    {
        [Fact]
        public async Task Should_Proceed_Full_Flow_Successfully()
        {
            // Arrange
            var services = new ServiceCollection()
                .AddAkinator();
            var provider = services.BuildServiceProvider();
            var client = provider.GetRequiredService<IAkinatorClient>();
            var totalSteps = 12;

            // Let's go!
            Assert.NotNull(client);

            var game = await client.StartNewGame();
            Assert.NotNull(game);

            // Validate snapshot feature
            var snapshot = game.CreateSnapshot();
            var serializedSnapshot = snapshot.Serialize();
            var deserializedSnapshot = GameSnapshot.Deserialize(serializedSnapshot);

            Assert.Equal(snapshot.SnapshotId, deserializedSnapshot.SnapshotId);
            Assert.Equal(snapshot.CreatedAt, deserializedSnapshot.CreatedAt);

            var gameFromSnapshot = client.LoadGameFromSnapshot(deserializedSnapshot);

            Assert.Equal(game.GetStep(), gameFromSnapshot.GetStep());
            Assert.Equal(game.GetQuestion(), gameFromSnapshot.GetQuestion());

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
                await game.Answer(Random.Shared.Next(0, 5));
            }

            Assert.NotEqual(game.GetStep(), gameFromSnapshot.GetStep());

            // Check that Back works
            await game.Back();
            var step = game.GetStep();
            Assert.Equal(totalSteps - 1, step);

            await game.Answer(Random.Shared.Next(0, 5));
            step = game.GetStep();
            Assert.Equal(totalSteps, step);

            // I hope 12 steps enough for akinator to guess some items =)
            var guessedItems = await game.Win();
            Assert.NotEmpty(guessedItems);
        }
    }
}