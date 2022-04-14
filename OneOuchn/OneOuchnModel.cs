using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneOuchn
{
    public class Configure
    {
        public string Cookie { get; set; }
        public bool CookieLogin { get; set; }
        public string UserNo { get; set; }
        public string Password { get; set; }
        public int MinSeconds { get; set; }
        public int MaxSeconds
        {
            get; set;
        }
    }

    /// <summary>
    /// 课程模块
    /// </summary>
    public class CourseModule
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    /// <summary>
    /// 课程模块的学习课程
    /// </summary>
    public class LearnActivitie
    {
        public string id { get; set; }

        /// <summary>
        /// 完成标注
        /// </summary>
        public string completion_criterion { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<Upload> uploads { get; set; }
    }

    /// <summary>
    /// 视频
    /// </summary>
    public class Videos
    {
        public decimal duration;
    }

    /// <summary>
    /// 文件
    /// </summary>
    public class Upload
    {
        public int id { get; set; }

        public List<Videos> videos { get; set; }
    }

}

