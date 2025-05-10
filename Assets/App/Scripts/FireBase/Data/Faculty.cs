using System.Collections.Generic;
using Firebase.Database;

public class Faculty : IFireBaseData
{
    public const string key = "Faculties";
    public string Key { get => key; }
    public string Name { get; set; }
    public Dictionary<string, Specialization> Specializations = new();
    public DatabaseReference Reference { get; private set; }

    public Faculty(DatabaseReference facultyReference)
    {
        Reference = facultyReference;
    }
}
 