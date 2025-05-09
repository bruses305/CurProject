using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
[Serializable]
public class AddStudent
{
    private static AddStudent _instance;
    [SerializeField] private GameObject studentCreator;
    [SerializeField] private Button openStudentCreator;
    private Button createStudent;
    private TMP_InputField studentName;
    private Toggle typeStudent;
    
    private const string Key_PARENT_CREATOR_OBJECT_NAME = "Student";
    private const string Key_PARENT_DATA_NAME = "Data";
    private const string Key_BUTTON_CREATE_OBJECT_NAME = "Create";
    private const string Key_TMP_STUDENT = "StudentName";
    private const string Key_TMP_TOGLE = "Toggle Free";
    private string GroupName => FormingTabelDate.LastGroupParsing.Name;

    public void Awake()
    {
        InitializeLoadObjects();
        _instance = this;
        openStudentCreator.onClick.AddListener(SwitcherStateStudentCreator);
        createStudent.onClick.AddListener(CreateStudent);
        
        studentCreator.gameObject.SetActive(false);
        SetActiveOpenRedactorButton(false);
    }

    public static void SetActiveOpenRedactorButton(bool active)
    {
        _instance.openStudentCreator.gameObject.SetActive(active);
    }

    private async void CreateStudent()
    {
        if (studentName.text != null)
        {
            Debug.Log("CreateStudent" + studentName.text);
            await FireBase.CreateStudent(GroupName, studentName.text, typeStudent.isOn);
            SwitcherStateStudentCreator();
        }
    }

    private void SwitcherStateStudentCreator()
    {
        studentCreator.SetActive(!studentCreator.activeInHierarchy);
    }
    private void InitializeLoadObjects()
    {
        foreach (Transform parentCreatorObject in studentCreator.transform)
        {
            if (parentCreatorObject.name == Key_PARENT_CREATOR_OBJECT_NAME)
            {
                bool isLeave = false;
                foreach (Transform creatorObject in parentCreatorObject)
                {
                    if (creatorObject.name == Key_PARENT_DATA_NAME)
                    {
                        foreach (Transform dataTMP in creatorObject)
                        {
                            if (dataTMP.name == Key_TMP_TOGLE)
                            {
                                typeStudent = dataTMP.GetChild(1).gameObject.GetComponent<Toggle>();
                            }
                            else if (dataTMP.name == Key_TMP_STUDENT)
                            {
                                studentName = dataTMP.GetChild(1).gameObject.GetComponent<TMP_InputField>();
                            }
                        }
                        if (isLeave)
                        {
                            return;
                        }
                        isLeave = true;
                    }
                    if (creatorObject.name == Key_BUTTON_CREATE_OBJECT_NAME)
                    {
                        createStudent = creatorObject.gameObject.GetComponent<Button>();
                        if (isLeave)
                        {
                            return;
                        }
                        isLeave = true;
                    }
                }
            }
        }
        
        Debug.LogError("FailLoadObject");
    }
}