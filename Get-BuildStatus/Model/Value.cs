using System;
using System.Collections.Generic;
using System.Text;

namespace Get_BuildStatus.Model
{
    public class Value
    {
        public string Quality { get; set; }
        public object[] Drafts { get; set; }
        public Queue Queue { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Uri { get; set; }
        public string Path { get; set; }
        public string Type { get; set; }
        public string QueueStatus { get; set; }
        public int Revision { get; set; }
        public DateTime CreatedDate { get; set; }
        public Draftof DraftOf { get; set; }
    }
}
