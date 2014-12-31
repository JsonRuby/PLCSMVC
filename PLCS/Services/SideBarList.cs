using PLCS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace PLCS.Services
{
    public static class SideBarList
    {
        public static List<SideBarModel> GetSideBarList(string userid)
        {
            #region 測試使用D1105Z320
            userid = "D1105Z320";
            #endregion


            var table = SqlHelper.ExecuteDataTable(@"exec GetSideBarListByUserId  @userid ",
                new Dictionary<string, object>
                {
                    {"userid",userid}
                });
            return ListHelper.ConvertDataTableToList<SideBarModel>(table);
        }
    }
}