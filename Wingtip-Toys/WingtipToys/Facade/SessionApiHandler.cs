using System.Web;
using System.Web.SessionState;
using MessagePack;
using MessagePack.Resolvers;

namespace WingtipToys.Facade
{
    public class SessionApiHandler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            if (!string.Equals(context.Request.Path, "/facadesession"))
            {
                return;
            }

            var key = context.Request.QueryString["key"];
            var value = context.Session[key];
            if (value is null)
            {
                context.Response.StatusCode = 404;
                context.Response.Close();
            }

            var bytes = MessagePackSerializer.Serialize(value, ContractlessStandardResolver.Instance);
            context.Response.BinaryWrite(bytes);
            context.Response.StatusCode = 200;
            context.Response.Flush();
            context.Response.Close();
        }

        public bool IsReusable { get; }
    }
}