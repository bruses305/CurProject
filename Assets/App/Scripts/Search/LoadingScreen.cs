using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField]private GameObject LoadingScreenObject;

    public void Activate() {
        LoadingScreenObject.SetActive(true);
    }

    public void DiesActivate() {
        LoadingScreenObject.SetActive(false);
    }
}
