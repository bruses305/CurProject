using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class RedactSelectedCells : MonoBehaviour
{
    private static RedactSelectedCells _instance;
    [SerializeField] private Button buttonConfirm;
    [SerializeField] private Button buttonCansel;
    private static GameObject ButtonConfirmObject => _instance.buttonConfirm.gameObject;
    private static GameObject ButtonCanselObject => _instance.buttonCansel.gameObject;

    private void Awake()
    {
        _instance = this;
        buttonConfirm.onClick.AddListener(() => _ = RedactingMissingStudent());
        buttonCansel.onClick.AddListener(CanselRedactingMissingStudent);
        SetActiveButton(false);
    }

    private async Task RedactingMissingStudent()
    {
        await FormingTabelDate.MergingMissingStudent();
        SetActiveButton(false);
    }
    
    private void CanselRedactingMissingStudent()
    {
        FormingTabelDate.CanselMissingStudent();
        SetActiveButton(false);
    }

    public static void SetActiveButton(bool isActive)
    {
        ButtonConfirmObject.SetActive(isActive);
        ButtonCanselObject.SetActive(isActive);
    }
}
