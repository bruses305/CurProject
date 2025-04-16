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
            case "��":
                return TypeLesson.Lecture;
            case "��":
                return TypeLesson.Practice;
            case "��":
                return TypeLesson.Curatorial;
            case "��":
                return TypeLesson.PhysicalEducation;
            default:
                return TypeLesson.Lecture;
        }
    }

    public static string ConvertToString(TypeLesson type) {
        switch (type)
        {
            case TypeLesson.Lecture:
                return "��";
            case TypeLesson.Practice:
                return "��";
            case TypeLesson.Curatorial:
                return "��";
            case TypeLesson.PhysicalEducation:
                return "��";
            default:
                return "��";
        }
    }
}
