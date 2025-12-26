using UnityEngine;

public class LineRendererAtoB : MonoBehaviour
{
	LineRenderer lineRenderer;

	private void Awake()
	{
		lineRenderer = GetComponent<LineRenderer>();

		lineRenderer.positionCount = 2;     // 그리는 점의 갯수
		lineRenderer.enabled = true;
	}

	public void Play(Vector3 from, Vector3 to)
	{
		lineRenderer.enabled = true;

		lineRenderer.SetPosition(0, from);
		lineRenderer.SetPosition(1, to);
	}

	public void Stop()
	{
		lineRenderer.enabled = false;
	}
}
