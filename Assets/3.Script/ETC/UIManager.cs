using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 게임 씬의 모든 UI를 한곳에서 관리한다.
/// - 현재 점수 HUD
/// - 좌/우 스킬 게이지
/// - 게임오버 시 결과창(재시작 / 타이틀) 표시
/// GameManager 의 이벤트를 구독해 동작한다.
/// </summary>
public sealed class UIManager : MonoBehaviour
{
    [Header("Score")]
    [SerializeField] private Text scoreText;

    [Header("Skill Gauge")]
    [SerializeField] private PlayerController player;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Image leftGaugeFill;
    [SerializeField] private Image rightGaugeFill;
    [SerializeField] private Text leftCooldownText;
    [SerializeField] private Text rightCooldownText;

    [Header("Result Panel")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private Text resultScoreText;
    [SerializeField] private Text resultBestText;
    [SerializeField] private GameObject newBestBadge;
    [SerializeField] private string titleSceneName = "TitleScene";

    private GameManager gameManager;
    private ScoreManager scoreManager;
    private bool lastRunWasNewBest;

    private void Start()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerController>();
        }

        if (playerInput == null)
        {
            playerInput = FindFirstObjectByType<PlayerInput>();
        }

        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }

        scoreManager = ScoreManager.Instance;
        scoreManager.ScoreAdded += HandleScoreAdded;

        gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            gameManager.ScoreChanged += ShowScore;
            gameManager.GameOver += ShowResultPanel;
            ShowScore(gameManager.CurrentScore);
        }
    }

    private void OnDestroy()
    {
        if (scoreManager != null)
        {
            scoreManager.ScoreAdded -= HandleScoreAdded;
        }

        if (gameManager != null)
        {
            gameManager.ScoreChanged -= ShowScore;
            gameManager.GameOver -= ShowResultPanel;
        }
    }

    // 게임오버 시 ScoreManager 가 이번 점수를 기록하면서 먼저 호출한다.
    private void HandleScoreAdded(int finalScore, bool isNewBest)
    {
        lastRunWasNewBest = isNewBest;
    }

    private void Update()
    {
        UpdateGauges();
        UpdateCooldownTexts();
    }

    private void UpdateGauges()
    {
        if (player == null)
        {
            return;
        }

        // 게이지 값이 0~max 를 살짝 벗어날 수 있어 Clamp01 로 막는다.
        if (leftGaugeFill != null)
        {
            leftGaugeFill.fillAmount = Mathf.Clamp01(player.currentLeftGauge / player.maxLeftGauge);
        }

        if (rightGaugeFill != null)
        {
            rightGaugeFill.fillAmount = Mathf.Clamp01(player.currentRightGauge / player.maxRightGauge);
        }
    }

    private void UpdateCooldownTexts()
    {
        if (playerInput == null)
        {
            return;
        }

        SetCooldownText(leftCooldownText, playerInput.leftCooldownRemaining);
        SetCooldownText(rightCooldownText, playerInput.rightCooldownRemaining);
    }

    // 남은 쿨타임을 소수점 없이 표시하고, 끝나면 빈 문자열로 지운다.
    private void SetCooldownText(Text target, float remaining)
    {
        if (target == null)
        {
            return;
        }

        target.text = remaining > 0f ? Mathf.CeilToInt(remaining).ToString() : string.Empty;
    }

    private void ShowScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"SCORE : {score}";
        }
    }

    private void ShowResultPanel()
    {
        int finalScore = gameManager != null ? gameManager.CurrentScore : 0;
        int bestScore = scoreManager != null ? scoreManager.BestScore : finalScore;

        if (resultScoreText != null)
        {
            resultScoreText.text = $"SCORE : {finalScore}";
        }

        if (resultBestText != null)
        {
            resultBestText.text = $"BEST : {bestScore}";
        }

        if (newBestBadge != null)
        {
            newBestBadge.SetActive(lastRunWasNewBest);
        }

        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>결과창 "재시작" 버튼에 연결한다.</summary>
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>결과창 "타이틀로" 버튼에 연결한다.</summary>
    public void GoToTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(titleSceneName);
    }
}
