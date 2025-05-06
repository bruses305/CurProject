using System;
using UnityEngine;
using UnityEngine.Serialization;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance;
    [SerializeField]private GameObject loadingScreenObject;

    private void Start()
    {
        Instance = this;
    }

    public void Activate() {
        Instance.loadingScreenObject.SetActive(true);
    }

    public void DiesActivate() {
        Instance.loadingScreenObject.SetActive(false);
    }
}
