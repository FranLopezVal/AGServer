using System;
namespace AGServer.Http
{
	public enum MethodRoutes
	{
		GET,
		POST
	}

	public struct Route
	{
		public string Name { get; set; }
		public string UrlRgex { get; set; }
		public string Method { get; set; }
		public Func<Request, Response> CallBack { get; set; }

		public static Route Default => new Route()
		{
			Name="__DEFAULT__",
			UrlRgex="__NONE__",
			Method = "GET",
		};

        public static bool operator ==(Route r, Route b)
        {
            return r.Name == b.Name && r.UrlRgex == b.UrlRgex;
        }
        public static bool operator !=(Route r, Route b)
        {
            return r.Name != b.Name || r.UrlRgex != b.UrlRgex;
        }
    }

	//public static class ExtedMethodRoute
	//{
 //       public static string ToStr(this MethodRoute mr)
 //       {
 //           switch (mr)
 //           {
 //               case MethodRoute.GET:
 //                   return "GET";
 //               case MethodRoute.POST:
 //                   return "POST";
 //               default:
 //                   return "GET";
 //           }
 //       }
 //       public static MethodRoute ToMethodRoute(this string mr)
 //       {
 //           switch (mr)
 //           {
 //               case "GET":
 //               return MethodRoute.GET;
 //               case "POST":
 //               return MethodRoute.POST;
 //               default:
 //                   return MethodRoute.GET;
 //           }
 //       }
 //   }


}

