using System.Collections.Generic;

namespace Whatsapp.Flow.Services.Flow.Domain.Entities
{
    public class DecisionNode : FlowNode
    {
        public List<Condition> Conditions { get; set; } = new List<Condition>();
        public string DefaultOutputHandle { get; set; } // Hiçbir koşul sağlanmazsa
        
        public DecisionNode()
        {
            NodeType = "decision";
        }
    }

    public class Condition
    {
        /// <summary>
        /// Koşul türü: "exact", "contains", "regex", "number", "date"
        /// </summary>
        public string ConditionType { get; set; } = "contains";
        
        /// <summary>
        /// Karşılaştırma operatörü: "equals", "contains", "starts_with", "ends_with", "greater_than", "less_than"
        /// </summary>
        public string Operator { get; set; } = "contains";
        
        /// <summary>
        /// Beklenen değer
        /// </summary>
        public string ExpectedValue { get; set; }
        
        /// <summary>
        /// Karşılaştırılacak değişken adı (boşsa gelen mesaj kullanılır)
        /// </summary>
        public string VariableName { get; set; }
        
        /// <summary>
        /// Bu koşul sağlandığında hangi çıkışa yönlendirileceğini belirtir
        /// </summary>
        public string TargetOutputHandle { get; set; }
        
        /// <summary>
        /// Koşul önceliği (düşük sayı = yüksek öncelik)
        /// </summary>
        public int Priority { get; set; } = 0;
        
        /// <summary>
        /// Koşul açıklaması
        /// </summary>
        public string Description { get; set; }
    }
} 