using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    private float _lastProgress = 0;
    private float _progress;
    public static float Progress
    {
        get => _instance._progress;
        set
        {
            _instance._progress  = value;
            SetProgress();
        }
    }
    private static ProgressBar _instance;
    [SerializeField] private GameObject progressGameObject;
    [SerializeField] private Image progressBarImage;
    [SerializeField] private TextMeshProUGUI progressProcentText;

    private void Awake()
    {
        _instance = this;
        progressGameObject.SetActive(false);
    }

    public static void ErrorProgress(string message)
    {
        Progress = 1;
        Notification.SendNotificationMessage(message, Color.red,1f);
    }

    private static void SetProgress()
    {
        if (Progress < 1)
        {
            _instance.progressGameObject.SetActive(true);
            _instance.StartCoroutine(_instance.CallBack());
        }
        else
        {
            _instance.StartCoroutine(_instance.CallBack());
            _instance._lastProgress = 0;
            _instance.progressGameObject.SetActive(false);
        }
    }
    
    private IEnumerator CallBack() {
        while (true)
        {
            if (Progress >= 1)
            {
                _instance.progressBarImage.fillAmount = Progress;
                _instance.progressProcentText.text = $"{Progress * 100:0.00}%"; 
                yield return new WaitForSeconds(0.01f);
                break;
            }
            float speed = (Progress - _lastProgress)*20;
            speed = speed > 1 ? speed : 1;
            _instance._lastProgress += 0.002f * speed;

            _instance.progressBarImage.fillAmount = _lastProgress;
            _instance.progressProcentText.text = $"{_lastProgress * 100:0.00}%"; 
            
            if(_lastProgress >= Progress) break;
            yield return new WaitForSeconds(0f);
        }
        
    }
}
