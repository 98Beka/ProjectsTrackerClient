using ProjectsTracker.Controllers;
using ProjectsTracker.Models;
using ProjectsTrackerClient.Controllers;
using ProjectsTrackerClient.Interfaces;

namespace ProjectsTrackerClient {

    //Get Projects list
    internal class GetAllProjectsCommand : IPageCommand {
        public ConsoleKey Key { get; set; } = ConsoleKey.D1;

        public async Task ExecCommand(ServerConnector connector){
            var entitys = await connector.GetList<Project>();
            HeaderWriter.WriteHeader(Pages.Filter);
            string header = "[  id  ]" + Tools.MakeCell("Project name");
            switch (Console.ReadKey(true).Key) {

                case ConsoleKey.D1:
                    entitys = FilterByPriority(entitys);
                    Tools.WriteColor(header + Tools.MakeCell("Priority"), ConsoleColor.Blue);
                    foreach (var e in entitys)
                        Console.WriteLine(Tools.MakeCell(e.Id) + Tools.MakeCell(e.Name) + Tools.MakeCell(e.Priority.ToString()));
                    break;

                case ConsoleKey.D2:
                    entitys = FilterByStartDate(entitys);
                    Tools.WriteColor(header + Tools.MakeCell("Start Date"), ConsoleColor.Blue);
                    foreach (var e in entitys)
                        Console.WriteLine(Tools.MakeCell(e.Id) + Tools.MakeCell(e.Name) + Tools.MakeCell(e.Start.ToShortDateString()));
                    break;                
                
                case ConsoleKey.D3:
                    try {
                        entitys = FilterByStartDateRange(entitys,Tools.GetDate("Date from"), Tools.GetDate("Date until"));
                    } catch { Console.WriteLine("Incorrect format of date"); }
                    Tools.WriteColor(header + Tools.MakeCell("Start Date"), ConsoleColor.Blue);
                    foreach (var e in entitys)
                        Console.WriteLine(Tools.MakeCell(e.Id) + Tools.MakeCell(e.Name) + Tools.MakeCell(e.Start.ToShortDateString()));
                    break;
                default:
                    Tools.WriteColor(header, ConsoleColor.Blue);
                    foreach (var e in entitys)
                        Console.WriteLine(Tools.MakeCell(e.Id) + Tools.MakeCell(e.Name));
                    break;
            }

        }

        private List<Project> FilterByPriority(List<Project> input) {
            List<Project> tmp = new List<Project>();
            foreach(var p in input) 
                tmp.Add(p);
            tmp.Sort((x, y) => x.Priority.Value.CompareTo(y.Priority.Value));

            for (int i = 0; i < tmp.Count; i++) {
                if (tmp[i].Priority == 0) { 
                    var proj = tmp[0];
                    for (int j = 0; j < tmp.Count - 1; j++)
                        tmp[j] = tmp[j + 1];
                    tmp[tmp.Count - 1] = proj;
                }
            }
            return tmp;
        }

        private List<Project> FilterByStartDate(List<Project> input) {

            List<Project> tmp = new List<Project>();
            foreach (var p in input)
                tmp.Add(p);
            tmp.Sort((x, y) => x.Start.CompareTo(y.Start));
            return tmp;
        }

        private List<Project> FilterByStartDateRange(List<Project> input, DateTime from, DateTime until) {

            return FilterByStartDate(input.FindAll(p => (p.Start >= from && p.Start <= until)));
        }


    }


    //Add Project
    internal class ProjectAddCommand : IPageCommand {
        public ConsoleKey Key { get; set; } = ConsoleKey.D3;

        public async Task ExecCommand(ServerConnector connector) { 
            var proj = new Project();
            proj.Start = DateTime.Today;
            new Editor().ChangePropValue(proj, new List<string> { "Employees", "TeamLead", "Id" });

            var result = await connector.Post(proj);
            HeaderWriter.WriteHeader(Pages.Projects);

            if(result.StatusCode == System.Net.HttpStatusCode.BadRequest)
                Console.WriteLine($"Project wasn't added\nStatus: " + result.StatusCode);
            else
                Console.WriteLine($"Project wasn added\nStatus: " + result.StatusCode);
        }
    }

    //Delete Project 
    internal class DeleteProjectCommand : IPageCommand {
        public ConsoleKey Key { get; set; } = ConsoleKey.D4;

        public async Task ExecCommand(ServerConnector connector) {
            var entity = await connector.GetById<Project>(Tools.GetId());
            Tools.ShowPropsValue(entity, new List<string> { "Employees", "TeamLead" });

            Tools.WriteColor("\nDo you wont to delete this project?", ConsoleColor.Red);
            Console.WriteLine("to delete click -Y\n");
            if (Console.ReadKey(true).Key == ConsoleKey.Y) {
                var result = await connector.Delete(entity.Id);
                Console.WriteLine("deletet");
            } else
                Console.WriteLine("wasn't deleted");                
        }
    }

    //GetById
    internal class GetProjectByIdCommand : IPageCommand {
        public ConsoleKey Key { get; set; } = ConsoleKey.D2;

        private List<string> ignorProps = new List<string>{ "Employees", "TeamLead" };

        public async Task ExecCommand(ServerConnector connector) {
            var cmd = new ConsoleKeyInfo();
            var entity = await connector.GetById<Project>(Tools.GetId());
            while (cmd.Key != ConsoleKey.Backspace) {
                if(entity.Id == 0) {
                    Console.WriteLine("NotFound");
                    Console.ReadKey(false);
                    return;
                }

                HeaderWriter.WriteHeader(Pages.Project);
                Tools.ShowPropsValue(entity, ignorProps);

                Tools.WriteColor("Employees:", ConsoleColor.Blue);
                Tools.WriteColor(Tools.MakeCell("Employee Name") + "[  id  ]", ConsoleColor.Blue);
                if (entity.Employees != null)
                    foreach (var e in entity.Employees)
                        Console.WriteLine(Tools.MakeCell(e.Name) + Tools.MakeCell(e.Id));

                Console.WriteLine();
                Tools.WriteColor(Tools.MakeCell("Teamlead name") + "[  id  ]", ConsoleColor.Blue);
                if(entity.TeamLead == null)
                    Console.WriteLine("not found");
                else
                    Console.WriteLine(Tools.MakeCell(entity.TeamLead.Name) + Tools.MakeCell(entity.TeamLead.Id));

                cmd = Console.ReadKey(true);
                switch (cmd.Key) {

                    case ConsoleKey.D1:
                       var editedEntity = new Editor().ChangePropValue(entity, ignorProps);
                       var res = await connector.Put(editedEntity);
                        Console.WriteLine(res.StatusCode);
                        break;

                    case ConsoleKey.D2:
                        await ((ProjectsController)connector).AddEmployee(entity.Id, Tools.GetId("EmployeeId: "));
                        entity = await connector.GetById<Project>(entity.Id);
                        break;
                    case ConsoleKey.D3:
                        int EmployeeId = Tools.GetId("EmployeeId: ");
                        Employee? employee = entity.Employees.FirstOrDefault(e => e.Id == EmployeeId);
                        if (employee == null) {
                            Console.WriteLine($"There isn't an employee with this id in {entity.Name.ToUpper()}");
                            Console.ReadKey(true);
                        } else {
                            await ((ProjectsController)connector).AppointTeamlead(entity.Id, EmployeeId);
                            entity = await connector.GetById<Project>(entity.Id);
                        }
                        break;
                    case ConsoleKey.D4:
                        await ((ProjectsController)connector).RemoveEmployee(entity.Id, Tools.GetId("EmployeeId: "));
                        entity = await connector.GetById<Project>(entity.Id);
                        break;
                }
            }
            HeaderWriter.WriteHeader(Pages.Projects);

        }
    }

}