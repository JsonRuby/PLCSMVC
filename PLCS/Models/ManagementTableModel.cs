﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PLCS.Models
{
    public  class ManagementTableModel
    {
        public Guid Id { get; set; }
        public string Region { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Dept { get; set; }
        public string OrgDept { get; set; }
        public string ClosetNo { get; set; }
        public string ClosetNorm { get; set; }
        public string ClosetRemark { get; set; }
        public DateTime? SignDate { get; set; }

        //private DateTime? _SignDate;
        
        //public DateTime? SignDate
        //{
        //    get
        //    {
        //        //return _SignDate==null
        //        //    ?(DateTime?)null
        //        //    : new DateTime(_SignDate.Value.Year, _SignDate.Value.Month, _SignDate.Value.Day);
        //        return _SignDate;
        //    }
        //    set
        //    {
        //        _SignDate = value;
        //    }
        //}
         
        public string Remark { get; set; }
    }
}