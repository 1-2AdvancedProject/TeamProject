using UnityEngine;

public class Hooking : MonoBehaviour
{
    GrapplingHook grappling;
    public DistanceJoint2D joint2D;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        grappling = GameObject.Find("Player").GetComponent<GrapplingHook>();
        joint2D = GetComponent<DistanceJoint2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ceiling"))
        {
            joint2D.enabled = true;
            grappling.isAttach = true;
        }
    }
}
