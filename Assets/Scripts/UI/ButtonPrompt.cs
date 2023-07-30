using UnityEngine;

namespace UI
{
    public class ButtonPrompt : MonoBehaviour
    {
        public float buttonPromptHoverSpeed;
        public float buttonPromptHoverDistance;

        // Update is called once per frame
        private void Update()
        {
            // Make the button prompt float up and down
            Vector3 position = transform.position;
            position.y += Mathf.Sin(Time.time * buttonPromptHoverSpeed) * buttonPromptHoverDistance;
            transform.position = position;
        }
        
        public void SetPromptText(string prompt)
        {
            GetComponentInChildren<TMPro.TextMeshProUGUI>().text = prompt;
        }
    }
}
