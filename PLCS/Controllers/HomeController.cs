using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PLCS.Models;
using System.Security.Cryptography;
using PLCS.Services;

namespace PLCS.Controllers
{
    //-----------------------------------------------------------------------------------------------------
    public class HomeController : Controller
    {
        // GET: Home
        //-----------------------------------------------------------------------------------------------------
        public ActionResult Index()
        {
            Session.RemoveAll();
            CommonHelper.RemoveCookies("plcs");
            ViewBag.LoginUserName = CommonHelper.Cookies("LoginUserName", "", CookiesEnum.Get);
            ViewBag.RememberMe = CommonHelper.Cookies("RememberMe", "", CookiesEnum.Get);
            return View("~/views/home/welcome.cshtml");
        }

        [HttpPost]
        public ActionResult Login()
        {
            var httpUserName = HttpContext.Request["LoginUserName"];
            var httpPassword = HttpContext.Request["LoginPassword"];
            var httpRememberMe = HttpContext.Request["rememberMe"];
            var md5 = MD5.Create();
            var table = CommonHelper.GetNonPagedDataTable("plcs_userInfo", new Dictionary<string, object> { { "UserName", httpUserName } });
            var input = table.Rows.Count > 0 ? table.Rows[0]["Password"].ToString() : "";
            if (CommonHelper.VerifyMd5Hash(md5, input, httpPassword + CommonHelper.GetMD5Sault()))
            {
                Session["UserInfoModel"] = new UserInfoModel
                {
                    UserName = httpUserName,
                    Active = true
                };
                CommonHelper.Cookies("LoginUserName", httpUserName, CookiesEnum.Set);
                CommonHelper.Cookies("rememberMe", httpRememberMe, CookiesEnum.Set);
                return RedirectToAction("../management/management/");
            }
            else
            {
                ViewBag.Msg = "用戶或密碼錯誤!";
                ViewBag.LoginUserName = httpUserName;
                ViewBag.RememberMe = httpRememberMe;
                CommonHelper.Cookies("LoginUserName", "", CookiesEnum.Set);
                return View("~/views/home/welcome.cshtml");
            }
        }
    }
}

