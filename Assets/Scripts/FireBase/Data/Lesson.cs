using System;
using System.Collections.Generic;

public class LessonFireBase
{
    public const string Key = "Lessons";
    public const string Key_NAME = "Name";
    public const string Key_TYPE = "Type";
    public string Name { get; set; }
    public TypeLesson type { get; set; }

    public List<StudentMissing> StudentMissings = new();

    public enum TypeLesson {
        Lecture, Practice, Curatorial, PhysicalEducation
    }

    public static TypeLesson isTypeLesson(string type) {
        switch (type)
        {
            case "ËÇ":
                return TypeLesson.Lecture;
            case "ÏÇ":
                return TypeLesson.Practice;
            case "ÊÇ":
                return TypeLesson.Curatorial;
            case "ÔÇ":
                return TypeLesson.PhysicalEducation;
            default:
                return TypeLesson.Lecture;
        }
    }

    public static string ConvertToString(TypeLesson type) {
        switch (type)
        {
            case TypeLesson.Lecture:
                return "ËÇ";
            case TypeLesson.Practice:
                return "ÏÇ";
            case TypeLesson.Curatorial:
                return "ÊÇ";
            case TypeLesson.PhysicalEducation:
                return "ÔÇ";
            default:
                return "ËÇ";
        }
    }
}
