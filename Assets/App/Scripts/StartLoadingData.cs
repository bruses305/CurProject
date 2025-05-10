using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StartLoadingData : MonoBehaviour
{
    [SerializeField] private Button UUIDTextCopy;
    [SerializeField] private TextMeshProUGUI NotificationText;
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
        
        NotificationText.gameObject.SetActive(false);
    }

    private void Start() {
        UUIDTextCopy.GetComponent<Button>().onClick.AddListener(CopyUUID);
    }

    private void CopyUUID()
    {
        GUIUtility.systemCopyBuffer = UUID;
        Notification.SendNotificationMessage("Скопированно");
    }

    
}
