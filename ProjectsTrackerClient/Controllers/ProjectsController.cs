using ProjectsTracker.Controllers;

namespace ProjectsTrackerClient.Controllers {
    internal class ProjectsController : ServerConnector {
        private string path = "Projects";
        public ProjectsController(HttpClient client) : base(client, "Projects") {}
        public async Task<HttpResponseMessage> AddEmployee(int projectId, int employeeId) {
            var result = await base.client.GetAsync($"{path}/addEmployee?projectId={projectId}&employeeId={employeeId}");
            Console.WriteLine(result.StatusCode);
            Console.ReadKey();
            return result;
        }
        public async Task<HttpResponseMessage> RemoveEmployee(int projectId, int employeeId) {
            var result = await base.client.GetAsync($"{path}/removeEmployee?projectId={projectId}&employeeId={employeeId}");
            Console.WriteLine(result.StatusCode);
            Console.ReadKey();
            return result;
        }
        public async Task<HttpResponseMessage> AppointTeamlead(int projectId, int employeeId) {
            var result = await base.client.GetAsync($"{path}/appointTeamlead?projectId={projectId}&employeeId={employeeId}");
            Console.WriteLine(result.StatusCode);
            Console.ReadKey();
            return result;
        }
    }
 
}
