using System;
using System.Collections.Generic;
using UnityEngine;

public static class ConverterDataToReports
{
    //private static GroupParsing GroupParsing => FormingTabelDate.LastGroupParsing;
    private static Group LastGroup => FormingTabelDate.LastGroup;
    private static Dictionary<Vector2Int, bool> MissingStudent => FormingTabelDate.MissingStudent;

    public static List<StudentAttendance> StudentAttendance()
    {
        List<StudentAttendance> students = new();
        
        foreach (var student in LastGroup.Students)
        {
            StudentAttendance studentAttendance = new()
            {
                FullName = student.Name,
                StudyForm = student.Type ? "Б" : "П"
            };
            
            students.Add(studentAttendance);
        }

        foreach (var missingStudent in MissingStudent)
        {
            FormingTabelDate.FindDateAndLessonAndStudent(missingStudent.Key, out string date, out int lessonID,
                out int studentID);
            Dictionary<int,int> dailyHours = students[missingStudent.Key.y].DailyHours;
            
            if(!dailyHours.TryAdd(DateTime.Parse(date).Day, 2)) dailyHours[DateTime.Parse(date).Day] += 2;

            if (LastGroup.Dates[date].lessons[lessonID].StudentsMissing.Find(obj=>obj.ID == studentID).Type) students[missingStudent.Key.y].Valid += 2;
        }

        return students;
    }
}
