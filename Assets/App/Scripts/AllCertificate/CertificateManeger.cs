using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CertificateManeger : MonoBehaviour
{
    [SerializeField] private GameObject certificateParent;
    [SerializeField] private Button exit;
    [SerializeField] private GameObject prefabCertificate;
    
    
    private GroupParsing Group => FormingTabelDate.LastGroupParsing;
    private List<Student> Students => FormingTabelDate.LastGroup.Students;

    private void Awake()
    {
        exit.onClick.AddListener(gameObject.GetComponent<OpenAllCertificate>().SwitchActiveCertificateRedactor);
    }

    public void CreatorAllCertificate()
    {
        TableObjectData.DestoroyChildObject(certificateParent);
        
        List<StudentJustificationDocument> justs = ConverterDataToReports.StudentJustificationDocument();
        foreach (var studentJustificationDocument in justs)
        {
            GameObject certificate = Instantiate(prefabCertificate, certificateParent.transform);
            OutDataCertificate(certificate.transform,out TextMeshProUGUI studentName,out TextMeshProUGUI dateStart,out TextMeshProUGUI dateEnd, out Button delete);

            studentName.text = studentJustificationDocument.Name;
            dateStart.text = studentJustificationDocument.startJust.ToString("dd-MM-yyyy");
            dateEnd.text = studentJustificationDocument.endJust.ToString("dd-MM-yyyy");
            var document = studentJustificationDocument;
            int idStudent = Students.FindIndex(obj => obj.Name == document.Name);
            delete.onClick.AddListener(()=>
                _ = FireBase.DeleteCertificate(Group.Reference, idStudent,dateEnd.text));
            delete.onClick.AddListener(()=>DeleteCertificate(certificate,idStudent,dateEnd.text));
        }
        
    }
    
    private void DeleteCertificate(GameObject certificate,int idStudent,string dateEnd)
    {
        Students[idStudent].Certificates.Remove(dateEnd);
        certificate.transform.SetParent(null);
        Destroy(certificate);
    }

    private void OutDataCertificate(Transform certificate, out TextMeshProUGUI studentName,out TextMeshProUGUI dateStart,out TextMeshProUGUI dateEnd, out Button delete)
    {
        studentName = certificate.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        dateStart = certificate.transform.GetChild(2).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        dateEnd = certificate.transform.GetChild(2).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>();
        delete = certificate.transform.GetChild(3).gameObject.GetComponent<Button>();
    }
}
