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
        public static int GetTableCount(string tableName, Dictionary<string, object> conditions = null)
        {
            return (int)SqlHelper.ExecuteDataTable(string.Format("select count(*) count from {0} where 1=1 {1} ", tableName, SqlHelper.AggregateConditionsToWheresql(conditions))).Rows[0]["count"];
        }


        public static DataTable GetPagedDataTable(string tableName, int pageIndex, int pageSize, Dictionary<string, object> conditions = null)
        {
           

            var sql = string.Format(@" select * from (
            select row_number() over(order by id) as rowId, *  from {0} where 1=1 {1}  ) temp
            where temp.rowId between (@pageIndex - 1)*@pageSize +1 and @pageIndex*@pageSize",
                    tableName,
                    SqlHelper.AggregateConditionsToWheresql(conditions));

            if (conditions == null)
            {
                conditions = new Dictionary<string, object>();
            }
            conditions.Add("pageIndex", pageIndex);
            conditions.Add("pageSize", pageSize);
            var table = SqlHelper.ExecuteDataTable(sql, conditions);
            return table.Rows.Count > 0 ? table : null;
        }
    }



}