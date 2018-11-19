using System.Collections.Generic;

namespace Public.Core.PageList
{
    public class PageList<T>
    {
        /// <summary>
        /// 数据
        /// </summary>
        public IEnumerable<T> Data { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPageCount { get; set; }
        /// <summary>
        /// 当前页索引
        /// </summary>
        public int CurrentPageIndex { get; set; }
        /// <summary>
        /// 每页记录数量
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 总共记录数
        /// </summary>
        public int TotalItemCount { get; set; }

    }
}
