using System;
using UnityEngine;

/// <summary>
/// 한 판(run) 전체를 관리한다.
/// - 현재 점수를 보관하고 변경을 알린다.
/// - 플레이어 사망 시 게임오버 처리: 장애물 생성 정지, 점수 기록, GameOver 이벤트 발생.
/// UI(점수·게이지·결과창)는 UIManager 가 이벤트를 구독해 담당한다.
/// 씬마다 하나만 존재하는 싱글톤. GameManager.Instance 로 접근한다.
/// </summary>
public sealed class GameManager : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private ObstacleSpawner spawner;

    public static GameManager Instance { get; private set; }

    public int CurrentScore { get; private set; }
    public bool IsGameOver { get; private set; }

    /// <summary>점수가 바뀔 때마다 새 점수와 함께 호출된다. (HUD 갱신용)</summary>
    public event Action<int> ScoreChanged;

    /// <summary>게임오버 진입 순간 한 번 호출된다.</summary>
    public event Action GameOver;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void AddPoint()
    {
        if (IsGameOver)
        {
            return;
        }

        CurrentScore += 1;
        ScoreChanged?.Invoke(CurrentScore);
    }

    public void ResetScore()
    {
        CurrentScore = 0;
        ScoreChanged?.Invoke(CurrentScore);
    }

    /// <summary>
    /// 플레이어가 죽었을 때 호출한다. (PlayerController 가 부딪힘을 감지해 호출)
    /// 중복 호출은 무시한다.
    /// </summary>
    public void HandlePlayerDeath()
    {
        if (IsGameOver)
        {
            return;
        }

        IsGameOver = true;

        if (spawner != null)
        {
            spawner.enabled = false;
        }

        ScoreManager.Instance.AddScore(CurrentScore);

        GameOver?.Invoke();
    }
}
