using System;
using System.Collections.Generic;

public class LessonFireBase
{
    public const string Key = "Lessons";
    public const string Key_NAME = "Name";
    public const string Key_TYPE = "Type";
    public string Name { get; set; }
    public TypeLesson type { get; set; }

    public List<StudentMissing> StudentsMissing = new();

    public enum TypeLesson {
        Lecture,
        Laboratory, 
        Practice, 
        Curatorial, 
        PhysicalEducation
    }

    public static TypeLesson isTypeLesson(string type) {
        type = type.ToUpper();
        switch (type)
        {
            case "ЛЗ":
                return TypeLesson.Lecture;
            case "ЛЕК":
                return TypeLesson.Lecture;
            case "ЛБЗ":
                return TypeLesson.Laboratory;
            case "ЛАБ":
                return TypeLesson.Laboratory;
            case "ПЗ":
                return TypeLesson.Practice;
            case "ПРАК":
                return TypeLesson.Practice;
            case "КЗ":
                return TypeLesson.Curatorial;
            case "ИНФОРМАЦИОННЫЙ ЧАС":
                return TypeLesson.Curatorial;
            case "КУРАТОРСКИЙ ЧАС":
                return TypeLesson.Curatorial;
            case "ФИЗИЧЕСКАЯ КУЛЬТУРА":
                return TypeLesson.PhysicalEducation;
            case "ФЗ":
                return TypeLesson.PhysicalEducation;
            default:
                return TypeLesson.Lecture;
        }
    }

    public static string ConvertToString(TypeLesson type) {
        switch (type)
        {
            case TypeLesson.Lecture:
                return "ЛЗ";
            case TypeLesson.Laboratory:
                return "ЛБЗ";
            case TypeLesson.Practice:
                return "ПЗ";
            case TypeLesson.Curatorial:
                return "КЗ";
            case TypeLesson.PhysicalEducation:
                return "ФЗ";
            default:
                return "ЛЗ";
        }
    }
}
