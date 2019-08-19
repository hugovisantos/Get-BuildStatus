using System;

namespace Get_BuildStatus
{
    class Program
    {
        static void Main(string[] args)
        {
            BuildService buildService = new BuildService();

            var buildsFailed = buildService.GetFailedBuilds();
            var buildFailedDetail = buildService.GetFailedBuildsDetails(buildsFailed.Result);
        }
    }
}
