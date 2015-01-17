using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace PLCS.Models
{
    public class EmpInfoModel
    {
        public EmpInfoModel(string UserId)
        {
            this.UserId = UserId;
            DataTable table = SqlHelper.ExecuteDataTable(@"
                select name,dept from employees where empno=@empno",
                new Dictionary<string, object>
                {
                    {"empno",UserId}
                });
            if (table.Rows.Count > 0)
            {
                this.UserName = table.Rows[0]["name"].ToString();
                this.UserDept = table.Rows[0]["dept"].ToString();
            }
            else
            {
                this.UserName = "";
                this.UserDept = "";
            }
        }
        public string UserId { get; set; }
        public string UserName{get;set;}
        public string UserDept { get; set; }
    }
}