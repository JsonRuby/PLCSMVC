using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using PLCS.Models;
using System.Data.SqlClient;

namespace PLCS.Services
{
    public class ManagementService
    {
        public static List<ClosetModel> GetData(PageInfoModel pageInfo)
        {
            var table = CommonHelper.GetPagedDataTable(
                "PLCS_Closet",
                pageInfo.PageIndex,
                pageInfo.PageSize);

            var list2 = ListHelper.ConvertDataTableToList<ClosetModel>(table);

             

            return list2;


        }
    }
    
    
}