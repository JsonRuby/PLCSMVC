using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace PLCS.Services
{
    public class CommonHelper
    {

        public static int GetTableCount(string tableName)
        {
            return (int)SqlHelper.ExecuteDataTable(string.Format("select count(*) count from {0} ", tableName)).Rows[0]["count"];
        }


        public static DataTable GetPagedDataTable(string tableName, int pageIndex, int pageSize, Dictionary<string, object> conditions = null)
        {
            string whereSql = "";
            if (conditions != null && conditions.Count > 0)
            {
                whereSql = conditions.Aggregate(whereSql,
                    (current, condition) =>
                        current + (" and " + condition.Key + "=@" + condition.Key));
            }
            else
            {
                conditions = new Dictionary<string, object>();
            }
            conditions.Add("pageIndex", pageIndex);
            conditions.Add("pageSize", pageSize);

            var sql = string.Format(@" select * from (
            select row_number() over(order by id) as rowId, *  from {0} where 1=1 {1}  ) temp
            where temp.rowId between (@pageIndex - 1)*@pageSize +1 and @pageIndex*@pageSize",
                    tableName,
                    whereSql);

            var table = SqlHelper.ExecuteDataTable(sql, conditions);
            return table.Rows.Count > 0 ? table : null;
        }


     
    }
}