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

		public string GetId => _session!=null?_session.Token : "_";

		public AGClient(Socket s,AGSession session)
		{
			_socket = s;
            _clientAddr = ((IPEndPoint)s.RemoteEndPoint).Address;

            _session = session;

            _handlerData = new Thread(HandleData);
            _handlerData.Start();

			onReceiveData += (data)=>{

			};

        }

		public void HandleData()
		{
			int nbytes = _socket.ReceiveBufferSize;
            byte[] buffer = new byte[nbytes];

			int n = _socket.Receive(buffer, nbytes, SocketFlags.None);

			string data = AGController.GlobalEncoding.GetString(buffer, 4, n - 6);

			if (buffer[0] == (byte)HeaderAGProtocol.client)
			{ //msgfromclient

			}

			if (buffer[n-2] == (byte)HeaderAGProtocol.eof_ident &&
				buffer[n-1] == (byte)HeaderAGProtocol.eof_confirm)
			{
				//Is correct format AGP
			}
			//if(onReceiveData!=null)
			Console.WriteLine(data);
			onReceiveData(data);
        }

		public void SendData(string data)
		{
            data = data + (char)0; 
            _socket.Send(AGController.AGProtocolSimple(data));
        }

		public void Close()
		{
			onDisconnect(this);
			_socket.Shutdown(SocketShutdown.Both);
			_socket.Close();
			_handlerData.Abort();
		}
	}
}

