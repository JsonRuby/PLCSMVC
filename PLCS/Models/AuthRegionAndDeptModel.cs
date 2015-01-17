using PLCS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PLCS.Models
{
    public class AuthRegionAndDeptModel
    {

        private List<RegionItem> _regionCollection;

        public List<RegionItem> RegionColletion
        {
            get { return _regionCollection; }
            set
            {
                _regionCollection = value =
                    ListHelper.ConvertDataTableToList<RegionItem>(
                    SqlHelper.ExecuteDataTable(@"select appgroupvalue 
                                                from plcs_appconfig     
                                                where appgroup=@AppGroup and AppGroupKey=@AppGroupKey ",
                    new Dictionary<string, object>{
                    {"AppGroup","Region"},
                    {"AppGroupKey",CommonHelper.Cookies("LoginUserName","",CookiesEnum.Get)}}));
            }
        }


        private List<DeptItem> _deptColletion;

        public List<DeptItem> DeptCollection
        {
            get { return _deptColletion; }
            set
            {
                _deptColletion = value =
                  ListHelper.ConvertDataTableToList<DeptItem>(
                  SqlHelper.ExecuteDataTable(@"select appgroupvalue 
                                                from plcs_appconfig     
                                                where appgroup=@AppGroup and AppGroupKey=@AppGroupKey ",
                  new Dictionary<string, object>{
                    {"AppGroup","Dept"},
                    {"AppGroupKey",CommonHelper.Cookies("LoginUserName","",CookiesEnum.Get)}})); ;
            }
        }

    }

    public class RegionItem
    {
        public string Region { get; set; }
    }

    public class DeptItem
    {
        public string Dept { set; get; }
    }
}