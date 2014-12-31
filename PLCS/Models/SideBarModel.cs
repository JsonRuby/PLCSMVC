using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PLCS.Models
{
    public class SideBarModel
    {
        public string SideBarGroupName { get; set; }
        public string SideBarGroupIcon { get; set; }
        public bool SideBarGroupActive { get; set; }
        public string SideBarItemName { get; set; }
        public string SideBarItemIcon { get; set; }
        public bool SideBarItemActive { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public dynamic Parameters { get; set; }
    }
}