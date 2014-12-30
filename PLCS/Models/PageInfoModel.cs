using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PLCS.Services;

namespace PLCS.Models
{
    public class PageInfoModel
    {
        public PageInfoModel(string tableName)
        {
            Count = CommonHelper.GetTableCount(tableName);
        }

        public int Count { get; private set; }

        private int _pageIndex;

        public int PageIndex
        {
            get { return _pageIndex; }
            set
            {
                _pageIndex = value < 1
                    ? 1
                    : value * _pageSize > Count ? (int)Math.Ceiling((double)Count / _pageSize) : value;
            }
        }

        private int _pageSize;

        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value > Count ? Count : value; }
        }

        //public int NewPageSize { get; set; }
        //public IntAlternateEum FullOrSmall { get; set; }
    }
}