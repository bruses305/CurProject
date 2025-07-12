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

    public static List<StudentJustificationDocument> StudentJustificationDocument()
    {
        List<StudentJustificationDocument> documents = new();
        
        foreach (var student in LastGroup.Students)
        {
            foreach (var certificate in FireBase.FindCertificate(student.Certificates,Times.StartDayInMonth(SelectedDates.DateStart),false))
            {
                StudentJustificationDocument document = new()
                {
                    Name = student.Name,
                    typeJust = global::StudentJustificationDocument.TypeJust.Справка,
                    startJust = DateTime.Parse(certificate.Value),
                    endJust = DateTime.Parse(certificate.Key)
                };
                
                documents.Add(document);
            }
            
            
        }
        
        return documents;
    }

    public static List<StudentMakeupEntry> StudentMakeupEntry()
    {
        List<StudentMakeupEntry> makeups = new();

        
        foreach (var missingStudent in MissingStudent)
        {
            if(!missingStudent.Value) continue;
            
            FormingTabelDate.FindDateAndLessonAndStudent(missingStudent.Key, out string date, out int lessonID,out int studentID);
            
            if (LastGroup.Dates[date].lessons[lessonID].StudentsMissing.Find(obj=>obj.ID == studentID).Type) continue;
            
            if (LastGroup.Dates[date].lessons[lessonID].type is not (LessonFireBase.TypeLesson.Practice
                or LessonFireBase.TypeLesson.PhysicalEducation or LessonFireBase.TypeLesson.Laboratory)) continue;
            
            string lessonName = LastGroup.Dates[date].lessons[lessonID].Name;
            int makeupID = makeups.FindIndex(obj => obj.StudentID == studentID && obj.Subject == lessonName);
            
            if (makeupID>=0)
            {
                makeups[makeupID].Hours += 2;
            }
            else
            {
                StudentMakeupEntry makeup = new()
                {
                    StudentID = studentID,
                    FullName = LastGroup.Students.Find(obj => obj.ID == studentID).Name,
                    Subject = lessonName,
                    Hours = 2
                };
                
                makeups.Add(makeup);
            }
        }
        
        return makeups;
    }
}
