using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using PLCS.Models;

namespace PLCS.Services
{
    public class ThemeService
    {
        public static DataTable GetTheme(string userid)
        {
            var table = SqlHelper.ExecuteDataTable("select * from PLCS_Theme where userid=@userid ",
                new Dictionary<string, object>
                {
                    {"userid",userid}
                });
            return table;
        }

        public static Boolean SaveThem(ThemeModel theme)
        {
            return true;
        }
    }
}