using System.Collections.Generic;

namespace Whatsapp.Flow.Services.Flow.Domain.Entities
{
    public class DecisionNode : FlowNode
    {
        public List<Condition> Conditions { get; set; } = new List<Condition>();
        
        public DecisionNode()
        {
            NodeType = "decision";
        }
    }

    public class Condition
    {
        /// <summary>
        /// Gelen mesajda aranacak anahtar kelime veya metin.
        /// </summary>
        public string ExpectedInput { get; set; }

        /// <summary>
        /// Bu koşul sağlandığında hangi çıkışa yönlendirileceğini belirtir.
        /// FlowNode'daki Outputs listesindeki SourceHandle ile eşleşmelidir.
        /// </summary>
        public string TargetOutputHandle { get; set; }
    }
} 