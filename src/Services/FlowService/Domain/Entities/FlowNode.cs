using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Whatsapp.Flow.Services.Flow.Domain.Entities
{
    [JsonDerivedType(typeof(SendMessageNode), typeDiscriminator: "sendMessage")]
    [JsonDerivedType(typeof(DecisionNode), typeDiscriminator: "decision")]
    [JsonDerivedType(typeof(ListMenuNode), typeDiscriminator: "listMenu")]
    [JsonDerivedType(typeof(ButtonNode), typeDiscriminator: "button")]
    [JsonDerivedType(typeof(WaitNode), typeDiscriminator: "wait")]
    [JsonDerivedType(typeof(WebhookNode), typeDiscriminator: "webhook")]
    [JsonDerivedType(typeof(AskQuestionNode), typeDiscriminator: "askQuestion")]
    [JsonDerivedType(typeof(NoteNode), typeDiscriminator: "note")]
    public abstract class FlowNode
    {
        public string Id { get; set; } = System.Guid.NewGuid().ToString();
        public string NodeType { get; protected set; }
        public Position Position { get; set; }
        public List<FlowOutput> Outputs { get; set; } = new List<FlowOutput>();
    }

    public class Position
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class FlowOutput
    {
        public string SourceHandle { get; set; } // Bu adımın hangi çıkış noktasından ("yes", "no", "default" vb.)
        public string TargetNodeId { get; set; } // Hangi hedef adıma bağlandığı
    }
} 