using PLCS.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace PLCS.Models
{
    public class FilterDataModel
    {
        private DataRowCollection _regionRows = ManagementService.GetFilterDataRows("region");
        private List<string> _region = new List<string>();
        public List<string> Region
        {
            get
            {                
                foreach (DataRow row in _regionRows)
                {
                    _region.Add(row.ItemArray.Length > 0 ? row.ItemArray[0].ToString() : "");

                }
                return _region;
            }
        }


        private DataRowCollection _deptRows = ManagementService.GetFilterDataRows("dept");
        private List<string> _dept =  new List<string>();
        public List<string> Dept
        {
            get
            {
                foreach (DataRow row in _deptRows)
                {
                    _dept.Add(row.ItemArray.Length > 0 ? row.ItemArray[0].ToString() : "");

                }
                return _dept;
            }
        }

        private DataRowCollection _orgDeptRows = ManagementService.GetFilterDataRows("dept");
        private List<string> _orgDept = new List<string>();
        public List<string> OrgDept
        {
            get
            {
                foreach (DataRow row in _orgDeptRows)
                {
                    _orgDept.Add(row.ItemArray.Length > 0 ? row.ItemArray[0].ToString() : "");

                }
                return _orgDept;
            }
        }


        private DataRowCollection _closetRemarkRows = ManagementService.GetFilterDataRows("closetremark");
        private List<string> _closetRemark =  new List<string>();
        public List<string> ClosetRemark
        {
            get
            {
                foreach (DataRow row in _closetRemarkRows)
                {
                    _closetRemark.Add(row.ItemArray.Length > 0 ? row.ItemArray[0].ToString() : "");

                }
                return _closetRemark;
            }
        }


        private DataRowCollection _remarkRows = ManagementService.GetFilterDataRows("remark");
        private List<string> _remark =  new List<string>();
        public List<string> Remark
        {
            get
            {
                foreach (DataRow row in _remarkRows)
                {
                    _remark.Add(row.ItemArray.Length>0?row.ItemArray[0].ToString():"");

                }
                return _remark;
            }
        }

        private DataRowCollection _closetNormkRows = ManagementService.GetFilterDataRows("closetNorm");
        private List<string> _closetNorm = new List<string>();
        public List<string> ClosetNorm
        {
            get
            {
                foreach (DataRow row in _closetNormkRows)
                {
                    _closetNorm.Add(row.ItemArray.Length > 0 ? row.ItemArray[0].ToString() : "");

                }
                return _closetNorm;
            }
        }

    }
}