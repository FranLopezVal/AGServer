// See https://aka.ms/new-console-template for more information
using AGServer;
using AGServer.Http;
using System.Reflection;
using Stc;


bool exit = false;
while (!exit)
{
    Console.Write(">> ");
    string? c = Console.ReadLine();
    switch (c)
    {
        case "init.agp":
            InitAGP();
            break;
        case "init.http":
            Console.Write("Set Port: ");
            string? prt = Console.ReadLine();
            bool correct = true;
            if (prt == null || string.IsNullOrEmpty(prt)) break;
            foreach (var chr in prt)
            {
                if (!char.IsNumber(chr)) correct = false;
            }
            if (!correct) break;
            InitHttpServer(Convert.ToInt16(prt));
            break;
        case "exit":
            {
                foreach (var item in statics.servers)
                {
                    item.Shutdown();
                }
                foreach (var item in statics.thrs)
                {
                    Semaphore s = new Semaphore(0, 1);
                    
                    item.Suspend();
                }
                exit = !exit;
            }break;
        default:
            break;
    }

}
void InitAGP()
{
    statics.servers.Add(new AGServer.AGServer(5008, System.Net.IPAddress.Any));
    statics.thrs.Add(
    new Thread(() =>
    {
        statics.servers[statics.servers.Count - 1].Run();
    }));
    statics.thrs[statics.thrs.Count - 1].Start();
    Console.WriteLine("AGP Running on port: " + 5008);
}

void InitHttpServer(int port)
{

    var route_config = new List<Route>() {
                new Route {
                    Name = "AGServer test",
                    UrlRgex = @"^/$",
                    Method = "GET",
                    CallBack = (Request request) => {
                        return new Response()
                        {
                            SetAsUTF8 = "Hola! Fran te saluda, con AGServer!",
                            ReasonReturned = "OK",
                            CodeStatus = "200"
                        };
                     }
                }
                
                //new Route {       
                //    Name = "FileSystem Static Handler",
                //    UrlRegex = @"^/Static/(.*)$",
                //    Method = "GET",
                //    Callable = new FileSystemRouteHandler() { BasePath = @"C:\Tmp", ShowDirectories=true }.Handle,
                //},
            };
    statics.servers.Add( new HttpServer(port, route_config, null));
    statics.thrs.Add(
    new Thread(() =>
    {
        statics.servers[statics.servers.Count - 1].Run();
    }));
    statics.thrs[statics.thrs.Count - 1].Start();
    Console.WriteLine("Running on port: " + port);
}

namespace Stc { 
public class statics
{//Just static, 4 never destroy instances
    public static List<Server> servers = new List<Server>();
    public static List<Thread> thrs = new List<Thread>();
}
}