using UnityEngine;

public class GameManager : MonoSingleton<GameManager> // 싱글톤 사용
{
    [Header("Manager 관련 코드")]
    public AudioManager audioManager;

    [Header("Player 관련 코드")]
    public PlayerController playerController;
    public PlayerStats playerStats;


    protected new void Awake()
    {
        QualitySettings.vSyncCount = 0; // VSync 비활성화 (모니터 주사율 영향 제거)

        Application.targetFrameRate = 120; // 프레임 120 제한
        if (Instance != null && Instance != this) // 중복 GameManager 방지
        {
            Destroy(gameObject);
            return;
        }

        base.Awake(); // MonoSingleton의 Awake 호출
    }

    private void Start()
    {
        playerStats.ResetStats();
    }
}
