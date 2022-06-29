using ProjectsTracker.Controllers;

namespace ProjectsTrackerClient.Interfaces {
    internal interface IPageCommand {
        public ConsoleKey Key { get; set; }
        public Task ExecCommand(ServerConnector connector);
    }
}
