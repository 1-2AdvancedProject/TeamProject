using UnityEngine;
using UnityEngine.InputSystem;

public class CursorPathMarking : MonoBehaviour
{
	[Header("메인 카메라")]
	public Camera mainCam;
	[Header("라인렌더러")]
	public LineRendererAtoB visualizerLine;

	// 표시선 길이
	float distance = 10f;

	private void Start()
	{
		distance = GameManager.Instance.playerStats.hookDistance;
	}

	void Update()
	{
		// 스크린 좌표 구하기
		if (Mouse.current == null) return;
		Vector3 mouseScreen = Mouse.current.position.ReadValue();
		mouseScreen.z = Mathf.Abs(mainCam.transform.position.z);	// z값 보정

		// 월드 좌표
		Vector2 worldPos = mainCam.ScreenToWorldPoint(mouseScreen);

		// 광선 방향
		Vector2 dir = (worldPos - (Vector2)transform.position).normalized;

		// 레이케스트 플레이어 충돌 무시
		LayerMask mask = ~LayerMask.GetMask("Player");

		// 자기 위치에서 dir 방향으로 광선 발사
		RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, distance, mask);

		// 광선에 부딪히는 오브젝트가 있으면 선 활성화
		if (hit.collider != null)
		{
			visualizerLine.Play(transform.position, hit.point);
		}
		else
		{
			// 안 맞아도 방향 확인용으로 선 그리기
			//visualizerLine.Play(
			//	transform.position,
			//	transform.position + (Vector3)(dir * distance)
			//);
			visualizerLine.Stop();
		}
	}
}

