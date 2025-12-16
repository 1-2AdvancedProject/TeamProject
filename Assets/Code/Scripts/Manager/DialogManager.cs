using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [Header("대화 말풍선")]
    public GameObject talkPanel;
    [Header("대화 텍스트")]
    public TextMeshProUGUI talkText;
    [Header("플레이어가 상호작용하는 오브젝트")]
    public GameObject scanObject;

    // 상태 저장용 변수
    [Header("상호작용 여부 (상태 저장용)")]
    public bool isAction;  // 액션여부 (상호작용 여부)

    public void Action(GameObject scanObj)
    {
        // 액션 중일 경우
        if (isAction)
        {
            isAction = false;
        }
        // 액션 중이지 않을 경우
        else
        {
            // 액션 취하기
            isAction = true;
            scanObject = scanObj;
            talkText.text = "Hello";
        }

        // 액션 여부에 따라 말풍선 상태 변경하기
        talkPanel.SetActive(isAction);
    }
}
