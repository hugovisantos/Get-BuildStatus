using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Get_BuildStatus
{
    public class BuildService : IDisposable
    {
        private const string account = "mongeral";
        private const string teamProjectName = "Projetos";
        private const string version = "4.1";
        private readonly HttpClient _client;
        private bool _disposed;
        private const string token = "7zz32gwaryit32vwz45nb4xbojsgsk6zweshdycs6mecuaxnh7ea";
        private const string baseUri = "https://dev.azure.com/";

        public BuildService()
        {
            _client = new HttpClient()
            {
                BaseAddress = new Uri(baseUri)
            };

            _client.DefaultRequestHeaders.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(
                    System.Text.ASCIIEncoding.ASCII.GetBytes(
                        string.Format("{0}:{1}", "", token))));
        }

        public async ValueTask<IList<Build>> GetFailedBuilds()
        {
            var url = $"{account}/{teamProjectName}/_apis/build/definitions?api-version={version}";

            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var builds = JsonConvert.DeserializeObject<AllBuilds>(await response.Content.ReadAsStringAsync());

            var filterdBuilds = builds.value
                .Where(build => !build.name.Contains("gated") && !build.name.StartsWith("DB."))
                .ToList();

            return await GetFailedBuildsDetails(filterdBuilds);
        }

        /// <summary>
        /// Retorna uma tupla com uma lista de builds que foram executados com sucesso e builds que não foram executados com sucesso 
        /// </summary>
        /// <param name="builds"></param>
        /// <returns>d</returns>
        public async ValueTask<IList<Build>> GetFailedOrNotRunnedBuildsDetails(IList<Value> builds)
        {
            // https://dev.azure.com/mongeral/projetos/_apis/build/latest/9?api-version=5.0-preview.1
            List<Build> failedOrNotRunnedBuilds = new List<Build>();
            

            foreach (var build in builds)
            {
                var url = $"{account}/{teamProjectName}/_apis/build/latest/{build.id}?api-version=5.0-preview.1";
                var response = await _client.GetAsync(url);

                Build buildDetail;
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // TODO Add not runned builds 
                    buildDetail = new Build(); 
                }
                else
                {
                    buildDetail = JsonConvert.DeserializeObject<Build>(await response.Content.ReadAsStringAsync());
                    if (!buildDetail.result.Equals("succeeded", StringComparison.OrdinalIgnoreCase))
                    {
                        failedOrNotRunnedBuilds.Add(buildDetail);
                    }
                }
            }

            return failedOrNotRunnedBuilds;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _client?.Dispose();
            }

            _disposed = true;
        }
    }
}



public class AllBuilds
{
    public int count { get; set; }
    public Value[] value { get; set; }
}

public class Value
{
    public string quality { get; set; }
    public object[] drafts { get; set; }
    public Queue queue { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public string url { get; set; }
    public string uri { get; set; }
    public string path { get; set; }
    public string type { get; set; }
    public string queueStatus { get; set; }
    public int revision { get; set; }
    public DateTime createdDate { get; set; }
    public Draftof draftOf { get; set; }
}

public class Queue
{
    public int id { get; set; }
    public string name { get; set; }
    public string url { get; set; }
}

public class Draftof
{
    public int id { get; set; }
    public string name { get; set; }
    public string url { get; set; }
    public string uri { get; set; }
    public string type { get; set; }
    public string queueStatus { get; set; }
    public int revision { get; set; }
}

public class Build
{
    public object[] tags { get; set; }
    public object[] validationResults { get; set; }
    public int id { get; set; }
    public string buildNumber { get; set; }
    public string status { get; set; }
    public string result { get; set; }
    public DateTime queueTime { get; set; }
    public DateTime startTime { get; set; }
    public DateTime finishTime { get; set; }
    public string url { get; set; }
    public Definition definition { get; set; }
    public int buildNumberRevision { get; set; }

}

public class Definition
{
    public object[] drafts { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public string url { get; set; }
    public string uri { get; set; }
    public string path { get; set; }
    public string type { get; set; }
    public string queueStatus { get; set; }
    public int revision { get; set; }
}

public class Orchestrationplan
{
    public string planId { get; set; }
}
