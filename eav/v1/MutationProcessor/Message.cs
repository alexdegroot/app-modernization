namespace MutationProcessor
{
    public class Message
    {
        public Message(string messageId, string popReceipt, Change change)
        {
            MessageId = messageId;
            PopReceipt = popReceipt;
            Change = change;
        }

        public string MessageId { get; }
        public string PopReceipt { get; }
        public Change Change { get; }
    }
}