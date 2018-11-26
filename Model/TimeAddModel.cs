using System;

namespace Model
{
    /// <summary>
    /// 列表数据的模型
    /// </summary>
    public class TimeAddModel
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int RowIndex { get; set; }

        /// <summary>
        /// 输入的内容
        /// </summary>
        public string AddContent { get; set; }

        /// <summary>
        /// 是否勾选
        /// </summary>
        public bool IsCheck { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        public string Photo { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime AddTime { get; set; }
    }
}
