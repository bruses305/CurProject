using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
[Serializable]
public class AddCertificate
{
    private static AddCertificate _instance;
    [SerializeField] private GameObject creatorCertificate;
    [SerializeField] private Button openCertificateRedactor;
    
    private TMP_InputField _studentName;
    private TMP_InputField _dateStart;
    private TMP_InputField _dateEnd;
    private Button _createCertificate;

    private const string Key_PARENT_CREATOR_OBJECT_NAME = "Certificate";
    private const string Key_PARENT_DATA_NAME = "Data";
    private const string Key_BUTTON_CREATE_OBJECT_NAME = "Create";
    private const string Key_TMP_STUDENT = "StudentName";
    private const string Key_TMP_DATE_START = "DateStart";
    private const string Key_TMP_DATE_END = "DateEnd";
    
    private string GroupName => FormingTabelDate.LastGroupParsing.Name;
    private List<Student> Students => FormingTabelDate.LastGroup.Students;
    private bool _onStartEditing;

    public void Awake()
    {
        InitializeLoadObjects();
        _instance = this;
        _studentName.onSubmit.AddListener((_) => StartInputStudentName());
        _studentName.onEndEdit.AddListener((_) => EndInputStudentName());
        
        _dateStart.onSubmit.AddListener((_) => StartInputDate(_dateStart));
        _dateStart.onEndEdit.AddListener((_) =>   EndInputDate(_dateStart));
        
        _dateEnd.onSelect.AddListener((_) => StartInputDate(_dateEnd));
        _dateEnd.onDeselect.AddListener((_) =>   EndInputDate(_dateEnd));
        
        openCertificateRedactor.onClick.AddListener(ActivateCreateCertificate);
        _createCertificate.onClick.AddListener(CreateCertificate);
        
        creatorCertificate.SetActive(false);
        SetActiveOpenRedactorButton(false);
        
    }

    public static void SetActiveOpenRedactorButton(bool active)
    {
        _instance.openCertificateRedactor.gameObject.SetActive(active);
    }


    private async void CreateCertificate()
    {
        bool isDate = DateTime.TryParse(_dateStart.text, out DateTime dateStartDate);
        isDate = DateTime.TryParse(_dateEnd.text, out DateTime dateEndDate) && isDate;
        if (dateStartDate > dateEndDate)
        {
            _dateEnd.textComponent.color = Color.red;
            return;
        }
        if (_onStartEditing && isDate)
        {
            ActivateCreateCertificate();
            
            await FireBase.CreateCertificate(GroupName, Students.FindIndex(obj => obj.Name == _studentName.text),
                dateStartDate.ToString("dd-MM-yyyy"),
                dateEndDate.ToString("dd-MM-yyyy"));
            
            ClearAllInputField();
        }
    }
    private void ActivateCreateCertificate()
    {
        creatorCertificate.SetActive(!creatorCertificate.activeInHierarchy);
    }
    private void StartInputDate(TMP_InputField inputField)
    {
        inputField.textComponent.color = Color.white;
    }
    private void EndInputDate(TMP_InputField inputField)
    {
        if (!DateTime.TryParse(inputField.text, out _))
        {
            inputField.textComponent.color = Color.red;
        }
    }
    private void StartInputStudentName()
    {
        _studentName.textComponent.color = Color.white;
        _onStartEditing = true;
    }
    private void EndInputStudentName()
    {
        if (!Students.Exists(obj => obj.Name == _studentName.text))
        {
            _onStartEditing=false;
            _studentName.textComponent.color = Color.red;
        }
    }
    private void ClearAllInputField()
    {
        _studentName.text = "";
        _dateStart.text = "";
        _dateEnd.text = "";
    }

    private void InitializeLoadObjects()
    {
        foreach (Transform parentCreatorObject in creatorCertificate.transform)
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
                            if (dataTMP.name == Key_TMP_STUDENT)
                            {
                                _studentName = dataTMP.GetChild(1).gameObject.GetComponent<TMP_InputField>();
                            }
                            else if (dataTMP.name == Key_TMP_DATE_START)
                            {
                                _dateStart = dataTMP.GetChild(1).gameObject.GetComponent<TMP_InputField>();
                            }
                            else if(dataTMP.name == Key_TMP_DATE_END)
                            {
                                _dateEnd = dataTMP.GetChild(1).gameObject.GetComponent<TMP_InputField>();
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
                        _createCertificate = creatorObject.gameObject.GetComponent<Button>();
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
