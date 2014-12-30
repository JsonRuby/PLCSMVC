using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PLCS.Models;
using PLCS.Services;

namespace PLCS.Controllers
{
    public class ManagementController : Controller
    {
        //
        // GET: /Management/
        public ActionResult Management(string id)
        {

            var tmpList = id == null ? new[] { "1", "10" } : id.Split('-').ToArray();
            
            var pageInfo = new PageInfoModel("PLCS_Closet")
            {
                PageSize = int.Parse(tmpList[1]),
                PageIndex = int.Parse(tmpList[0])
            };

            var maxPageNumber = (int)Math.Ceiling((double)pageInfo.Count / pageInfo.PageSize);

            #region 分頁處理

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



            #endregion

            ViewBag.PageInfo = pageInfo;
            ViewBag.MaxPageNumber = maxPageNumber;
            ViewBag.listPageNumberModel = listPageNumberModel;
            ViewBag.TableRows = ManagementService.GetData(pageInfo);

            return View("Management");
        }
    }
}