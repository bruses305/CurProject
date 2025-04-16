using UnityEngine;
using TMPro;

public class StartLoadingData : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI UUIDText;
    private const string UUID_PLayerPrefs = "UUID";
    public static string UUID;
    private void Awake() {
        if (!PlayerPrefs.HasKey(UUID_PLayerPrefs)){
            UUID = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString(UUID_PLayerPrefs, UUID);
        }
        else {
            UUID = PlayerPrefs.GetString(UUID_PLayerPrefs);
        }
    }

    private void Start() {
        UUIDText.text = "UUID: " + UUID;
    }
}
