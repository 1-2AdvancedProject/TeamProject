using UnityEngine;

public class RainDrop : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            GameManager.Instance.poolManager.ReturnToPool(gameObject);
        }
    }

}
