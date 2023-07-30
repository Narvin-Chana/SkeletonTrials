using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
        public GameObject interactHintPanelPrefab;
        public GameObject heartPrefab;

        public Image attackIcon;
        public Image spellIcon;
        public Image dashIcon;
        public Transform healthParent;
        private List<Image> hearts = new();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this);
            }
        }

        private void Start()
        {
            UpdateHearts();
        }

        public void UpdateHearts()
        {
            // Delete current hearts

            foreach (Image heart in hearts)
            {
                Destroy(heart.gameObject);
            }

            hearts.Clear();

            for (int i = 0; i < TopDownCharacterController.Instance.PlayerStats.MaxHealth; i++)
            {
                hearts.Add(Instantiate(heartPrefab, transform).GetComponent<Image>());
                hearts[i].transform.SetParent(healthParent);
                hearts[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(50 * i + 5 * (i + 1), -5);
                if (i > TopDownCharacterController.Instance.PlayerStats.Health)
                {
                    hearts[i].color = Color.black;
                }
            }
        }

        public ButtonPrompt ShowPromptHint(Transform target)
        {
            GameObject buttonPrompt = Instantiate(interactHintPanelPrefab, target.position + Vector3.up * 3.25f,
                Quaternion.identity);
            buttonPrompt.transform.SetParent(target);

            return buttonPrompt.GetComponent<ButtonPrompt>();
        }

        public void HidePromptHint(GameObject buttonPrompt)
        {
            Destroy(buttonPrompt);
        }

        public void ActivateAttackIcon()
        {
            attackIcon.gameObject.SetActive(true);
        }

        public void ActivateSpellIcon()
        {
            spellIcon.gameObject.SetActive(true);
        }

        public void ActivateDashIcon()
        {
            dashIcon.gameObject.SetActive(true);
        }
    }
}