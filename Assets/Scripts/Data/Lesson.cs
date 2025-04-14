using System;
using System.Collections.Generic;

public class Lesson
{
    public DateTime dateLesson;
    public string Name;
    public List<int> IdLostStudents = new();

    public Lesson(string Name, DateTime dateLesson) {
        this.Name = Name;
        this.dateLesson = dateLesson;
    }
}
