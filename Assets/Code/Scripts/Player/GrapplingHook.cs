using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
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
    public bool isEnemyAttach;

    bool hasShakedOnAttach;
    bool hasPlayedAttachSound;
    bool isPlayedDraftSound;

    [Header("Swing")]
    public float swingForce = 28f;
    public float climbForce = 14f;
    public float maxSwingSpeed = 22f;

    // 슬로우
    public float slowFactor;
    public float slowLength;
    Coroutine slowCoroutine;

    public Vector3 enemyFollowOffset = Vector3.zero;
    private List<Transform> enemies = new List<Transform>();
    private Transform attachedEnemy; // 현재 끌고 있는 적

    Rigidbody2D rb;
    SpriteRenderer sprite;
    DistanceJoint2D hookJoint;

    Vector2 inputVec;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        hookJoint = hook.GetComponent<DistanceJoint2D>();

        hookJoint.enabled = false;
        hookJoint.autoConfigureDistance = false;

        line.positionCount = 2;
        line.endWidth = line.startWidth = 0.05f;
        line.useWorldSpace = true;

        hook.gameObject.SetActive(false);
    }

    void Update()
    {
        line.SetPosition(0, transform.position);
        line.SetPosition(1, hook.position);

        // 처음 붙을 때
        if ((isAttach || isEnemyAttach) && !hasPlayedAttachSound)
        {
            GameManager.Instance.audioManager.HookAttachSound(1f);
            CancelSlow();
            hasPlayedAttachSound = true;
        }

        // 훅 발사
        if (Mouse.current.leftButton.wasPressedThisFrame && !isHookActive && !isAttach && !isEnemyAttach)
        {
            GameManager.Instance.audioManager.HookShootSound(0.7f);

            hook.position = transform.position;
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mouseWorld.z = 0f;

            mousedir = (mouseWorld - transform.position).normalized;
            isHookActive = true;
            hook.gameObject.SetActive(true);
        }

        // 훅 전진
        if (isHookActive && !isLineMax && !isAttach && !isEnemyAttach)
        {
            hook.Translate(mousedir * GameManager.Instance.playerStatsRuntime.hookSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, hook.position) >
                GameManager.Instance.playerStatsRuntime.hookDistance)
            {
                isLineMax = true;
            }
        }

        // 훅 회수
        else if (isHookActive && isLineMax && !isAttach && !isEnemyAttach)
        {
            hook.position = Vector2.MoveTowards(
                hook.position,
                transform.position,
                GameManager.Instance.playerStatsRuntime.hookSpeed * Time.deltaTime
            );

            if (Vector2.Distance(transform.position, hook.position) < 0.1f)
            {
                ResetHook();
            }
        }

        // 그래플 해제
        if (isAttach && Mouse.current.leftButton.wasPressedThisFrame)
        {
            DetachHook();
        }

        // 적 던지기
        if (isEnemyAttach && attachedEnemy != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 throwDir = (mouseWorld - (Vector2)transform.position).normalized;

            ThrowEnemy(attachedEnemy, throwDir, GameManager.Instance.playerStatsRuntime.hookEnemyThrowForce);
        }
    }

    void FixedUpdate()
    {
        if (!isAttach) return;

        inputVec = new Vector2(
            (Keyboard.current.aKey.isPressed ? -1 : 0) + (Keyboard.current.dKey.isPressed ? 1 : 0),
            0
        );

        Vector2 ropeDir = (rb.position - (Vector2)hook.position).normalized;
        Vector2 tangent = new Vector2(-ropeDir.y, ropeDir.x);

        rb.AddForce(tangent * inputVec.x * swingForce, ForceMode2D.Force);

        // 우클릭으로 줄이기
        if (Mouse.current.rightButton.isPressed)
        {
            if (hookJoint.enabled)
            {
                hookJoint.distance = Mathf.Max(1.2f, hookJoint.distance - 4f * Time.fixedDeltaTime);

                if (!isPlayedDraftSound)
                {
                    GameManager.Instance.audioManager.HookDraftSound(1f);
                    isPlayedDraftSound = true;
                }
            }
        }
        else
        {
            if (isPlayedDraftSound)
            {
                GameManager.Instance.audioManager.StopSFX();
                isPlayedDraftSound = false;
            }
        }

        rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxSwingSpeed);
    }

    void DetachHook()
    {
        isAttach = false;
        isHookActive = false;
        isLineMax = false;

        hookJoint.enabled = false;
        hook.gameObject.SetActive(false);

        hasShakedOnAttach = false;
        hasPlayedAttachSound = false;

        if (slowCoroutine != null)
            StopCoroutine(slowCoroutine);

        slowCoroutine = StartCoroutine(SlowRoutine());
    }

    void ResetHook()
    {
        isHookActive = false;
        isLineMax = false;
        hook.gameObject.SetActive(false);
    }

    IEnumerator SlowRoutine()
    {
        sprite.color = Color.red;
        Time.timeScale = slowFactor;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        while (Time.timeScale < 1f)
        {
            Time.timeScale += (1f / slowLength) * Time.unscaledDeltaTime;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
            yield return null;
        }

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        sprite.color = Color.white;
    }

    public void CancelSlow()
    {
        if (slowCoroutine != null)
            StopCoroutine(slowCoroutine);

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        sprite.color = Color.white;
    }

    // === Enemy 관련 ===
    public void AttachEnemy(Transform enemy)
    {
        if (enemies.Contains(enemy)) return;

        enemies.Add(enemy);
        attachedEnemy = enemy;

        // Collider 충돌 무시
        Collider2D enemyCol = enemy.GetComponent<Collider2D>();
        Collider2D playerCol = GetComponent<Collider2D>();
        if (enemyCol != null && playerCol != null)
            Physics2D.IgnoreCollision(enemyCol, playerCol, true);

        // Rigidbody를 Kinematic으로
        Rigidbody2D rbEnemy = enemy.GetComponent<Rigidbody2D>();
        if (rbEnemy != null)
            rbEnemy.bodyType = RigidbodyType2D.Kinematic;

        // 플레이어 자식으로 붙이기
        enemy.SetParent(transform);

        // 플레이어 SpriteRenderer 기준으로 x 좌우 맞춤
        SpriteRenderer playerSprite = GetComponent<SpriteRenderer>();
        Vector3 offset = enemyFollowOffset;
        offset.x = playerSprite.flipX ? -Mathf.Abs(enemyFollowOffset.x) : Mathf.Abs(enemyFollowOffset.x);
        enemy.localPosition = offset;

        // 훅 숨기기
        hook.gameObject.SetActive(false);
        line.enabled = false;

        isEnemyAttach = true;
        isAttach = false;
        isHookActive = false;
        isLineMax = false;
    }

    public void ThrowEnemy(Transform enemy, Vector2 throwDir, float throwForce)
    {
        if (!enemies.Contains(enemy)) return;

        GameManager.Instance.audioManager.HookThrowEnemySound(1f);

        enemies.Remove(enemy);
        attachedEnemy = null;

        // 부모 해제
        enemy.SetParent(null);

        // Rigidbody 처리
        Rigidbody2D rbEnemy = enemy.GetComponent<Rigidbody2D>();
        if (rbEnemy != null)
        {
            rbEnemy.bodyType = RigidbodyType2D.Dynamic;
            rbEnemy.linearVelocity = Vector2.zero;
            rbEnemy.AddForce(throwDir.normalized * throwForce, ForceMode2D.Impulse);
        }

        if (enemies.Count == 0)
            isEnemyAttach = false;

        // 훅 초기화
        line.enabled = true;
        hook.gameObject.SetActive(false);
        if (hookJoint != null) hookJoint.enabled = false;
    }

}
