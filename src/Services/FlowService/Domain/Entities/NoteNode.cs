namespace Whatsapp.Flow.Services.Flow.Domain.Entities
{
    public class NoteNode : FlowNode
    {
        /// <summary>
        /// Not içeriği
        /// </summary>
        public string Content { get; set; }
        
        /// <summary>
        /// Not rengi (UI'da görsel ayırım için)
        /// </summary>
        public string Color { get; set; } = "#FFD700"; // Varsayılan altın sarısı
        
        /// <summary>
        /// Not önceliği (1-5 arası)
        /// </summary>
        public int Priority { get; set; } = 3;
        
        /// <summary>
        /// Not kategorisi
        /// </summary>
        public string Category { get; set; } = "Genel";
        
        /// <summary>
        /// Not oluşturulma tarihi
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Not oluşturan kullanıcı
        /// </summary>
        public string CreatedBy { get; set; }

        public NoteNode()
        {
            NodeType = "note";
        }
    }
} 