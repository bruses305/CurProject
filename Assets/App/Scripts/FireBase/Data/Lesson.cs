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
            case "À«":
                return TypeLesson.Lecture;
            case "À≈ ":
                return TypeLesson.Lecture;
            case "À¡«":
                return TypeLesson.Laborator;
            case "À¿¡":
                return TypeLesson.Laborator;
            case "œ«":
                return TypeLesson.Practice;
            case "œ–¿ ":
                return TypeLesson.Practice;
            case " «":
                return TypeLesson.Curatorial;
            case "»Õ‘Œ–Ã¿÷»ŒÕÕ€… ◊¿—":
                return TypeLesson.Curatorial;
            case " ”–¿“Œ–— »… ◊¿—":
                return TypeLesson.Curatorial;
            case "‘»«»◊≈— ¿ﬂ  ”À‹“”–¿":
                return TypeLesson.PhysicalEducation;
            case "‘«":
                return TypeLesson.PhysicalEducation;
            default:
                return TypeLesson.Lecture;
        }
    }

    public static string ConvertToString(TypeLesson type) {
        switch (type)
        {
            case TypeLesson.Lecture:
                return "À«";
            case TypeLesson.Laborator:
                return "À¡«";
            case TypeLesson.Practice:
                return "œ«";
            case TypeLesson.Curatorial:
                return " «";
            case TypeLesson.PhysicalEducation:
                return "‘«";
            default:
                return "À«";
        }
    }
}
