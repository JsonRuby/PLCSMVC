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


            var rows = SqlHelper.ExecuteDataTable(@"exec GetSideBarListByUserId  @userid ",
                new Dictionary<string, object>
                {
                    {"userid",userid}
                }).Rows;
            var list = (from DataRow row in rows
                        select new SideBarModel
                        {
                            SideBarGroupName = row.ItemArray[0].ToString(),
                            SideBarGroupIcon = row.ItemArray[1].ToString(),
                            SideBarItemName = row.ItemArray[2].ToString(),
                            SideBarItemIcon = row.ItemArray[3].ToString(),
                            Controller = row.ItemArray[4].ToString(),
                            Action = row.ItemArray[5].ToString(),
                            Parameters = row.ItemArray[6].ToString()
                        }).ToList();
            return list;
        }
    }
}