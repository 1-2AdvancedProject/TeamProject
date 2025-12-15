using UnityEngine;
using UnityEngine.InputSystem;

public class GrapplingHook : MonoBehaviour
{
    public LineRenderer line;
    public Transform hook;
    private Vector2 mousedir;

    public bool isHookActive;
    public bool isLineMax;
    public bool isAttach;

    void Start()
    {
        // 라인을 그리는 포지션을 두개로 설정하고 (PositionCount)
        // 한 점은 Player의 포지션, 한 점은 Hook의 포지션으로 설정 (SetPosition)
        line.positionCount = 2;
        line.endWidth = line.startWidth = 0.05f;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, hook.position);
        line.useWorldSpace = true;
        isAttach = false;
        hook.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        line.SetPosition(0, transform.position);
        line.SetPosition(1, hook.position);

        if (Mouse.current.leftButton.wasPressedThisFrame && !isHookActive)
        {
            hook.position = transform.position;
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mouseWorldPos.z = 0f;

            mousedir = mouseWorldPos - transform.position;
            isHookActive = true;
            hook.gameObject.SetActive(true);
        }

        // 훅이 발사된 상태이고, 아직 최대 사거리에 도달하지 않았을 때
        if (isHookActive && !isLineMax && !isAttach)
        {
            // 마우스 방향으로 훅을 전진시킴
            hook.Translate(mousedir.normalized * Time.deltaTime * GameManager.Instance.playerStatsRuntime.hookSpeed);
            // 플레이어와 훅 사이의 거리가 최대 사거리보다 커지면
            if (Vector2.Distance(transform.position, hook.position) > GameManager.Instance.playerStatsRuntime.hookDistance)
            {
                // 최대 사거리 도달 상태로 전환
                isLineMax = true;
            }
        }

        // 훅이 최대 사거리에 도달한 이후
        else if (isHookActive && isLineMax && !isAttach)
        {
            // 훅을 플레이어 위치로 부드럽게 되돌림
            hook.position = Vector2.MoveTowards(hook.position, transform.position, Time.deltaTime * GameManager.Instance.playerStatsRuntime.hookSpeed);

            // 훅이 거의 플레이어 위치까지 돌아왔을 경우
            if (Vector2.Distance(transform.position, hook.position) < 0.1f)
            {
                // 훅 상태 초기화
                isHookActive = false;
                isLineMax = false;
                // 훅 오브젝트 비활성화
                hook.gameObject.SetActive(false);
            }
        }

        else if (isAttach)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                isAttach = false;
                isHookActive = false;
                isLineMax = false;
                hook.GetComponent<Hooking>().joint2D.enabled = false;
                hook.gameObject.SetActive(false);
            } 
        }
    }
}
