using PuppeteerSharp;


namespace Ouchn;


public class LearningCourse
{
    /// <summary>
    /// 学分
    /// </summary>
    public double Credit { get; set; }

    /// <summary>
    /// 学习进度
    /// </summary>
    public double ExamScore { get; set; }

    /// <summary>
    /// 课程类别
    /// </summary>
    public string CourseNatureName { get; set; }

    /// <summary>
    /// 课程名称
    /// </summary>
    public string CourseName { get; set; }

    /// <summary>
    /// 班级名称
    /// </summary>
    public string CourseClassName { get; set; }

    /// <summary>
    /// 未完成的作业和测验
    /// </summary>
    public string UnCompletedTestAssignCount { get; set; }

    /// <summary>
    /// 已完成的作业和测验
    /// </summary>
    public double CompletedFormativeTestCount { get; set; }

    /// <summary>
    /// 课程网址
    /// </summary>
    public string CourseUrl { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Page Page { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public double ActivityCount { get; set; }
}

public class Configure
{
    public string UserNo { get; set; }
    public string Password { get; set; }
    public int MinSeconds { get; set; }
    public int MaxSeconds { get; set; }
    public bool Debug { get; set; }
}

