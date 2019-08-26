using Get_BuildStatus.Services;
using System;

namespace Get_BuildStatus
{
    class Program
    {
        static void Main(string[] args)
        {
            BuildService buildService = new BuildService();

            var buildsFailed = buildService.GetFailedBuilds().AsTask().Result;
            foreach (var item in buildsFailed)
            {
                Console.WriteLine($"{item.DefinitionName}{item.BuildNumber}{item.FinishTime}{item.Result}");
            }
            Console.ReadKey();
        }
    }
}
