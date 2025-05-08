using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AddCertificate : MonoBehaviour
{
    private static AddCertificate _instance;
    [SerializeField] private GameObject creatorCertificate;
    [SerializeField] private TMP_InputField studentName;
    [SerializeField] private TMP_InputField dateStart;
    [SerializeField] private TMP_InputField dateEnd;
    [SerializeField] private Button openCertificateRedactor;
    [SerializeField] private Button createCertificate;
    
    private string GroupName => FormingTabelDate.LastGroupParsing.Name;
    private List<Student> Students => FormingTabelDate.LastGroup.Students;
    private bool _onStartEditing;

    private void Awake()
    {
        _instance = this;
        
        studentName.onSubmit.AddListener((_) => StartInputStudentName());
        studentName.onEndEdit.AddListener((_) => EndInputStudentName());
        
        dateStart.onSubmit.AddListener((_) => StartInputDate(dateStart));
        dateStart.onEndEdit.AddListener((_) =>   EndInputDate(dateStart));
        
        dateEnd.onSelect.AddListener((_) => StartInputDate(dateEnd));
        dateEnd.onDeselect.AddListener((_) =>   EndInputDate(dateEnd));
        
        openCertificateRedactor.onClick.AddListener(ActivateCreateCertificate);
        createCertificate.onClick.AddListener(CreateCertificate);
        
        creatorCertificate.SetActive(false);
        openCertificateRedactor.gameObject.SetActive(false);
        
    }

    public static void SetActiveOpenRedactorButton(bool active)
    {
        _instance.openCertificateRedactor.gameObject.SetActive(active);
    }


    private async void CreateCertificate()
    {
        bool isDate = DateTime.TryParse(dateStart.text, out DateTime dateStartDate);
        isDate = DateTime.TryParse(dateEnd.text, out DateTime dateEndDate) && isDate;
        if (dateStartDate > dateEndDate)
        {
            dateEnd.textComponent.color = Color.red;
            return;
        }
        if (_onStartEditing && isDate)
        {
            ActivateCreateCertificate();
            
            await FireBase.CreateCertificate(GroupName, Students.FindIndex(obj => obj.Name == studentName.text),
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
        studentName.textComponent.color = Color.white;
        _onStartEditing = true;
    }
    private void EndInputStudentName()
    {
        if (!Students.Exists(obj => obj.Name == studentName.text))
        {
            _onStartEditing=false;
            studentName.textComponent.color = Color.red;
        }
    }
    private void ClearAllInputField()
    {
        studentName.text = "";
        dateStart.text = "";
        dateEnd.text = "";
    }
    
    
}
