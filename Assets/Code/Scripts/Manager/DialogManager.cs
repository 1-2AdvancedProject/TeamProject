using TMPro;
using UnityEngine;
using System.Collections;

public class DialogManager : MonoBehaviour
{
    [Header("대화 말풍선")]
    public GameObject talkPanel;
    [Header("대화 텍스트( | 넣으면 숨 고름)")]
    public TextMeshProUGUI talkText;

    [TextArea]
    public string dialogText;

    public float typingSpeed = 0.05f;
    bool isTyping;

    [HideInInspector]
    public bool isAction;

    Coroutine typingCoroutine;

    public void Action()
    {
        isAction = !isAction;

        if (isAction)
        {
            talkPanel.SetActive(true);

            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            typingCoroutine = StartCoroutine(TypeText());
        }
        else
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            talkPanel.SetActive(false);
        }
    }

    IEnumerator TypeText()
    {
        isTyping = true;
        talkText.text = "";

        int visibleCharCount = 0;

        foreach (char c in dialogText)
        {
            // | 하나당 즉시 0.1초 대기
            if (c == '|')
            {
                yield return new WaitForSeconds(0.1f);
                continue;
            }

            talkText.text += c;
            visibleCharCount++;

            if (visibleCharCount % 1 == 0)
            {
                GameManager.Instance.audioManager.TextTypingSound(1f);
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

}
