using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IDamageable
{
    Rigidbody2D rb;
    SpriteRenderer sprite;
    GrapplingHook grappling;

    Vector2 input;

    public float moveForce = 30f;
    public float maxSpeed = 18f;
    public float swingForce = 25f;
    public float ropeAdjustSpeed = 6f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        grappling = GetComponent<GrapplingHook>();
    }

    void FixedUpdate()
    {
        if (grappling.isAttach)
        {
            ApplyGrappleMovement();
        }
        else
        {
            ApplyGroundMovement();
        }

        ClampVelocity();
        Flip();
    }

    void ApplyGroundMovement()
    {
        rb.AddForce(new Vector2(input.x * moveForce, 0f), ForceMode2D.Force);
    }

    void ApplyGrappleMovement()
    {
        // 훅 → 플레이어 방향
        Vector2 ropeDir = (rb.position - (Vector2)grappling.hook.position).normalized;

        // 접선 방향 (스윙 핵심)
        Vector2 tangent = new Vector2(-ropeDir.y, ropeDir.x);

        rb.AddForce(tangent * input.x * swingForce, ForceMode2D.Force);

    }

    void ClampVelocity()
    {
        rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxSpeed);
    }

    void Flip()
    {
        if (input.x > 0) sprite.flipX = false;
        else if (input.x < 0) sprite.flipX = true;
    }

    void OnMove(InputValue value)
    {
        input = value.Get<Vector2>();
    }

    void OnJump()
    {
        if (!grappling.isAttach)
        {
            rb.AddForce(Vector2.up * 12f, ForceMode2D.Impulse);
        }
    }

    public void TakeDamage(int attack)
    {
        GameManager.Instance.playerStatsRuntime.currentHP -= attack;
    }
}
