using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddStudent : MonoBehaviour
{
    private static AddStudent _instance;
    [SerializeField] private GameObject studentCreator;
    [SerializeField] private TMP_InputField studentName;
    [SerializeField] private Toggle typeStudent;
    [SerializeField] private Button openStudentCreator;
    [SerializeField] private Button createStudent;

    private string GroupName => FormingTabelDate.LastGroupParsing.Name;

    private void Awake()
    {
        _instance = this;
        openStudentCreator.onClick.AddListener(SwitcherStateStudentCreator);
        createStudent.onClick.AddListener(CreateStudent);
    }

    public static void SetActiveOpenRedactorButton(bool active)
    {
        _instance.openStudentCreator.gameObject.SetActive(active);
    }

    private async void CreateStudent()
    {
        if (studentName.text != null)
        {
            await FireBase.CreateStudent(GroupName, studentName.text, typeStudent.isOn);
        }
    }

    private void SwitcherStateStudentCreator()
    {
        studentCreator.SetActive(!studentCreator.activeInHierarchy);
    }
}