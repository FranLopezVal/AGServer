using System;
namespace AGServer
{
	/// <summary>
	/// Guarda datos sobre la sesion de un cliente, ID, TimeOut, y Fecha conexion
	/// </summary>
	public class AGSession
	{
		Guid _id;
		string _token;
		DateTime _sessionCreationTime;
		int _timeout;

		public AGSession()
		{
			_id = Guid.NewGuid();
			
			_token = _id.ToString();
			_sessionCreationTime = DateTime.Now;
			_timeout = 5 * 60; //in secs
		}

		public string Token => _token;
		public int GetTimeOut => _timeout;


        public static bool operator ==(AGSession a, AGSession b)
        { return a._id.CompareTo(b) == 0; }
        public static bool operator !=(AGSession a, AGSession b)
        { return a._id.CompareTo(b) == 0; }
    }
}

