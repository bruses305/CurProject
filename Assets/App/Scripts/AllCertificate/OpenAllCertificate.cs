using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class OpenAllCertificate : MonoBehaviour
{
    [SerializeField] private Button openButton;
    [SerializeField] private GameObject certificateRedactor;
    [SerializeField] private GameObject allCertificateRedactor;

    private void Awake()
    {
        openButton.onClick.AddListener(SwitchActiveCertificateRedactor);
        openButton.onClick.AddListener(gameObject.GetComponent<CertificateManeger>().CreatorAllCertificate);
    }

    public void SwitchActiveCertificateRedactor()
    {
        allCertificateRedactor.SetActive(!allCertificateRedactor.activeInHierarchy);
        certificateRedactor.SetActive(!certificateRedactor.activeInHierarchy);
    }
}
