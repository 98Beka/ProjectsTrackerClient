using ProjectsTracker.Controllers;
using ProjectsTracker.Models;
using ProjectsTrackerClient.Controllers;
using ProjectsTrackerClient.Interfaces;

namespace ProjectsTrackerClient {
    //Get Employees list
    internal class GetAllEmployeeCommand : IPageCommand {
        public ConsoleKey Key { get; set; } = ConsoleKey.D1;

        public async Task ExecCommand(ServerConnector connector) {
            var entitys = await connector.GetList<Employee>();
            foreach (var e in entitys)
                Console.WriteLine(Tools.MakeCell(e.Name) + Tools.MakeCell(e.Id));
        }
    }


    //Add Employee
    internal class EmployeeAddCommand : IPageCommand {
        public ConsoleKey Key { get; set; } = ConsoleKey.D3;

        public async Task ExecCommand(ServerConnector connector) {
            var emp = new Employee();
            new Editor().ChangePropValue(emp, new List<string> { "Projects", "ProjectsAsLead", "Id" });

            var result = await connector.Post(emp);

            HeaderWriter.WriteHeader(Pages.Employees);
            if (result.StatusCode == System.Net.HttpStatusCode.BadRequest)
                Console.WriteLine($"Project wasn't added\nStatus: " + result.StatusCode);
            else
                Console.WriteLine($"Project wasn added\nStatus: " + result.StatusCode);
        }
    }

    //Delete Employee 
    internal class DeleteEmployeeCommand : IPageCommand {
        public ConsoleKey Key { get; set; } = ConsoleKey.D4;

        public async Task ExecCommand(ServerConnector connector) {
            var entity = await connector.GetById<Employee>(Tools.GetId());
            Tools.ShowPropsValue(entity, new List<string> { "Projects", "ProjectsAsLead" });

            Tools.WriteColor("\nDo you wont to delete this employee?", ConsoleColor.Red);
            Console.WriteLine("to delete click -Y\n");
            if (Console.ReadKey(true).Key == ConsoleKey.Y) {
                var result = await connector.Delete(entity.Id);
                Console.WriteLine("deletet");
            } else
                Console.WriteLine("wasn't deleted");
        }
    }

    //GetById
    internal class GetEmployeeByIdCommand : IPageCommand {
        public ConsoleKey Key { get; set; } = ConsoleKey.D2;

        private List<string> ignorProps = new List<string> { "Projects", "ProjectsAsLead" };

        public async Task ExecCommand(ServerConnector connector) {
            var cmd = new ConsoleKeyInfo();
            var entity = await connector.GetById<Employee>(Tools.GetId());
            while (cmd.Key != ConsoleKey.Backspace) {
                if (entity.Id == 0) {
                    Console.WriteLine("NotFound");
                    Console.ReadKey(false);
                    return;
                }

                HeaderWriter.WriteHeader(Pages.Employee);
                Tools.ShowPropsValue(entity, ignorProps);

                Tools.WriteColor("Projects:", ConsoleColor.Blue);
                Tools.WriteColor(Tools.MakeCell("Project Name") + "[  id  ]", ConsoleColor.Blue);
                if (entity.Projects != null)
                    foreach (var p in entity.Projects)
                        Console.WriteLine(Tools.MakeCell(p.Name) + Tools.MakeCell(p.Id));

                cmd = Console.ReadKey(true);
                switch (cmd.Key) {
                    case ConsoleKey.D1:
                        var editedEntity = new Editor().ChangePropValue(entity, ignorProps);
                        var res = await connector.Put(editedEntity);
                        Console.WriteLine(res.StatusCode);
                        break;
                    case ConsoleKey.D2:
                        await ((EmployeesController)connector).AddProject(entity.Id, Tools.GetId("ProjectId: "));
                        entity = await connector.GetById<Employee>(entity.Id);
                        break;

                    case ConsoleKey.D3:
                        await ((EmployeesController)connector).RemoveProject(entity.Id, Tools.GetId("ProjectId: "));
                        entity = await connector.GetById<Employee>(entity.Id);
                        break;
                }
            }
            HeaderWriter.WriteHeader(Pages.Employees);
        }
    }
}
