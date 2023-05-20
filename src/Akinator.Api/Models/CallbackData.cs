namespace Akinator.Api.Models
{
    internal class CallbackData
    {
        public const string MakeAnswerRequest = "MakeAnswerRequest";
        public const string ShowPossibleGuesses = "ShowPossibleGuesses";
        public const string StartNewGameRequest = "StartNewGameRequest";

        /// <summary>
        /// Who should process this callback data.
        /// </summary>
        public string Request { get; set; }

        public object Data { get; set; }
    }
}
