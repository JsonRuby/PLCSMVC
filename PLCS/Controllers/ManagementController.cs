using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NPOI.HSSF.UserModel;
using PLCS.Models;
using PLCS.Services;
using System.Reflection;
using Newtonsoft.Json;

namespace PLCS.Controllers
{
    //-----------------------------------------------------------------------------------------------------
    public class ManagementController : Controller
    {

        //
        // GET: /Management/
        //-----------------------------------------------------------------------------------------------------
        public ActionResult Management(string id)
        {
            CommonHelper.AuthValidation();

            var activeFilterModel = new ActiveFilterDataModel();
            var activeDict = new Dictionary<string, object>();

            var properties = activeFilterModel.GetType().GetProperties();
            foreach (PropertyInfo p in properties.Where(p => !string.IsNullOrEmpty(HttpContext.Request[p.Name])))
            {
                p.SetValue(activeFilterModel, HttpContext.Request[p.Name]);
                activeDict.Add(p.Name, HttpContext.Request[p.Name]);
            }



            Session["conditions"] = activeDict;

            var tmpList = id == null ? new[] { "1", "10" } : id.Split('-').ToArray();
            var tableCount =
                ManagementService.GetNonPagedDataCount(conditions: activeDict, isLike: false);

            var pageInfo = new PageInfoModel(tableCount)
            {
                PageSize = int.Parse(tmpList[1]),
                PageIndex = int.Parse(tmpList[0])
            };

            var maxPageNumber = (int)Math.Ceiling((double)pageInfo.Count / pageInfo.PageSize);

            //-----------------------------------------------------------------------------------------------------
            #region 分頁處理
            //-----------------------------------------------------------------------------------------------------

            var listPageNumberModel = new List<PageNumberModel>();

            if (maxPageNumber <= 10) //前10個
            {
                for (int i = 1; i <= maxPageNumber; i++)
                {
                    listPageNumberModel.Add(new PageNumberModel
                    {
                        PageNumber = i
                    });
                }
            }
            else if (pageInfo.PageIndex / 10 == maxPageNumber / 10 && (pageInfo.PageIndex + 10) % 10 != 0)//最後x個
            {
                //79-73=6; 79-3=73
                for (int i = (pageInfo.PageIndex / 10) * 10 + 1; i <= maxPageNumber; i++)
                {
                    listPageNumberModel.Add(new PageNumberModel
                    {
                        PageNumber = i
                    });
                }
            }
            else //其他
            {
                //10%10=0,(20/10)*10-=10;
                var tmpRemainder = (pageInfo.PageIndex + 10) % 10;
                var tmpStart =
                tmpRemainder == 0
                ? (pageInfo.PageIndex / 10 - 1) * 10 + 1
                : pageInfo.PageIndex - tmpRemainder + 1;//31

                var tmpEnd = tmpStart + 9;//40

                for (int i = tmpStart; i <= tmpEnd; i++)
                {
                    listPageNumberModel.Add(new PageNumberModel
                    {
                        PageNumber = i
                    });
                }
            }

            //-----------------------------------------------------------------------------------------------------
            #endregion
            //-----------------------------------------------------------------------------------------------------

            ViewBag.PageInfo = pageInfo;
            ViewBag.MaxPageNumber = maxPageNumber;
            ViewBag.listPageNumberModel = listPageNumberModel;
            ViewBag.TableRows = ManagementService.GetPagedData(pageInfo, conditions: activeDict, isLike: false);
            ViewBag.TableHead = ManagementService.GetTableHead();

            ViewBag.SideBarList = SideBarList.GetSideBarList("");

            ViewBag.FilterData = new FilterDataModel();
            ViewBag.ActiveFilterData = activeFilterModel;

            return View("Management");


        }

        //-----------------------------------------------------------------------------------------------------
        [HttpPost]
        public ActionResult Save()
        {
            CommonHelper.AuthValidation();
            ManagementTableModel tableModel = new ManagementTableModel();

            var properties = tableModel.GetType().GetProperties();
            foreach (PropertyInfo p in properties)
            {
                if (p.PropertyType == typeof(Guid))
                {
                    p.SetValue(tableModel, new Guid(HttpContext.Request["data[" + p.Name + "]"]));
                }
                else if (p.PropertyType == typeof(DateTime?))
                {
                    p.SetValue(tableModel, HttpContext.Request["data[" + p.Name + "]"] == ""
                        ? (DateTime?)null : Convert.ToDateTime(HttpContext.Request["data[" + p.Name + "]"]));
                }
                else
                {
                    p.SetValue(tableModel, HttpContext.Request["data[" + p.Name + "]"]);
                }
            }

            List<ManagementTableModel> existUserModel =
                ManagementService.GetNonPagedData(conditions: new Dictionary<string, object> { { "UserId", tableModel.UserId } });

            List<ManagementTableModel> list = existUserModel
                .Where(x => x.ClosetNo != tableModel.ClosetNo && !String.IsNullOrEmpty(x.UserId)).ToList();
            if (list.Count > 0)
            {
                return Content(JsonConvert.SerializeObject(list));
            }
            var tmplist = new List<ManagementTableModel> { tableModel };
            if (tmplist.Where(x => x.Dept != "" && x.Dept.Substring(0, 4) != x.OrgDept.Substring(0, 4)).ToList().Count > 0)
            {
                return Content("errorDept");
            }
            return SqlHelper.UpdateDataTable("plcs_closet", tmplist, "Id") > 0 ? Content("true") : Content("false");
        }

        //-----------------------------------------------------------------------------------------------------
        [HttpPost]
        public ActionResult GetNameAndDept()
        {
            CommonHelper.AuthValidation();
            var userId = HttpContext.Request["UserId"];
            var userInfo = new EmpInfoModel(UserId: userId);
            return Content(userInfo.UserName + "|" + userInfo.UserDept);
        }

        [HttpPost]
        public ActionResult Delete()
        {
            CommonHelper.AuthValidation();
            var tmpId = HttpContext.Request["Id"];
            if (tmpId == "")
            {
                return Content("未獲取到Id參數,請聯繫系統管理員!");
            }
            else
            {
                var tmpModel = ManagementService.GetNonPagedData(conditions: new Dictionary<string, object>
                {
                    {"Id",tmpId}
                });
                foreach (var model in tmpModel)
                {
                    model.UserId = null;
                    model.Name = null;
                    model.Dept = null;
                    model.SignDate = null;
                }
                return Content(SqlHelper.UpdateDataTable("plcs_closet", tmpModel, "Id") > 0 ? "true" : "false");
            }
        }




        [HttpPost]
        public ActionResult UploadExcel()
        {
            HttpPostedFileBase file = Request.Files["uploadfie"];
            if (file != null)
            {
                CommonHelper.AuthValidation();
                var result = CommonHelper.UploadByExcel(file: file);
                if (result == "done")
                {
                    return Redirect("~/Iframe/MUploadStep2.html");
                }
                else
                {
                    return Content(result);
                }

            }
            else
            {
                return Content("請先選擇文件!");
            }

        }

        [HttpPost]
        public ActionResult UploadExcelProcess()
        {
            var result = CommonHelper.UploadExcelProcess();

            if (result == "done")
            {
                return Redirect("~/Iframe/MUploadStep4.html");
            }
            else
            {
                return Content(result);
            }
        }

        [HttpPost]
        public ActionResult UploadExcelValidate()
        {
            var result = CommonHelper.UploadExcelValidate();

            if (result == "done")
            {
                return Redirect("~/Iframe/MUploadStep3.html");
            }
            else
            {
                return Content(result);
            }
        }

        [HttpPost]
        public ActionResult UploadResultCount()
        {
            return Content("共處理<strong>" + CommonHelper.Cookies("uploadlistCount", "", CookiesEnum.Get) + "</strong>條記錄!");
        }

        [HttpPost]
        public ActionResult NonPagedCount()
        {
            return Content(ManagementService.GetNonPagedDataCount(conditions: (Dictionary<string, object>)Session["conditions"], isLike: false).ToString());
        }


        [HttpPost]
        public ActionResult GenerateExcel(string times)
        {
            var fColumnName = new[]
            {
                "區域",
                "工號",
                "姓名",
                "部門",
                "序號所屬部門",
                "序號",
                "型號",
                "描述",
                "日期",
                "備註"
            };

            var pageIndex = HttpContext.Request["pageIndex"];
            var pageCount = HttpContext.Request["pageCount"];
            var pageSize = HttpContext.Request["pageSize"];
            var isFirstTime = (pageIndex == "1");
            var isLastTime = (pageIndex == pageCount);

            if (pageIndex == "1")
            {
                CommonHelper.AuthValidation();
                Session["list"] =
                    ManagementService.GetNonPagedData(conditions: (Dictionary<string, object>)Session["conditions"]);
            }
            var list = (List<ManagementTableModel>)Session["list"];
            list = list.Skip(((int.Parse(pageIndex) - 1) * int.Parse(pageSize))).Take(int.Parse(pageSize)).ToList();
            CommonHelper.RenderToExcel((HSSFWorkbook)Session["book"],
                (HSSFSheet)Session["sheet"], int.Parse(pageIndex), int.Parse(pageSize), list, fColumnName, 1, 0, Convert.ToBoolean(isFirstTime), Convert.ToBoolean(isLastTime));

            return Content("");
            // return Content(int.Parse(pageIndex) == int.Parse(pageCount) - 1 ? null : "processing");
        }



    }
}

