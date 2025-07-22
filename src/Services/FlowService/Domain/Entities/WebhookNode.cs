using System.Collections.Generic;

namespace Whatsapp.Flow.Services.Flow.Domain.Entities
{
    public class WebhookNode : FlowNode
    {
        public string Url { get; set; }
        public string Method { get; set; } = "POST";
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        
        /// <summary>
        /// Request body template - {{variableName}} formatında değişkenler kullanılabilir
        /// </summary>
        public string BodyTemplate { get; set; }
        
        /// <summary>
        /// Response'u hangi değişkene kaydedeceğimiz
        /// </summary>
        public string ResponseVariableName { get; set; }
        
        /// <summary>
        /// Response mapping - API'den dönen değerleri flow değişkenlerine map etmek için
        /// </summary>
        public Dictionary<string, string> ResponseMapping { get; set; } = new Dictionary<string, string>();

        public WebhookNode()
        {
            NodeType = "webhook";
        }
    }
} 