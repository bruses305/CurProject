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
        Lecture,
        Laborator, 
        Practice, 
        Curatorial, 
        PhysicalEducation
    }

    public static TypeLesson isTypeLesson(string type) {
        type = type.ToUpper();
        switch (type)
        {
            case "��":
                return TypeLesson.Lecture;
            case "���":
                return TypeLesson.Lecture;
            case "���":
                return TypeLesson.Laborator;
            case "���":
                return TypeLesson.Laborator;
            case "��":
                return TypeLesson.Practice;
            case "����":
                return TypeLesson.Practice;
            case "��":
                return TypeLesson.Curatorial;
            case "�������������� ���":
                return TypeLesson.Curatorial;
            case "����������� ���":
                return TypeLesson.Curatorial;
            case "���������� ��������":
                return TypeLesson.PhysicalEducation;
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
            case TypeLesson.Laborator:
                return "���";
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
