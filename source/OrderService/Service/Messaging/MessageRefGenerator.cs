namespace SummarisationSample.OrderService.Service.Messaging
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