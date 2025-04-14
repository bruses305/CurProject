using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;

public class FireBaseData
{
    public const string Key_ADMINISTRATION = "Administrations";
    public const string Key_UUID = "UUID";
    public bool IsAdministration { get; set; }

    public List<string> NameGroupAdministration = new();

    public List<Faculty> Faculties = new();



}
