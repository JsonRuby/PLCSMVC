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
        public static List<ManagementTableModel> GetPagedData(PageInfoModel pageInfo, Dictionary<string, object> conditions = null, bool isLike = false)
        {
            var table = GetPagedDataTable(
                pageInfo.PageIndex,
                pageInfo.PageSize, conditions, isLike);
            return ListHelper.ConvertDataTableToList<ManagementTableModel>(table, ConvertTypeEum.ByIndexNumber, 1);

        }

        public static List<ManagementTableModel> GetNonPagedData(Dictionary<string, object> conditions = null, bool isLike = false)
        {
            var table = GetNonPagedDataTable(
                 conditions, isLike);
            return ListHelper.ConvertDataTableToList<ManagementTableModel>(table, ConvertTypeEum.ByIndexNumber, 0);

        }


        public static int GetNonPagedDataCount(Dictionary<string, object> conditions = null, bool isLike = false)
        {
            var table = GetNonPagedDataTable(
                 conditions, isLike);

            return table.Rows.Count;

        }


        public static DataTable GetPagedDataTable(int pageIndex, int pageSize, Dictionary<string, object> conditions = null, bool isLike = false)
        {
            var sql = string.Format(@" select * from (
                    select row_number() over(order by t.id) as rowId,t.* from (
                    select distinct  a.*  
                    from PLCS_Closet  as a,PLCS_Appconfig as b,PLCS_Appconfig as c
                    where 1=1  
                    and left(a.region,len(b.appgroupvalue))=b.appgroupvalue
                    and b.appgroupkey=@currentUser
                    and b.appgroup='Region'
                    and left(a.orgdept,len(c.appgroupvalue))=c.appgroupvalue
                    and c.appgroupkey=@currentUser
                    and c.appgroup='Dept' {0} ) t ) temp
                    where temp.rowId between (@pageIndex - 1)*@pageSize +1 and @pageIndex*@pageSize",
             SqlHelper.AggregateConditionsToWheresql("a", conditions, isLike));

            if (conditions == null)
            {
                conditions = new Dictionary<string, object>();
            }
            if (conditions.ContainsKey("currentUser"))
            {
                conditions.Remove("currentUser");
            }
            var currentUser = CommonHelper.Cookies("LoginUserName", "", CookiesEnum.Get);
            conditions.Add("currentUser", @currentUser);
            conditions.Add("pageIndex", pageIndex);
            conditions.Add("pageSize", pageSize);
            var table = SqlHelper.ExecuteDataTable(sql, conditions, isLike);
            return table;
        }


        public static DataTable GetNonPagedDataTable(Dictionary<string, object> conditions = null, bool isLike = false)
        {
            if (conditions == null)
            {
                conditions = new Dictionary<string, object>();
            }
            if (conditions.ContainsKey("pageIndex"))
            {
                conditions.Remove("pageIndex");
            }
            if (conditions.ContainsKey("pageSize"))
            {
                conditions.Remove("pageSize");
            }
            if (conditions.ContainsKey("currentUser"))
            {
                conditions.Remove("currentUser");
            }
            var currentUser = CommonHelper.Cookies("LoginUserName", "", CookiesEnum.Get);
            conditions.Add("currentUser", @currentUser);


            var sql = string.Format(@" select distinct a.*  
                                       from PLCS_Closet  as a,PLCS_Appconfig as b,PLCS_Appconfig as c
                                       where 1=1  
                                       and left(a.region,len(b.appgroupvalue))=b.appgroupvalue
                                        and b.appgroupkey=@currentUser
                                        and b.appgroup='Region'
                                        and left(a.orgdept,len(c.appgroupvalue))=c.appgroupvalue
                                        and c.appgroupkey=@currentUser
                                        and c.appgroup='Dept'  {0} ",
            SqlHelper.AggregateConditionsToWheresql("a", conditions, isLike));
            var table = SqlHelper.ExecuteDataTable(sql, conditions, isLike);
            return table;
        }


        public static DataRowCollection GetFilterDataRows(string columnName, Dictionary<string, object> conditions = null)
        {
            if (conditions == null)
            {
                conditions = new Dictionary<string, object>();
            }
            if (conditions.ContainsKey("currentUser"))
            {
                conditions.Remove("currentUser");
            }
            var currentUser = CommonHelper.Cookies("LoginUserName", "", CookiesEnum.Get);
            conditions.Add("currentUser", @currentUser);
            return SqlHelper.ExecuteDataTable(string.Format(@"select distinct a.{0} 
                    from plcs_closet as a,plcs_appconfig as b,plcs_appconfig as c
                    where 
                    left(a.region,len(b.appgroupkey))=b.appgroupkey
                    and b.appgroupvalue=@currentUser
                    and b.appgroup='Region'
                    and left(a.dept,len(c.appgroupkey))=c.appgroupkey
                    and c.appgroupvalue=@currentUser
                    and c.appgroup='Dept' order by a.{0} ", columnName), conditions).Rows;
        }


        public static DataTable GetTableHead(Dictionary<string, object> conditions = null)
        {
            if (conditions == null)
            {
                conditions = new Dictionary<string, object>();
            }
            if (conditions.ContainsKey("currentUser"))
            {
                conditions.Remove("currentUser");
            }
            var currentUser = CommonHelper.Cookies("LoginUserName", "", CookiesEnum.Get);
            conditions.Add("currentUser", @currentUser);
            return SqlHelper.ExecuteDataTable(@"select * from v_plcs_closet_cn where 1=0 ");
        }
    }


}