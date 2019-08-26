using System;
using System.Collections.Generic;
using System.Text;

namespace Get_BuildStatus.Model
{
    public class Build
    {
        public object[] Tags { get; set; }
        public object[] validationResults { get; set; }
        public int Id { get; set; }
        public string BuildNumber { get; set; }
        public string Status { get; set; }
        public string Result { get; set; }
        public DateTime QueueTime { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
        public string Url { get; set; }
        public Definition Definition { get; set; }
        public int BuildNumberRevision { get; set; }

    }
}
