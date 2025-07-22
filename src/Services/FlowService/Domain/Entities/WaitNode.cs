using System;

namespace Whatsapp.Flow.Services.Flow.Domain.Entities
{
    public class WaitNode : FlowNode
    {
        /// <summary>
        /// Bekleme süresi (saniye cinsinden)
        /// </summary>
        public int DurationInSeconds { get; set; }
        
        /// <summary>
        /// Bekleme türü: "delay" (sabit bekleme), "schedule" (belirli zamana kadar bekle)
        /// </summary>
        public string WaitType { get; set; } = "delay";
        
        /// <summary>
        /// Eğer WaitType "schedule" ise, hedef zaman
        /// </summary>
        public DateTime? ScheduledTime { get; set; }

        public WaitNode()
        {
            NodeType = "wait";
        }
    }
} 