using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using NPOI.HSSF.UserModel;
using PLCS.Models;
using System.Security.Cryptography;
using System.Text;

namespace PLCS.Services
{
    //-----------------------------------------------------------------------------------------------------
    public class CommonHelper
    {
        public static void RemoveCookies(string cookiename)
        {
            var httpCookie = HttpContext.Current.Response.Cookies[cookiename];
            if (httpCookie != null)
                httpCookie.Values.Clear();
        }

        public static string Cookies(string key, string value, CookiesEnum type)
        {
            var reqCookie = HttpContext.Current.Request.Cookies["plcs"];
            if (reqCookie != null)
            {
                switch (type)
                {
                    case CookiesEnum.Set:
                        if (reqCookie.Values.AllKeys.Contains(key))
                        {
                            reqCookie.Values.Remove(key);
                        }
                        reqCookie.Values.Add(key, value);
                        HttpContext.Current.Response.Cookies.Add(reqCookie);
                        return null;
                    case CookiesEnum.Get:
                        return reqCookie.Values.Get(key);
                    default:
                        return null;
                }
            }
            else
            {
                var cookie = new HttpCookie("plcs");
                cookie.Expires = DateTime.Now + new TimeSpan(365, 0, 0, 1);

                switch (type)
                {
                    case CookiesEnum.Set:
                        cookie.Values.Add(key, value);
                        HttpContext.Current.Response.Cookies.Add(cookie);
                        return null;
                    case CookiesEnum.Get:
                        return cookie.Values.Get(key);
                    default:
                        return null;
                }
            }

        }


        //-----------------------------------------------------------------------------------------------------
        public static int GetTableCount(string tableName, Dictionary<string, object> conditions = null)
        {
            DataTable table =
            SqlHelper.ExecuteDataTable(string.Format("select count(*) count from {0} where 1=1 {1} ", tableName,
            SqlHelper.AggregateConditionsToWheresql("", conditions)));
            return table != null ? (int)table.Rows[0]["count"] : 0;
        }

        //-----------------------------------------------------------------------------------------------------
        //        public static DataTable GetPagedDataTable(string tableName, int pageIndex, int pageSize, Dictionary<string, object> conditions = null, bool isLike = false)
        //        {
        //            var currentUser = Cookies("LoginUserName", "", CookiesEnum.Get);
        //            var sql = string.Format(@" select * from (
        //                    select row_number() over(order by id) as rowId, *  from {0} where 1=1 {1}  ) temp
        //                    where temp.rowId between (@pageIndex - 1)*@pageSize +1 and @pageIndex*@pageSize",
        //            tableName,
        //            SqlHelper.AggregateConditionsToWheresql(conditions, isLike));

        //            if (conditions == null)
        //            {
        //                conditions = new Dictionary<string, object>();
        //            }
        //            conditions.Add("pageIndex", pageIndex);
        //            conditions.Add("pageSize", pageSize);
        //            var table = SqlHelper.ExecuteDataTable(sql, conditions, isLike);
        //            return table;
        //        }

        //-----------------------------------------------------------------------------------------------------
        public static DataTable GetNonPagedDataTable(string tableName, Dictionary<string, object> conditions = null, bool isLike = false)
        {

            var sql = string.Format(@" select  *  from {0} where 1=1 {1} ",
            tableName,
            SqlHelper.AggregateConditionsToWheresql("", conditions, isLike));
            var table = SqlHelper.ExecuteDataTable(sql, conditions, isLike);
            return table;
        }


        public static string GetMD5Sault()
        {
            return GetNonPagedDataTable("plcs_appconfig",
                new Dictionary<string, object> { { "AppGroup", "MD5Sault" } })
                .Rows[0]["AppGroupValue"].ToString();
        }

        public static List<AuthRegionAndDeptModel> GetAppConfigList(Dictionary<string, object> conditions)
        {
            var table = GetNonPagedDataTable("plcs_appconfig", conditions);
            return ListHelper.ConvertDataTableToList<AuthRegionAndDeptModel>(table);
        }


        public static void UserInfoValidate(string httpUserName, string httpPassWord)
        {
            var md5 = MD5.Create();
            var input = httpPassWord + GetMD5Sault();
            var table = GetNonPagedDataTable("PLCS_UserInfo",
                new Dictionary<string, object> { { "UserName", httpUserName } });
            var hash = table.Rows.Count > 0 ? table.Rows[0]["password"].ToString() : "";
            if (!VerifyMd5Hash(md5, input, hash))
            {
                HttpContext.Current.Response.Redirect("/home/index");
            }
            else
            {
                CommonHelper.Cookies("LoginUserName", "", CookiesEnum.Get);
            }

        }

        public static string GetMd5Hash(MD5 md5Hash, string input)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            foreach (byte t in data)
            {
                sBuilder.Append(t.ToString("x2"));
            }
            return sBuilder.ToString();
        }

        public static bool VerifyMd5Hash(MD5 md5Hash, string hash, string input)
        {
            using (md5Hash)
            {
                string hashOfInput = GetMd5Hash(md5Hash, input);
                StringComparer comparer = StringComparer.OrdinalIgnoreCase;
                return comparer.Compare(hashOfInput, hash) == 0;
            }

        }

        public static void AuthValidation()
        {
            if (string.IsNullOrEmpty(CommonHelper.Cookies("LoginUserName", "", CookiesEnum.Get)))
            {
                HttpContext.Current.Response.Redirect("/home/index/");
            }
        }


        public static void RenderToExcel<T>(HSSFWorkbook book, HSSFSheet sheet, int pageIndex, int pageSize, List<T> list, string[] firstColumnName, int fromColumnIndex = 0, int endColumnIndex = 0, bool isFirstTime = false, bool isLastTime = false)
        {
            if (book == null)
            {
                book = new HSSFWorkbook();
            }
            if (sheet == null)
            {
                sheet = (HSSFSheet)book.CreateSheet("Report");
            }
            //var book = new HSSFWorkbook();
            if (isFirstTime)
            {
                var firstRow = sheet.CreateRow(0);
                for (int x = 0; x < firstColumnName.Length; x++)
                {
                    firstRow.CreateCell(x).SetCellValue(firstColumnName[x]);
                    //不起作用,NPOIBUG?
                    //sheet.AutoSizeColumn(x);
                }
            }

            int currentRow = (pageIndex - 1) * pageSize + 1;
            foreach (var data in list)
            {
                var row = sheet.CreateRow(currentRow);
                var classProperties = typeof(T).GetProperties();
                for (int i = (fromColumnIndex == 0 ? 0 : fromColumnIndex);
                    i < (endColumnIndex == 0 ? classProperties.Length : endColumnIndex); i++)
                {
                    row.CreateCell(fromColumnIndex == 0 ? i : i - fromColumnIndex)
                       .SetCellValue(classProperties[i].GetValue(data) == null ? "" : classProperties[i].GetValue(data).ToString());
                }
                currentRow++;
            }

            HttpContext.Current.Session["book"] = book;
            HttpContext.Current.Session["sheet"] = sheet;
            //篩選
            //sheet.SetAutoFilter(new NPOI.SS.Util.CellRangeAddress(0, list.Count-1, 0, firstColumnName.Length-1));

            if (isLastTime)
            {
                MemoryStream ms = new MemoryStream();
                using (ms)
                {
                    book.Write(ms);
                    HttpContext.Current.Response.AddHeader("Content-Disposition",
                        String.Format("attachment;filename={0}.xls", DateTime.Now.ToString("yyyyMMHHss")));
                    HttpContext.Current.Response.BinaryWrite(ms.ToArray());
                    book = null;
                    ms.Close();
                    ms.Dispose();

                }
                HttpContext.Current.Session["book"] = null;
                HttpContext.Current.Session["sheet"] = null;
                HttpContext.Current.Session["list"] = null;
            }


        }


        public static string UploadByExcel(HttpPostedFileBase file = null)
        {

            if (file == null || file.FileName == "")
            {
                return "請先選擇文件!";
            }
            var wb = new HSSFWorkbook();
            var filePath = Path.Combine(HttpContext.Current.Server.MapPath("/bin"), Path.GetFileName(file.FileName));
            file.SaveAs(filePath);
            var tempFileName = Guid.NewGuid().ToString();
            var tmpFilePath = Path.Combine(HttpContext.Current.Server.MapPath("/bin"), tempFileName + ".xls");
            System.IO.File.Move(filePath, tmpFilePath);
            System.IO.File.Delete(filePath);
            var fileStram = new System.IO.FileStream(tmpFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            using (fileStram)
            {
                wb = new HSSFWorkbook(fileStram);
            }
            var sheet = wb.GetSheetAt(0);
            var list = new List<UploadClassModel>();
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {

                var row = sheet.GetRow(i);
                if (row.GetCell(0).StringCellValue == "")
                {
                    break;
                }

                list.Add(new UploadClassModel
                {
                    Region = row.GetCell(0) == null ? "" : row.GetCell(0).ToString().Trim(),
                    UserId = row.GetCell(1) == null ? "" : row.GetCell(1).ToString().Trim(),
                    OrgDept = row.GetCell(2) == null ? "" : row.GetCell(2).ToString().Trim(),
                    ClosetNo = row.GetCell(3) == null ? "" : row.GetCell(3).ToString().Trim(),
                    ClosetNorm = row.GetCell(4) == null ? "" : row.GetCell(4).ToString().Trim(),
                    ClosetRemark = row.GetCell(5) == null ? "" : row.GetCell(5).ToString().Trim(),
                    SignDate = row.GetCell(6) == null ? null : (DateTime?)Convert.ToDateTime(row.GetCell(6).ToString().Trim()),
                    Remark = row.GetCell(7) == null ? "" : row.GetCell(7).ToString().Trim()
                });
            }

            //不爲空檢查
            var tmplist = new List<UploadClassModel>();
            tmplist = list.Where(x => x.ClosetNo == "" || x.Region == "").ToList();
            if (tmplist.Count > 0)
            {
                return "區域和序號不允許爲空" +
                       "<hr/>" +
                       "<table>" +
                       "<thead>" +
                       "<th>區域</th><th>工號</th><th>所屬部門</th><th>序號</th><th>型號</th><th>描述</th><th>日期</th><th>備註</th>" +
                       "</thead>" +
                       "<tbody>" +
                       tmplist.Aggregate("",
                       (currenct, item) => currenct
                           + ("<tr>" +
                              "<td>" + item.Region + "</td>" +
                              "<td>" + item.UserId + "</td>" +
                              "<td>" + item.OrgDept + "</td>" +
                              "<td>" + item.ClosetNo + "</td>" +
                              "<td>" + item.ClosetNorm + "</td>" +
                              "<td>" + item.ClosetRemark + "</td>" +
                              "<td>" + item.SignDate + "</td>" +
                              "<td>" + item.Remark + "</td>" +
                              "</tr>"))
                       + "</tbody>" +
                       "</tbale>";
            }

            //查找重複序號
            tmplist = (from q in list
                       where (from g in list
                              group g by g.ClosetNo
                                  into duplicate
                                  where duplicate.Count() > 1
                                  select duplicate.Key).Contains(q.ClosetNo)
                       orderby q.ClosetNo
                       select q).ToList();
            if (tmplist.Count > 0)
            {
                return "序號有重複!請確認!" +
                       "<hr/>" +
                       "<table>" +
                       "<thead>" +
                       "<th>區域</th><th>工號</th><th>所屬部門</th><th>序號</th><th>型號</th><th>描述</th><th>日期</th><th>備註</th>" +
                       "</thead>" +
                       "<tbody>" +
                       tmplist.Aggregate("",
                       (currenct, item) => currenct
                           + ("<tr>" +
                              "<td>" + item.Region + "</td>" +
                              "<td>" + item.UserId + "</td>" +
                              "<td>" + item.OrgDept + "</td>" +
                              "<td>" + item.ClosetNo + "</td>" +
                              "<td>" + item.ClosetNorm + "</td>" +
                              "<td>" + item.ClosetRemark + "</td>" +
                              "<td>" + item.SignDate + "</td>" +
                              "<td>" + item.Remark + "</td>" +
                              "</tr>"))
                       + "</tbody>" +
                       "</tbale>";
            }


            //查找重複工號
            tmplist = (from q in list
                       where (from g in list.Where(x => x.UserId != "")
                              group g by g.UserId
                                  into duplicate
                                  where duplicate.Count() > 1
                                  select duplicate.Key).Contains(q.UserId)
                       orderby q.UserId
                       select q).ToList();
            if (tmplist.Count > 0)
            {
                return "工號有重複!請確認!" +
                       "<hr/>" +
                       "<table>" +
                       "<thead>" +
                       "<th>區域</th><th>工號</th><th>所屬部門</th><th>序號</th><th>型號</th><th>描述</th><th>日期</th><th>備註</th>" +
                       "</thead>" +
                       "<tbody>" +
                       tmplist.Aggregate("",
                       (currenct, item) => currenct
                           + ("<tr>" +
                              "<td>" + item.Region + "</td>" +
                              "<td>" + item.UserId + "</td>" +
                              "<td>" + item.OrgDept + "</td>" +
                              "<td>" + item.ClosetNo + "</td>" +
                              "<td>" + item.ClosetNorm + "</td>" +
                              "<td>" + item.ClosetRemark + "</td>" +
                              "<td>" + item.SignDate + "</td>" +
                              "<td>" + item.Remark + "</td>" +
                              "</tr>"))
                       + "</tbody>" +
                       "</tbale>";
            }

            CommonHelper.Cookies("uploadlistCount", list.Count.ToString(), CookiesEnum.Set);
            CommonHelper.Cookies("uploadgroupid", Guid.NewGuid().ToString(), CookiesEnum.Set);

            foreach (UploadClassModel item in list)
            {
                if (!HandleUploadClassModel(item))
                {
                    return item.ClosetNo + "上傳失敗!請聯繫MIS!";
                }
            }



            return "done";
        }


        public static bool HandleUploadClassModel(UploadClassModel item)
        {
            var groupid = Guid.Parse(CommonHelper.Cookies("uploadgroupid", "", CookiesEnum.Get));
            return SqlHelper.ExecuteNonQuery(@"
                        INSERT INTO [dbo].[Plcs_Closet_Upload]
                                   ([GroupId]
                                   ,[Region]
                                   ,[UserId]
                                   ,[OrgDept]
                                   ,[ClosetNo]
                                   ,[ClosetNorm]
                                   ,[ClosetRemark]
                                   ,[SignDate]
                                   ,[Remark]
                                   ,[CrtDate]
                                   ,[UploadUser])
                             VALUES
                                   (@GroupId
                                   ,@Region
                                   ,@UserId
                                   ,@OrgDept
                                   ,@ClosetNo
                                   ,@ClosetNorm
                                   ,@ClosetRemark
                                   ,@SignDate
                                   ,@Remark
                                   ,getdate()
                                   ,@currentUser)", new Dictionary<string, object>
                        {
                            {"GroupId",groupid},
                            {"Region",item.Region},
                            {"UserId",item.UserId},
                            {"OrgDept",item.OrgDept},
                            {"ClosetNo",item.ClosetNo},
                            {"ClosetNorm",item.ClosetNorm},
                            {"ClosetRemark",item.ClosetRemark},
                            {"SignDate",item.SignDate},
                            {"Remark",item.Remark},
                            {"currentUser",CommonHelper.Cookies("LoginUserName","",CookiesEnum.Get)}
                        }) > 0;
        }




        public static string UploadExcelValidate()
        {
            var currentUser = CommonHelper.Cookies("LoginUserName", "", CookiesEnum.Get);
            var groupId = Guid.Parse(CommonHelper.Cookies("uploadgroupid", "", CookiesEnum.Get));
            var table = SqlHelper.ExecuteDataTable(@" exec PLCS_UploadValidation @currentUser,@groupId ",
                new Dictionary<string, object>
                {
                    {"currentUser",currentUser},
                    {"groupId",groupId}
                });
            if (table.Rows.Count > 0)
            {
                return "您沒有操作以下數據的權限.請確認." +
                       "<hr/>" +
                       "<table>" +
                       "<thead>" +
                       "<th>區域</th><th>工號</th><th>姓名</th><th>部門</th><th>所屬部門</th><th>序號</th>" +
                       "</thead>" +
                       "<tbody>" +
                       table.Rows.Cast<DataRow>().Aggregate("",
                       (current, row) =>
                           current + ("<tr><td>" + row.ItemArray[0] + "</td>" +
                           "<td>" + row.ItemArray[1] + "</td>" +
                           "<td>" + row.ItemArray[2] + "</td>" +
                           "<td>" + row.ItemArray[3] + "</td>" +
                           "<td>" + row.ItemArray[4] + "</td>" +
                           "<td>" + row.ItemArray[5] + "</td></tr>"))
                       + "</tbody>" +
                       "</tbale>";
            }
            return "done";
        }

        public static string UploadExcelProcess()
        {
            var currentUser = CommonHelper.Cookies("LoginUserName", "", CookiesEnum.Get);
            var groupId = Guid.Parse(CommonHelper.Cookies("uploadgroupid", "", CookiesEnum.Get));

            SqlHelper.ExecuteNonQuery(@" exec PLCS_UploadProcess @currentUser,@groupId",
                new Dictionary<string, object>{
                    {"currentUser",currentUser},
                    {"groupId",groupId}
                });

            return "done";
        }




    }

}

