using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using AGServer.Http;

namespace AGServer
{
	public class HttpServer : Server
	{
        private TcpListener _listener;
        private HttpHandler _handler;
        //private List<Route> _routes;

		public HttpServer(int port,List<Route> routes,IPAddress? ip):base(port,ip)
		{
            _port = port; _targetIp = ip;
            _handler = new HttpHandler();
            routes.ForEach(o => _handler.AddRoute(o));
            //Lambda not necesary in .net6
                //foreach (var rt in routes)
                //{
                //    _handler.AddRoute(rt);
                //}
        }


        public override void Run()
        {
            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();

            _state = ServerState.Running;

            while (_state == ServerState.Running)
            { 
                TcpClient c = _listener.AcceptTcpClient();

                Thread tr = new Thread(() =>
                {
                    _handler.HandleClient(c);
                });

                tr.Start();
                Thread.Sleep(1);
            }

        }

        public override void Shutdown()
        {
            _state = ServerState.Stopped;
        }
    }
}

