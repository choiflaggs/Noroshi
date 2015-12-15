using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightNode.Server;
using Noroshi.Server.Contexts;

namespace Noroshi.Server.Filters
{
    public class ContextContainerHandler : LightNodeFilterAttribute
    {
        public override async Task Invoke(OperationContext operationContext, Func<Task> next)
        {
            try
            {
                // OnBeforeAction

                ContextContainer.Initialize(new WebContext());

                // TODO : 別 Filter に切り出して認証
                var sessionId = "DEBUG-SESSION";
                var headers = operationContext.Environment["owin.RequestHeaders"] as IDictionary<string, string[]>;
                if (headers.ContainsKey("SessionID"))
                {
                    sessionId = headers["SessionID"].FirstOrDefault();
                }
                //                operationContext.Environment["sessionId"] = "F62C8BB4-C9D1-52D8-AAEE-0896CA033DFA";
                ContextContainer.GetWebContext().LoadPlayer(sessionId);

                await next(); // next filter or operation handler

                // OnAfterAction
            }
            catch(Exception)
            {
                // OnExeception
            }
            finally
            {
                ContextContainer.ClearContext();
            }
        }
    }
}