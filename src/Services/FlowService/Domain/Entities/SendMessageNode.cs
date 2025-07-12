namespace Whatsapp.Flow.Services.Flow.Domain.Entities
{
    public class SendMessageNode : FlowNode
    {
        public string Content { get; set; }

        public SendMessageNode()
        {
            NodeType = "sendMessage"; // UI ve akış motoru bu tipi tanıyacak
        }
    }
} 