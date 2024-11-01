using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class LevelEndPopup : MonoBehaviour
    {
        [SerializeField] private Button levelEndButton;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text buttonText;

        private void Awake()
        {
            levelEndButton.onClick.AddListener(ButtonClicked);
        }

        public void ShowPopup(bool isSuccess)
        {
            gameObject.SetActive(true);
            titleText.text = isSuccess ? "Excellent!" : "Failed!";
            descriptionText.text = isSuccess ? "YOU\nWON!" : "YOU\nLOST!";
            buttonText.text = isSuccess ? "NEXT" : "RETRY";
        }

        private void ButtonClicked()
        {
            SceneManager.LoadScene(0);
        }

        private void OnDestroy()
        {
            levelEndButton.onClick.RemoveAllListeners();
        }
    }
}
