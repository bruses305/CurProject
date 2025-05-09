using System;
using UnityEngine;

public class AddData : MonoBehaviour
{
    public AddCertificate addCertificate = new();
    public AddStudent addStudent = new();

    private void Awake()
    {
        addCertificate.Awake();
        addStudent.Awake();
    }
    
    public static void SetActiveOpenRedactorButton(bool active)
    {
        AddCertificate.SetActiveOpenRedactorButton(active);
        AddStudent.SetActiveOpenRedactorButton(active);
    }
}
