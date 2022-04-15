namespace SummarisationSample.ActivityService.Service.Contracts
{
    internal class MessageRefGenerator
    {
        internal static string Generate()
        {
            string messageRef;

            messageRef = DateTime.Now.ToString("o") + Guid.NewGuid().ToString("N");

            return messageRef;
        }
    }
}