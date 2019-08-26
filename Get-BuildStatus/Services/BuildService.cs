using Get_BuildStatus.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Get_BuildStatus.Services
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

        public async ValueTask<IList<BuildReport>> GetFailedBuilds()
        {
            var url = $"{account}/{teamProjectName}/_apis/build/definitions?api-version={version}";

            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var builds = JsonConvert.DeserializeObject<AllBuilds>(await response.Content.ReadAsStringAsync());

            var filterdBuilds = builds.Value
                .Where(build => !build.Name.Contains("gated") && !build.Name.StartsWith("DB."))
                .ToList();

            return await GetFailedOrNotRunnedBuildsDetails(filterdBuilds);
        }

        /// <summary>
        /// Retorna uma tupla com uma lista de builds que foram executados com sucesso e builds que não foram executados com sucesso 
        /// </summary>
        /// <param name="buildDefinitions"></param>
        /// <returns>d</returns>
        public async  ValueTask<IList<BuildReport>> GetFailedOrNotRunnedBuildsDetails(IList<Value> buildDefinitions)
        {
            // https://dev.azure.com/mongeral/projetos/_apis/build/latest/9?api-version=5.0-preview.1
            List<BuildReport> failedOrNotRunnedBuilds = new List<BuildReport>();
            

            foreach (var buildDefinition in buildDefinitions)
            {
                var url = $"{account}/{teamProjectName}/_apis/build/latest/{buildDefinition.Id}?api-version=5.0-preview.1";
                var response = await _client.GetAsync(url);

                BuildReport buildDetail;
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // TODO Add not runned builds 
                    buildDetail = new BuildReport
                    {
                        DefinitionName = buildDefinition.Name
                    };
                    failedOrNotRunnedBuilds.Add(buildDetail);
                }
                else
                {
                    var build = JsonConvert.DeserializeObject<Build>(await response.Content.ReadAsStringAsync());
                    if (!build.Result.Equals("succeeded", StringComparison.OrdinalIgnoreCase))
                    {
                        buildDetail = new BuildReport
                        {
                            DefinitionName = buildDefinition.Name,
                            BuildNumber = build.BuildNumber,
                            FinishTime = build.FinishTime,
                            Result = build.Result
                        };
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

