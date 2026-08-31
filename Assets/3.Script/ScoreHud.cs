using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 게임 중 현재 점수를 화면에 표시한다. GameManager 의 점수 변경 이벤트만 듣는다.
/// </summary>
public sealed class ScoreHud : MonoBehaviour
{
    [SerializeField] private Text scoreText;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
        Subscribe();
    }

    private void OnEnable()
    {
        // 첫 프레임엔 Start 가 구독을 맡고, 이후 다시 켜졌을 때만 재구독한다.
        if (gameManager != null)
        {
            Subscribe();
        }
    }

    private void Subscribe()
    {
        if (gameManager == null)
        {
            return;
        }

        gameManager.ScoreChanged -= ShowScore;
        gameManager.ScoreChanged += ShowScore;
        ShowScore(gameManager.CurrentScore);
    }

    private void OnDisable()
    {
        if (gameManager != null)
        {
            gameManager.ScoreChanged -= ShowScore;
        }
    }

    private void ShowScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"SCORE : {score}";
        }
    }
}
