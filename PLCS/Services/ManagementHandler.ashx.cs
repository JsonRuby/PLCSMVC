using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace PLCS.Services
{
    /// <summary>
    /// ManagementHandler 的摘要说明
    /// </summary>
    public class ManagementHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            var userId = HttpContext.Current.Request["userId"];

            var table = SqlHelper.ExecuteDataTable(@"select  name,dept from employees where empno=@empno", 
                new Dictionary<string, object>
            {
                {"empno",userId}
            });
            string userName, userDept;
            if (table.Rows.Count > 0)
            {
                userName = table.Rows[0][0].ToString();
                userDept = table.Rows[0][1].ToString();
            }
            else
            {
                userName = "";
                userDept = "";
            }

            context.Response.Write(userName + "|" + userDept);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}