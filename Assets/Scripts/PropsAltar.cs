using System.Collections.Generic;
using UI;
using UnityEngine;

//when something get into the alta, make the runes glow
public class PropsAltar : MonoBehaviour
{
    public List<SpriteRenderer> runes;
    public float lerpSpeed;

    private Color curColor;
    private Color targetColor;

    private ButtonPrompt buttonPrompt;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || !GameManager.Instance.ableToGoToNextLevel) return;

        targetColor = new Color(1, 1, 1, 1);
        buttonPrompt = UIManager.Instance.ShowPromptHint(transform);
        buttonPrompt.SetPromptText("Start Level");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") == false) return;

        targetColor = new Color(1, 1, 1, 0);
        
        if (!buttonPrompt) return;
        
        UIManager.Instance.HidePromptHint(buttonPrompt.gameObject);
        buttonPrompt = null;
    }

    private void Update()
    {
        curColor = Color.Lerp(curColor, targetColor, lerpSpeed * Time.deltaTime);

        foreach (SpriteRenderer r in runes)
        {
            r.color = curColor;
        }

        if (buttonPrompt == null) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            UIManager.Instance.HidePromptHint(buttonPrompt.gameObject);
            GameManager.Instance.StartLevel();
            targetColor = new Color(1, 0, 0, 0);
        }
    }
}