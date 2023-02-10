using System;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace AGServer.AG
{
	public class AGClient
	{
		public Action<string> onReceiveData;
		public Action<object> onDisconnect;

		private Socket _socket;
		private IPAddress _clientAddr;
		private Thread _handlerData;

		private AGSession _session;

		public AGSession GetSession => _session;
		public string GetId => _session!=null?_session.Token : "_";

		public AGClient(Socket s,AGSession session)
		{
			_socket = s;
            _clientAddr = ((IPEndPoint)s.RemoteEndPoint).Address;

            _session = session;

            _handlerData = new Thread(HandleData);
            _handlerData.Start();

			onReceiveData += (data)=>{
				if (string.IsNullOrEmpty(data)) return;
				AGController.ComputeReceivedData(data, this);
			};

        }

		public void HandleData()
		{
			int nbytes = _socket.ReceiveBufferSize;
            byte[] buffer = new byte[nbytes];

			int n = _socket.Receive(buffer, nbytes, SocketFlags.None);

			string data = AGController.GlobalEncoding.GetString(buffer,0, n);

			//if(onReceiveData!=null)
			//Console.WriteLine(data);
			onReceiveData(data);
        }

		public void SendData(string data)
		{
            data = data + (char)0;


			Console.WriteLine($"Sending data: {data} // To client: {this.GetSession.Token}" +
				$" //with Ip: {_clientAddr.ToString()}");

            byte[] b = AGController.GlobalEncoding.GetBytes(data);
            _socket.Send(b);
        }

		public void Close()
		{
			if(onDisconnect!=null)onDisconnect(this);
			_socket.Shutdown(SocketShutdown.Both);
			//_handlerData.Abort();
		}
	}
}

