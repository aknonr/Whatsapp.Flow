namespace Whatsapp.Flow.Services.Flow.Domain.Entities
{
    public class AskQuestionNode : FlowNode
    {
        public string Content { get; set; }
        
        /// <summary>
        /// Gelen cevabın kaydedileceği değişkenin adı.
        /// </summary>
        public string VariableName { get; set; }

        public AskQuestionNode()
        {
            NodeType = "askQuestion";
        }
    }
} 