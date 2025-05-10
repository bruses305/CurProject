using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;

public class FireBaseData
{
    public const string Key_ADMINISTRATION = "Administrations";
    public const string Key_BLOCK = "Blocked";
    public const string Key_DEVICES = "Devices";
    public const string Key_UUID = "UUID";
    public const string Key_CONNECTION = "Connection";
    public const string Key_UPDATEALL = "UpdateAll";
    public const string Key_TEST = "Test";
    public bool IsAdministration { get; set; }

    public List<string> NameGroupAdministration = new();

    public List<Faculty> Faculties = new();



}
