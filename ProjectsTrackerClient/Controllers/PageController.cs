using ProjectsTracker.Controllers;
using ProjectsTrackerClient.Interfaces;

namespace ProjectsTrackerClient.Controllers {
    internal class PageController {
        private List<IPageCommand> pageCommands;
        private ServerConnector connector;
        public PageController(ServerConnector connector ,List<IPageCommand> pageCommands) {
            this.pageCommands = pageCommands;
            this.connector = connector;
        }

        public async Task OpenPage(Pages pagesEnum) {
            ConsoleKeyInfo cmd = new ConsoleKeyInfo();
            HeaderWriter.WriteHeader(pagesEnum);

            while (cmd.Key != ConsoleKey.Backspace) { 
                cmd = Console.ReadKey(true);
                HeaderWriter.WriteHeader(pagesEnum);

                try {
                    foreach (var pgCommand in pageCommands) {
                        if (pgCommand.Key == cmd.Key)
                            await pgCommand.ExecCommand(connector);
                    }
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            } 
        }

    }
}
