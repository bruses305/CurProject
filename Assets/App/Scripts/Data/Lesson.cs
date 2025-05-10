using System;
using System.Collections.Generic;

public class Lesson
{
    public DateTime DateLesson;
    public string Name;
    public List<int> IdLostStudents = new();

    public Lesson(string name, DateTime dateLesson) {
        this.Name = name;
        this.DateLesson = dateLesson;
    }
}
