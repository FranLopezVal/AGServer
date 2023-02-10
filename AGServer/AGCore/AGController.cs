using System;
using System.Text;



namespace AGServer.AG
{
    [Obsolete]
	public enum HeaderAGProtocol
	{
		server = (0b1110_1010),
		client = server >> 1,
		unicast = 0b0010,
		multicast = unicast << 2,
		eof_ident = 0b1011_0111,
		eof_confirm = 0b0011_0111
    }

    /// <summary>
    /// Listado de cabezeras que utiliza AG
    /// </summary>
    public struct Headers //NEED SAME HEADERS ON CLIENT COMB
    {
        public const string ERR_HEADER = "ERR_HEADER_0";
        public const string CLIENT_INIT_SESSION = "CL_I_SESSION";
        public const string CLIENT_LOGIN_SERVER = "CL_LOG_SRV";
        public const string CLIENT_REGISTER_SERVER = "CL_REG_SRV";
        public const string GET_VERSION_AG = "VERSION_SERVER_AG";
        public const string MAINTENANCE = "MAINTENANCE_MODE";
        //ONLY SEND BY SERVER
        public const string CONNECTED_OK = "CONNECTED";
    }

    /// <summary>
    /// Clase que controla los datos recividos y enviados en el servidor
    /// </summary>
    public class AGController
	{

        internal static Dictionary<string, Action<object, AGClient>> _responses
        = new Dictionary<string, Action<object, AGClient>>();

        internal static Encoding GlobalEncoding =>
			Encoding.UTF8;

		public static char GLOBAL_DELIMITER = '@';

        [Obsolete]
        public static byte[] AGProtocolSimple(string data)
		{
			byte[] msgData = GlobalEncoding.GetBytes(data);

			byte[] bytes = new byte[4 + msgData.Length + 2];

			bytes[0] = (byte)HeaderAGProtocol.server;
			bytes[1] = (byte)(msgData.Length);

			for (int i = 0; i < msgData.Length; i++)
			{
				bytes[i + 4] = msgData[i];
			}

            bytes[msgData.Length - 2] = 0b1011_0111;
            bytes[msgData.Length - 2] = 0b0011_0111;

			return bytes;
        }

        /// <summary>
        /// Esta clase es llamada cuando un cliente envia datos al servidor,
        /// este iniciara una accion preprogramada
        /// </summary>
        /// <param name="data"></param>
        /// <param name="client"></param>
		public static void ComputeReceivedData(string data,AGClient client)
		{
			string header = GetHeader(data);
            int idx = data.IndexOf(GLOBAL_DELIMITER);
            string dat = data.Substring(idx>=0?idx: 0);
			InitAction(header, dat, client);
		}
        /// <summary>
        /// Recive la cabezera de un mensaje enviado al servidor
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
		public static string GetHeader(string data)
		{
            string[] parts = data.Split(new char[] { GLOBAL_DELIMITER }, StringSplitOptions.RemoveEmptyEntries);

            string header = parts[0];
            header.Replace(@"\u0000", "");
            header.Trim('\0');
            
			if (string.IsNullOrEmpty(header)) return Headers.ERR_HEADER;
			return header;
		}


        /// <summary>
        /// Inicia una accion preconfigurada en el servidor
        /// </summary>
        /// <param name="header"></param>
        /// <param name="dataReceived"></param>
        /// <param name="handler"></param>
        public static void InitAction(string header, string dataReceived, AGClient handler)
        {
            if (_responses != null)
                _responses[header](dataReceived, handler);
        }

        /// <summary>
        /// Añade una accion que sera usada en <see cref="InitAction(string, string, AGClient)"/>
        /// cuando el cliente mande datos al servidor
        /// </summary>
        /// <param name="header"></param>
        /// <param name="function"></param>
        public static void AddDataHandle(string header, Action<object, AGClient> function)
        {
            _responses.Add(header, function);
        }

        /// <summary>
        /// Añade las cabeceras con sus controladores por defecto de AG
        /// </summary>
        internal static void InitAGHeaders()
        {
            AddDataHandle(Headers.GET_VERSION_AG, (data, handler) =>
            { handler.SendData(Headers.GET_VERSION_AG + AGController.GLOBAL_DELIMITER + AGServer.__VERSION__); });

            AddDataHandle(Headers.ERR_HEADER, (data, handler) =>
            {
                handler.SendData(Headers.ERR_HEADER + AGController.GLOBAL_DELIMITER + "NOT INFO");
            });
        }
    
}
}

