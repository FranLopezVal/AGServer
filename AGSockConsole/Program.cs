// See https://aka.ms/new-console-template for more information
using AGServer;
using AGServer.Http;
using System.Reflection;
using Stc;


Csl("AG Server controller initialized.", 4);

bool exit = false;
while (!exit) //Estructura simple para la ejecucion de AG
{
    string? c = Console.ReadLine();
    if (GetParams(c)) exit = !exit;
}

/*
 * Shutdown servers here!!
 */


bool GetParams(string line)
{
    
    string[] subStr = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

    if (subStr.Length <= 0) return false;

    short pc = (short)subStr.Length;

    switch (subStr[0])
    {
        case "exit":
            return true;
            break;
        case "init":
            {
                string f_so = "ags";
                int port = 5008;
                bool b = false;
                bool testDebug = false;
                if (pc >= 2)
                {
                    f_so = subStr[1];
                }
                if (pc >= 3)
                {
                    if (subStr[2] == "b") b = true;
                    else if (subStr[2] == "d") testDebug = true;
                    else if (char.IsNumber(subStr[2][0]))
                    {
                        bool IsOkNumber = true;
                        foreach (var c in subStr[2])
                        {
                            if (!char.IsNumber(c)) IsOkNumber = false;
                        }
                        if (IsOkNumber)
                        {
                            port = Convert.ToInt32(subStr[2]);
                        }
                        else
                        {
                            Csl($"Port: {subStr[2]} not is a valid type", 2);
                            return false;
                        }
                    }
                }
                if (pc >= 4)
                {
                    if (subStr[3] == "b") b = true;
                    else if (subStr[3] == "d") testDebug = true;
                }
                if (pc >= 5)
                {
                    if (subStr[4] == "b") b = true;
                    else if (subStr[4] == "d") testDebug = true;
                }
                if (f_so == "ags")
                {
                    InitAGServer(port);
                }
                else if (f_so == "http")
                {
                    InitHttpServer(port);
                }
                else
                {
                    Csl($"Not know a {f_so} type Server.", 2);
                    return false;
                }
                Csl($"Staring server {f_so} on Port: {port} " +
                    $"\n base: {b} Debug mode: {testDebug}", 1);
            }
            break;
        case "set":
            {
                int m_mode = 0;
                int index = 0;
                if(pc>=2) //index srv, if not index here, index = 0
                {
                    if (char.IsNumber(subStr[1][0]))
                    {
                        index = Convert.ToInt32(subStr[1]);
                    }
                    if (subStr[1]=="m")
                    {
                        m_mode = 1;
                    }
                    if (subStr[1] == "-m")
                    {
                        m_mode = 2;
                    }
                }
                if(pc>=3)
                {
                    if (subStr[2] == "m")
                    {
                        m_mode = 1;
                    }
                    if (subStr[2] == "-m")
                    {
                        m_mode = 2;
                    }
                }
                if(statics.servers==null || statics.servers.Count == 0)
                {
                    Csl("Not servers running", 2);
                    return false;
                }
                if(index >=0 && index < statics.servers.Count)
                {
                    if (m_mode == 1) (statics.servers[index] as AGServer.AGServer).SetMaintenance(1000);
                    if (m_mode == 2) (statics.servers[index] as AGServer.AGServer).SetMaintenance(-1);
                    Csl($"Change maintenance mode on server: {statics.servers[index].ToString()}",4);
                }
                else
                {
                    Csl("Index out of range", 3);
                    return false;
                }
            }
            break;
        default:
            break;
    }
    return false;

}


void InitAGServer(int port = 5008)
{
    statics.servers.Add(new AGServer.AGServer(port, System.Net.IPAddress.Any));
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


void Csl (string txt,int level)
{
    switch (level)
    {
        case 0:
            Console.ResetColor(); break;
        case 1:
            Console.ForegroundColor = ConsoleColor.DarkBlue; break;
        case 2:
            Console.ForegroundColor = ConsoleColor.DarkYellow; break;
        case 3:
            Console.ForegroundColor = ConsoleColor.Red; break;
        case 4:
            Console.ForegroundColor = ConsoleColor.Green; break;
        default:
            break;
    }
    Console.WriteLine(txt);
    Console.ResetColor();
    Console.Write(">>");
}


namespace Stc
{
    public class statics
    {//Just static, 4 never destroy instances
        public static List<Server> servers = new List<Server>();
        public static List<Thread> thrs = new List<Thread>();




        void tst()
        {
            string c ="";
            switch (c)
            {
                case "agp.m": //pone el sistema en modo mantenimiento.
                    (statics.servers[0] as AGServer.AGServer).SetMaintenance(1);
                    break;
                case "agp.nm": //quita el modo mantenimiento de el sistema
                    (statics.servers[0] as AGServer.AGServer).SetMaintenance(-1);
                    break;
                case "init.agp": //inicializa un sistema servidor AG
                    //InitAGP();
                    break;
                case "init.http": //inicializa un servidor HTTP Simple
                    Console.Write("Set Port: ");
                    string? prt = Console.ReadLine();
                    bool correct = true;
                    if (prt == null || string.IsNullOrEmpty(prt)) break;
                    foreach (var chr in prt)
                    {
                        if (!char.IsNumber(chr)) correct = false;
                    }
                    if (!correct) break;
                    //InitHttpServer(Convert.ToInt16(prt));
                    break;
                case "exit": //Cierra el programa
                    {
                        foreach (var item in statics.servers)
                        {
                            item.Shutdown(); //ToDo: Error on exit when close
                        }
                        foreach (var item in statics.thrs)
                        {
                            Semaphore s = new Semaphore(0, 1);

                            item.Suspend();
                        }
                        //   exit = !exit;
                    }
                    break;
                default:
                    break;
            }
        }

    }
}
