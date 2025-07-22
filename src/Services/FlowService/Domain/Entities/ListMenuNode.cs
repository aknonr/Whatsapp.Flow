using System.Collections.Generic;

namespace Whatsapp.Flow.Services.Flow.Domain.Entities
{
    public class ListMenuNode : FlowNode
    {
        public string HeaderText { get; set; }
        public string BodyText { get; set; }
        public string FooterText { get; set; }
        public string ButtonText { get; set; }
        public List<ListSection> Sections { get; set; } = new List<ListSection>();

        public ListMenuNode()
        {
            NodeType = "listMenu";
        }
    }

    public class ListSection
    {
        public string Title { get; set; }
        public List<ListRow> Rows { get; set; } = new List<ListRow>();
    }

    public class ListRow
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
} 