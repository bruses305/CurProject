using UnityEngine;
using UnityEngine.UI;

namespace App.Scripts
{
    public class ExitApp : MonoBehaviour
    {
        [SerializeField] private Button exitButton;

        private void Awake()
        {
            exitButton.onClick.AddListener(Exit);
        }

        private void Exit()
        {
            Application.Quit();
        }
    }
}
