using System;

namespace AGServer.Http
{
	public class Request
	{
		public string Method;
		public string Url;
        public string Path { get; internal set; }
        public Route Route { get; internal set; }
        public Dictionary<string, string> Headers { get; internal set; }
        public string? Content { get; internal set; }

        public Request()
        {
            Headers = new Dictionary<string, string>();
        }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(Content))
                if (!Headers.ContainsKey("Content-Length"))
                    Headers.Add("Content-Length", Content.Length.ToString());

            return string.Format("{0} {1} HTTP/1.0\r\n{2}\r\n\r\n{3}", Method, Url, string.Join("\r\n",
                Headers.Select(x => string.Format("{0}: {1}", x.Key, x.Value))), Content);
        }
    }
}

