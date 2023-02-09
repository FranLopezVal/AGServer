using System;
namespace AGServer.Http
{
	public static class Builder
	{
        public static Response InternalServerError()
        {
            string content = "<h1>500 Server Error/h1>\n\n" +
                "<span>Internal error, don´t know the error, but is a error :/</span>";

            return new Response()
            {
                ReasonReturned = "InternalServerError",
                CodeStatus = "500",
                SetAsUTF8 = content
            };
        }

        public static Response NotFound()
        {
            string content = "<h1>404 Not Found</h1>\n\n" +
                "<small>AGServer not found this page :(</small>";

            return new Response()
            {
                ReasonReturned = "NotFound",
                CodeStatus = "404",
                SetAsUTF8 = content
            };
        }

        public static Response NotMethod()
        {
            string content = "<h1>405 Bad Method</h1>\n\n" +
                "<small>Im not sure... this is the correct method?</small>";

            return new Response()
            {
                ReasonReturned = "Bad method, not allowed",
                CodeStatus = "405",
                SetAsUTF8 = content
            };
        }
    }
}

