using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// 타이틀 씬의 버튼/입력을 관리한다.
/// - 게임 시작: 게임 씬으로 이동
/// - 게임 나가기: 애플리케이션 종료
/// - 조작법 가이드 패널 / 랭킹 패널: 버튼으로 토글(다시 누르면 닫힘), ESC 로도 닫힌다.
/// </summary>
public sealed class TitleManager : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private GameObject guidePanel;
    [SerializeField] private GameObject rankingPanel;

    private void Awake()
    {
        if (guidePanel != null)
        {
            guidePanel.SetActive(false);
        }

        if (rankingPanel != null)
        {
            rankingPanel.SetActive(false);
        }
    }

    private void Update()
    {
        // 이 프로젝트는 새 Input System 만 쓰므로 Keyboard.current 로 읽는다.
        if (Keyboard.current == null || !Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            return;
        }

        // ESC: 열려 있는 패널을 닫는다.
        if (guidePanel != null && guidePanel.activeSelf)
        {
            guidePanel.SetActive(false);
        }

        if (rankingPanel != null && rankingPanel.activeSelf)
        {
            rankingPanel.SetActive(false);
        }
    }

    /// <summary>"게임 시작" 버튼에 연결한다.</summary>
    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>"게임 나가기" 버튼에 연결한다.</summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>"조작법" 버튼에 연결한다. 다시 누르면 닫힌다.</summary>
    public void ToggleGuide()
    {
        if (guidePanel == null)
        {
            return;
        }

        bool willOpen = !guidePanel.activeSelf;
        guidePanel.SetActive(willOpen);

        // 하나 열 때 다른 패널은 닫아 겹치지 않게 한다.
        if (willOpen && rankingPanel != null)
        {
            rankingPanel.SetActive(false);
        }
    }

    /// <summary>"랭킹" 버튼에 연결한다. 다시 누르면 닫힌다.</summary>
    public void ToggleRanking()
    {
        if (rankingPanel == null)
        {
            return;
        }

        bool willOpen = !rankingPanel.activeSelf;
        rankingPanel.SetActive(willOpen);

        if (willOpen && guidePanel != null)
        {
            guidePanel.SetActive(false);
        }
    }

    /// <summary>가이드 패널의 닫기 버튼용. (선택)</summary>
    public void CloseGuide()
    {
        if (guidePanel != null)
        {
            guidePanel.SetActive(false);
        }
    }

    /// <summary>랭킹 패널의 닫기 버튼용. (선택)</summary>
    public void CloseRanking()
    {
        if (rankingPanel != null)
        {
            rankingPanel.SetActive(false);
        }
    }
}
