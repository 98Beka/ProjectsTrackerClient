using ProjectsTrackerClient;
using System.Configuration;
using ProjectsTrackerClient.Controllers;
using ProjectsTrackerClient.Interfaces;
using ProjectsTracker.Controllers;

string? uriStr = ConfigurationManager.AppSettings["serverURL"];
if (uriStr == null || uriStr == "") {
    Tools.WriteColor("Plaes add serverURL in the App.config", ConsoleColor.Red);
    Console.ReadLine();
    return;
}
var client = new HttpClient();
client.BaseAddress = new Uri(uriStr);
var projectsController = new ProjectsController(client);
var employeesController = new EmployeesController(client);

var projectPage = new PageController(projectsController ,new List<IPageCommand> {
    new GetAllProjectsCommand(),
    new GetProjectByIdCommand(),
    new ProjectAddCommand(),
    new DeleteProjectCommand()
});
var employeePage = new PageController(employeesController, new List<IPageCommand> {
    new GetAllEmployeeCommand(),
    new EmployeeAddCommand(),
    new DeleteEmployeeCommand(),
    new GetEmployeeByIdCommand()
});

//test connection
try {
    Console.WriteLine(uriStr);
    var answer = await client.GetAsync("test");
    Tools.WriteColor(await answer.Content.ReadAsStringAsync(), ConsoleColor.Green);
    Console.ReadKey();

} catch (Exception ex) {
    Tools.WriteColor(ex.Message, ConsoleColor.Red);
    Tools.WriteColor("Please try to change configurations -> App.config", ConsoleColor.Red);
    Console.ReadKey();
    return;
}


//MainMen
while (true) {
    HeaderWriter.WriteHeader(Pages.Menu);
   
    switch (Console.ReadKey().Key) {
        case ConsoleKey.D1:
            await projectPage.OpenPage(Pages.Projects);
            break;        
        
        case ConsoleKey.D2:
            await employeePage.OpenPage(Pages.Employees);
            break;

        case ConsoleKey.Escape:
            Environment.Exit(0);
            break;
    }
}


