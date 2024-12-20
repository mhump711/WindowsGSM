using WindowsGSM.Functions;
using WindowsGSM.GameServer.Engine;
using WindowsGSM.GameServer.Query;
using Newtonsoft.Json.Linq;

namespace WindowsGSM.Plugins
{
    public class Satisfactory : SteamCMDAgent
    {
        public Plugin Plugin = new Plugin
        {
            name = "WindowsGSM.Satisfactory",
            author = "mhump711",
            description = "🧩 WindowsGSM plugin for supporting Satisfactory Dedicated Server",
            version = "1.0",
            url = "https://github.com/mhump711/WindowsGSM/tree/master/WindowsGSM-Plugin-Development",
            color = "#0000ff"
        };


        public Satisfactory(ServerConfig serverData) : base(serverData) => base.serverData = _serverData = serverData;
        private readonly ServerConfig _serverData;

        public override bool loginAnonymous => true; // true if allows login anonymous on steamcmd, else false
        public override string AppId => "1690800"; // Value of app_update <AppId> 

        public override string StartPath => "Engine\Binaries\Win64\FactoryServer-Win64-Shipping.exe"; // Game server start path
        public string FullName = "Satisfactory Dedicated Server"; // Game server FullName
        public bool AllowsEmbedConsole = true;  // Does this server support output redirect?
        public int PortIncrements = 1; // This tells WindowsGSM how many ports should skip after installation
        public object QueryMethod = null; // Query method. Accepted value: null or new A2S() or new FIVEM() or new UT3()

        public string Port = "7777"; // Default port
        public string Defaultmap = "empty"; // Default map name
        public string Maxplayers = "4"; // Default maxplayers
        public string Additional = "-log -unattended"; // Additional server start parameter ***Note do NOT Remove -unattended

        public async void CreateServerCFG() { } // Creates a default cfg for the game server after installation

        public async Task<Process> Start() // Start server function, return its Process
        { 
            var param = new StringBuilder();
            param.Append(string.IsNullOrWhiteSpace(_serverData.ServerPort) ? string.Empty : $" -port={_serverData.ServerPort}");
            param.Append(string.IsNullOrWhiteSpace(_serverData.ServerName) ? string.Empty : $" -name=\"{_serverData.ServerName}\"");
            param.Append(string.IsNullOrWhiteSpace(_serverData.ServerParam) ? string.Empty : $" {_serverData.ServerParam}");            
            
            var p = new Process
            {
                StartInfo = 
                {
                    WindowsStyle = ProcessWindowStyle.Normal,
                    UseShellExecute = false,
                    WorkingDirectory = ServerPath.GetServersServerFiles(_serverData.ServerID),
                    FileName = ServerPath.GetServersServerFiles(_serverData.ServerID, StartPath),
                    Argument = param.ToString()
                },
                EnableRisingEvents = true
            };

            try
            {
                p.Start();
                return p;
            }
            catch (Exception e)
            {
                base.Error = e.Message;
                return null; // return null if fail to start
            }

            
        } 
        public async Task Stop(Process p) // Stop server function
        {
            await Task.Run(() =>
            {
                Functions.ServerConsole.SetMainWindow(p.MainWindowHandle);
                Functions.ServerConsole.SendWaitToMainWindow("^c");
                p.WaitForExit(20000);
            });
        } 
    }
}