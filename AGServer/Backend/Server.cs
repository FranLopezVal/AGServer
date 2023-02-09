
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace AGServer
{
	public enum ServerState
	{
		StandBy=0,
		Running,
		Stopped,
		Error
	}

	public abstract class Server
	{
		protected IPAddress? _targetIp;
		protected int _port;
		protected ServerState _state;

		public Server(int port, IPAddress? ip)
		{
			if (port <= 3001 || port > 64000) port = 15080;
			_targetIp = ip;
			_state = (ServerState)0;
		}
		public abstract void Run();
		public abstract void Shutdown();
	}
}

