namespace Akinator.Api.Models
{
    internal class CallbackData
    {
        /// <summary>
        /// Who should process this callback data.
        /// </summary>
        public string Request { get; set; }

        public object Data { get; set; }
    }
}
