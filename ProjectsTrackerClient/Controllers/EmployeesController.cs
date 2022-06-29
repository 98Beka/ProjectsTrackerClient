using ProjectsTracker.Models;
using System.Net.Http.Json;

namespace ProjectsTracker.Controllers {

    public class EmployeesController : ServerConnector {
        private string path = "Employees";
        public EmployeesController(HttpClient client) : base(client, "Employees") {

        }

        public async Task<HttpResponseMessage> AddProject(int employeeId, int projectId) {
            var result = await base.client.GetAsync($"{path}/addProject?employeeId={employeeId}&projectId={projectId}");
            Console.WriteLine(result.StatusCode);
            Console.ReadKey();
            return result;
        }
        public async Task<HttpResponseMessage> RemoveProject(int employeeId, int projectId) {
            var result = await base.client.GetAsync($"{path}/removeProject?employeeId={employeeId}&projectId={projectId}");
            Console.WriteLine(result.StatusCode);
            Console.ReadKey();
            return result;
        }
    }
}