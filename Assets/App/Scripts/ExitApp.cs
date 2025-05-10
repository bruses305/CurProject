using UnityEngine;
using UnityEngine.UI;

    public class ExitApp : MonoBehaviour
    {
        [SerializeField] private Button exitButton;

        private void Awake()
        {
            exitButton.onClick.AddListener(Exit);
        }

        public static void Exit()
        {
            Application.Quit();
        }
    }
