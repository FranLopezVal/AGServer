using System;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace AGServer.Http
{
	public class HttpHandler
	{
		private List<Route> _routes;

		public HttpHandler()
		{
			_routes = new List<Route>();
		}

		public void AddRoute(Route rt)
		{
			if(!_routes.Contains(rt))
				_routes.Add(rt);
		}

		public void HandleClient(TcpClient client)
		{

			Stream strInput = GetStr(client);
			Stream strOutput = GetStr(client);

            Request req = GetRequest(strInput,strOutput);
			Response res = RouteReq(strInput,strOutput,req);

            Console.WriteLine("{0} {1}", res.CodeStatus, req.Url);
            // build a default response for errors
            if (res.Content == null)
            {
                if (res.CodeStatus != "200")
                {
                    res.SetAsUTF8 = string.Format("{0} {1} <p> {2}", res.CodeStatus, req.Url, res.ReasonReturned);
                }
            }

            WriteResponse(strOutput, res);

            strOutput.Flush();
            strOutput.Close();
            strOutput = null;

            strInput.Close();
            strInput = null;

        }

        private Stream GetStr(TcpClient cl) => cl.GetStream();

		private Response RouteReq(Stream In, Stream Out, Request req)
		{
			List<Route> validRoutes = _routes.Where(o =>
			Regex.Match(req.Url, o.UrlRgex).Success).ToList();

			if (!validRoutes.Any()) return Builder.NotFound();

			Route f_route = Route.Default;
            f_route = validRoutes.SingleOrDefault(o => o.Method == req.Method);

			if (f_route == Route.Default) return Builder.NotMethod();

			var match = Regex.Match(req.Url,f_route.UrlRgex);

			if (match.Groups.Count > 1)
                req.Path = match.Groups[1].Value;
            else
                req.Path = req.Url;
			req.Route = f_route;

			try
			{
				return f_route.CallBack(req);
			}
			catch (Exception ex)
			{
				return Builder.InternalServerError();	
			}
        }

        private static string Readline(Stream stream)
        {
            int n_char;
            string line = "";
            
            while (true)
            {
                n_char = stream.ReadByte();

                if (n_char == '\n') { break; } //End line
                if (n_char == '\r') { continue; } //
                if (n_char == -1) { Thread.Sleep(1); continue; };

                line += Convert.ToChar(n_char);
            }
            return line;
        }

        private static void Write(Stream stream, string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            stream.Write(bytes, 0, bytes.Length);
        }

        //Response like HTTP Format
        private static void WriteResponse(Stream str, Response res)
        {

            if (res.Content == null)
            {
                res.Content = new byte[] { };
            }

            if (!res.Headers.ContainsKey("Content-Type"))
            {
                res.Headers["Content-Type"] = "text/html; charset=utf-8";
            }

            res.Headers["Server"] = "AGServer";
            res.Headers["Content-Length"] = res.Content.Length.ToString();

            Write(str, string.Format("HTTP/1.1 {0} {1}\r\n", res.CodeStatus, res.ReasonReturned));
            Write(str, string.Join("\r\n", res.Headers.Select(
                x => string.Format("{0}: {1}", x.Key, x.Value)
                        )
                    )
                );
            Write(str, "\r\n\r\n");

            str.Write(res.Content, 0, res.Content.Length);
        }

        private Request GetRequest(Stream istr, Stream ostr)
        {
            //Read Request Line
            string request = Readline(istr);

            string[] tokens = request.Split(' ');
            if (tokens.Length != 3)
            {
                throw new Exception("invalid http request line");
            }
            string method = tokens[0].ToUpper();
            string url = tokens[1];
            string protocolVersion = tokens[2];

            //Read Headers
            Dictionary<string, string> headers = new Dictionary<string, string>();
            string line;
            while ((line = Readline(istr)) != null)
            {
                if (line.Equals(""))
                {
                    break;
                }

                int separator = line.IndexOf(':');
                if (separator == -1)
                {
                    throw new Exception("invalid http header line: " + line);
                }
                string name = line.Substring(0, separator);
                int pos = separator + 1;
                while ((pos < line.Length) && (line[pos] == ' '))
                {
                    pos++;
                }

                string value = line.Substring(pos, line.Length - pos);
                headers.Add(name, value);
            }

            string content = null;
            if (headers.ContainsKey("Content-Length"))
            {
                int totalBytes = Convert.ToInt32(headers["Content-Length"]);
                int bytesLeft = totalBytes;
                byte[] bytes = new byte[totalBytes];

                while (bytesLeft > 0)
                {
                    byte[] buffer = new byte[bytesLeft > 1024 ? 1024 : bytesLeft];
                    int n = istr.Read(buffer, 0, buffer.Length);
                    buffer.CopyTo(bytes, totalBytes - bytesLeft);

                    bytesLeft -= n;
                }

                content = Encoding.ASCII.GetString(bytes);
            }


            return new Request()
            {
                Method = method,
                Url = url,
                Headers = headers,
                Content = content
            };
        }


    }
}

