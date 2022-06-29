using System.Net.Http.Json;

namespace ProjectsTracker.Controllers {

    public abstract class ServerConnector {
        protected HttpClient client;
        private string path;

        public ServerConnector(HttpClient client, string path) {
            this.client = client;
            this.path = path;
        }

        public async Task<List<T>> GetList<T>(){
            var result = await client.GetAsync(path);
            var proj = await result.Content.ReadFromJsonAsync<List<T>>();
            return proj;
        }

        public async Task<T> GetById<T>(int id) {
            var result = await client.GetAsync(path + $"/{id}");
            return await result.Content.ReadFromJsonAsync<T>();
        }

        public async Task<HttpResponseMessage> Post<T>(T t) {
            var result = await client.PostAsync(path, JsonContent.Create(t));
            return result;
        }

        public async Task<HttpResponseMessage> Put<T>(T t) {
            var result = await client.PutAsync(path, JsonContent.Create(t));
            return result;
        }

        public async Task<HttpResponseMessage> Delete(int id) {
            var result = await client.DeleteAsync(path + $"/{id}");
            return result;
        }
    }
}