using System;
using UnityEngine;
using UnityEngine.Serialization;

public class LoadingScreen : MonoBehaviour
{
    private static LoadingScreen _instance;
    [SerializeField]private GameObject loadingScreenObject;

    private void Start()
    {
        _instance = this;
    }

    public void Activate() {
        _instance.loadingScreenObject.SetActive(true);
    }

    public void DiesActivate() {
        _instance.loadingScreenObject.SetActive(false);
    }
}
