using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Notification : MonoBehaviour
{
    private static Notification _instance;
    [SerializeField] private TextMeshProUGUI notificationText;
    private static TextMeshProUGUI NotificationText => _instance.notificationText;

    private void Awake()
    {
        _instance = this;
        NotificationText.gameObject.SetActive(false);
    }

    public static void SendNotificationMessage(string message, Color? color = null, float time = 0.5f)
    {
        _instance.StartCoroutine(NotificationMessage(message, color, time));
    }

    private static IEnumerator NotificationMessage(string message,Color? color, float time)
    {
        Color colorMessage = color ?? Color.white;
        NotificationText.text = "";
        NotificationText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0f);
        NotificationText.color = colorMessage;
        NotificationText.text = message;
        yield return new WaitForSeconds(time);
        NotificationText.gameObject.SetActive(false);
    }
}
