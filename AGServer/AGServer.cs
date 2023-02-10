using System;
using AGServer.Http;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;

using AGServer.AG;

namespace AGServer
{
    /// <summary>
    /// Servidor customizable para cualquier uso, administra multiples clientes
    /// </summary>
    public class AGServer : Server
    {
        public const string __VERSION__ = "v0.1_debug_king";


        private IPEndPoint _endIP;
        private Socket _socket;

        private bool _active = false;
        private bool _initialized = false;
        private int _maintenance = -1;

        private List<AGClient> _clients;

        public Action<Socket> onNewConnection = null;

        public AGServer(int port, IPAddress? ip) : base(port, ip)
        {
            if (ip == null)
            {
                throw new ArgumentNullException("ip_addr");
            }
            _clients = new List<AGClient>();

            _endIP = new IPEndPoint(ip, port);
            _socket = new Socket(_endIP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _port = port;

        }

        ~AGServer()
        {
            //Shutdown(); error internal with C# v 11
        }

        public EndPoint? GetEndPoint => _active ? _socket.LocalEndPoint : _endIP;

        /// <summary>
        /// Inicia preconfigurando el servidor
        /// </summary>
        /// <param name="MaxConnections"></param>
        /// <returns></returns>
        public bool Initialize(int MaxConnections)
        {
            if (_initialized) return true;
            if(MaxConnections > (0x7fffffff) || MaxConnections < 0)
            {
                return false;
            }
            _socket.Bind(_endIP);
            try
            {
                _socket.Listen(MaxConnections);
            }
            catch (SocketException)
            {
                Shutdown();
                return false;
            }
            return true;
        }

        public bool PendingBytes()
        {
            if (_initialized)
                return _socket.Poll(1, SelectMode.SelectRead);
            return false;
        }

        /// <summary>
        /// Bucle principal de el servidor
        /// </summary>
        public override void Run()
        {
            _initialized = Initialize(0x7fffffff);
            AGController.InitAGHeaders();

            if (!_initialized) return;
            _active = true;

            while (_active)
            {
                HandleClient();
            }
        }

        /// <summary>
        /// Controla la conexion de un cliente
        /// </summary>
        public void HandleClient()
        {
            
            var client = _socket.Accept();

            if (_maintenance != -1)
            {
                var agc = new AGClient(client, new AGSession());
                agc.SendData(Headers.MAINTENANCE +
                    AGController.GLOBAL_DELIMITER + "TIMEOUT:" + _maintenance);
                agc.Close();
            }
            else
            {
                if (onNewConnection != null)
                    onNewConnection(client);

                _clients.Add(new AGClient(client, new AGSession()));
                _clients[_clients.Count - 1].SendData(Headers.CONNECTED_OK +
                    AGController.GLOBAL_DELIMITER + "uID:" + _clients[_clients.Count - 1].GetId);
            }
        }   


        /// <summary>
        /// Envia un mensaje multicast.
        /// </summary>
        /// <param name="header">cabecera de datos</param>
        /// <param name="data">datos a enviar</param>
        /// <param name="TaskCount">Task en los que se va a dividir el proceso</param>
        public void SendMulticast(string header,string data,int TaskCount = 2)
        {
            int parts = _clients.Count / TaskCount;
            for (int i = 0; i < parts; i++)
            {
                Task.Factory.StartNew(()=>
                {
                    if (i == parts - 1)
                    { //Cuando hacemos la division en partes, se puede perder datos al dividir y pasar a int
                        //asi nos aseguramos que los datos perdidos se añaden en la ultima parte
                        while (parts * TaskCount < _clients.Count) parts++;
                    }
                    for (int x = i * parts; x < (i+1)*parts; x++)
                    {
                        AGController.InitAction(header, data, _clients[x]);
                    }
                });
            }

        }


        /// <summary>
        /// Pone en modo mantenimiento el servidor,
        /// Rechazara las conexiones enviando primero una respuesta al cliente
        /// </summary>
        /// <param name="timeout"></param>
        public void SetMaintenance(int timeout)
        {
            _maintenance = timeout;
        }

        public override void Shutdown()
        {
            _active = false;
            _socket.Disconnect(true);
            _socket.Dispose();
            _socket = null;
        }

        public override string ToString()
        {
            return $"AG server>{_targetIp}:{_port}";
        }
    }
}

