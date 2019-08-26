using System;
using System.Collections.Generic;
using System.Text;

namespace Get_BuildStatus.Model
{
    public class BuildReport
    {
        public string DefinitionName { get; set; }
        public string BuildNumber { get; set; }
        public DateTime FinishTime { get; set; }
        public string Result { get; set; }
    }
}
