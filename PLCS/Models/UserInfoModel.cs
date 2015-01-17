using PLCS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PLCS.Models
{
    public class UserInfoModel
    {
        public Guid Id { set; get; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool Valid { get; set; }
        public bool Active { get; set; }
     
    }


}