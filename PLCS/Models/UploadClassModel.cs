﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PLCS.Models
{
    public class UploadClassModel
    {
        public string Region { get; set; }
        public string UserId { get; set; }
        public string ClosetNo { get; set; }
        public string OrgDept { get; set; }
        public string ClosetNorm { get; set; }
        public string ClosetRemark { get; set; }
        public DateTime? SignDate { get; set; }
        public string Remark { get; set; }
    }
}