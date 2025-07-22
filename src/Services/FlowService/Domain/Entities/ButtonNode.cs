using System.Collections.Generic;

namespace Whatsapp.Flow.Services.Flow.Domain.Entities
{
    public class ButtonNode : FlowNode
    {
        public string HeaderText { get; set; }
        public string BodyText { get; set; }
        public string FooterText { get; set; }
        public List<ButtonOption> Buttons { get; set; } = new List<ButtonOption>();

        public ButtonNode()
        {
            NodeType = "button";
        }
    }

    public class ButtonOption
    {
        public string Id { get; set; }
        public string Title { get; set; }
        /// <summary>
        /// Bu butona tıklandığında hangi çıkışa yönlendirileceğini belirtir.
        /// FlowNode'daki Outputs listesindeki SourceHandle ile eşleşmelidir.
        /// </summary>
        public string TargetOutputHandle { get; set; }
    }
} 