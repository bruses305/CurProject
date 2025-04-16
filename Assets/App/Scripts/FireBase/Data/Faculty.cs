using System.Collections.Generic;

public class Faculty : IFireBaseData
{
    public const string key = "Faculties";
    public string Key { get => key; }
    public const string Key_NAME = "Name";
    public string Name { get; set; }
    public Dictionary<string, Specialization> Specializations = new();

}
 