using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace PLCS
{
    class SqlHelper
    {
        private static string connStr = ConfigurationManager.ConnectionStrings["dbConnStr"].ConnectionString;

        //封装方法的原则：把不变的放到方法里，把变化的放到参数中

        ////public static int ExecuteNonQuery(string sql)
        ////{
        ////    using (SqlConnection conn = new SqlConnection(connStr))
        ////    {
        ////        conn.Open();
        ////        using (SqlCommand cmd = conn.CreateCommand())
        ////        {
        ////            cmd.CommandText = sql;
        ////            return cmd.ExecuteNonQuery();
        ////        }
        ////    }
        ////}

        ////public static object ExecuteScalar(string sql)
        ////{
        ////    using (SqlConnection conn = new SqlConnection(connStr))
        ////    {
        ////        conn.Open();
        ////        using (SqlCommand cmd = conn.CreateCommand())
        ////        {
        ////            cmd.CommandText = sql;
        ////            return cmd.ExecuteScalar();
        ////        }
        ////    }
        ////}

        //////只用来执行查询结果比较少的sql
        ////public static DataTable ExecuteDataTable(string sql)
        ////{
        ////    using (SqlConnection conn = new SqlConnection(connStr))
        ////    {
        ////        conn.Open();
        ////        using (SqlCommand cmd = conn.CreateCommand())
        ////        {
        ////            cmd.CommandText = sql;
        ////            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
        ////            DataSet dataset = new DataSet();
        ////            adapter.Fill(dataset);
        ////            return dataset.Tables[0];
        ////        }
        ////    }
        ////}

        //第二版

        //public static int ExecuteNonQuery(string sql,SqlParameter[] parameters)
        //{
        //    using (SqlConnection conn = new SqlConnection(connStr))
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = sql;
        //            //foreach (SqlParameter param in parameters)
        //            //{
        //            //    cmd.Parameters.Add(param);
        //            //}
        //            cmd.Parameters.AddRange(parameters);
        //            return cmd.ExecuteNonQuery();
        //        }
        //    }
        //}

        //public static object ExecuteScalar(string sql,SqlParameter[] parameters)
        //{
        //    using (SqlConnection conn = new SqlConnection(connStr))
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = sql;
        //            cmd.Parameters.AddRange(parameters);
        //            return cmd.ExecuteScalar();
        //        }
        //    }
        //}

        ////只用来执行查询结果比较少的sql
        //public static DataTable ExecuteDataTable(string sql, SqlParameter[] parameters)
        //{
        //    using (SqlConnection conn = new SqlConnection(connStr))
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = sql;
        //            cmd.Parameters.AddRange(parameters);

        //            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
        //            DataSet dataset = new DataSet();
        //            adapter.Fill(dataset);
        //            return dataset.Tables[0];
        //        }
        //    }
        //}

        //第三版：使用长度可变参数来简化
        public static int ExecuteNonQuery(string sql, Dictionary<string, object> conditions=null)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    if (conditions != null && conditions.Count > 0)
                    {
                        foreach (var condition in conditions)
                        {
                            cmd.Parameters.AddWithValue("@" + condition.Key, condition.Value);
                        }
                    }
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        //只用来执行查询结果比较少的sql
        public static DataTable ExecuteDataTable(string sql, Dictionary<string, object> conditions=null)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    // cmd.Parameters.AddRange(parameters);
                    if (conditions != null && conditions.Count > 0)
                    {
                        foreach (var condition in conditions)
                        {
                            cmd.Parameters.AddWithValue("@" + condition.Key, condition.Value);
                        }
                    }
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataSet dataset = new DataSet();
                    adapter.Fill(dataset);
                    return dataset.Tables[0];
                }
            }
        }

        public static string AggregateConditionsToWheresql(Dictionary<string, object> conditions = null)
        {
            string whereSql = "";
            if (conditions != null && conditions.Count > 0)
            {
                whereSql = conditions.Aggregate(whereSql,
                    (current, condition) =>
                        current + (" and " + condition.Key + "=@" + condition.Key));
            }
            return whereSql;
        }
    }
}
