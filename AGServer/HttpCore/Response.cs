using System;
using System.Text;

namespace AGServer.Http
{
    public enum StatusCodes
    {
        Continue = 100,
        Ok = 200,
        Created = 201,
        Accepted = 202,
        MovedPermanently = 301,
        Found = 302,
        NotModified = 304,
        BadRequest = 400,
        Forbidden = 403,
        NotFound = 404,
        MethodNotAllowed = 405,
        InternalServerError = 500
    }

	public class Response
	{
        public string CodeStatus { get; set; }
        public string ReasonReturned { get; set; }
        public byte[] Content { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public string SetAsUTF8 { set => SetContent(value, Encoding.UTF8); }
        public string SetAsUTF32 { set => SetContent(value, Encoding.UTF32); }
        public string SetAsASCII { set => SetContent(value, Encoding.ASCII); }
        public string SetAsUnicode { set => SetContent(value, Encoding.Unicode); }

        public void SetContent(string content, Encoding? encoding)
        {
            encoding = encoding == null ? Encoding.UTF8 : encoding;
            Content = encoding.GetBytes(content);
        }

        public Response()
		{
            CodeStatus = "";
            ReasonReturned = "";
            Headers = new Dictionary<string, string>();
		}

        public override string ToString()
        {
            return string.Format("Response status: {0} :: {1}", CodeStatus, ReasonReturned);
        }
    }
}

