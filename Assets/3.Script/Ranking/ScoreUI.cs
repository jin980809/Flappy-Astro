using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class ScoreUI : MonoBehaviour
{
    [Header("Summary")]
    [SerializeField] private Text currentScoreText;
    [SerializeField] private Text bestScoreText;
    [SerializeField] private GameObject newBestMessage;

    [Header("Top 10")]
    [SerializeField] private Button recordsButton;
    [SerializeField] private GameObject recordsPanel;
    [SerializeField] private Transform rowContainer;
    [SerializeField] private ScoreRowUI rowPrefab;
    [SerializeField] private GameObject emptyMessage;
    [SerializeField] private bool showRecordsOnStart;

    private readonly List<ScoreRowUI> rows = new List<ScoreRowUI>();
    private ScoreManager manager;

    private void OnEnable()
    {
        manager = ScoreManager.Instance;
        manager.RecordsChanged += Refresh;
        manager.ScoreAdded += HandleScoreAdded;

        if (recordsButton != null)
        {
            recordsButton.onClick.AddListener(ToggleRecordsPanel);
        }

        if (recordsPanel != null)
        {
            recordsPanel.SetActive(showRecordsOnStart);
        }

        if (newBestMessage != null)
        {
            newBestMessage.SetActive(false);
        }

        Refresh();
    }

    private void OnDisable()
    {
        if (manager != null)
        {
            manager.RecordsChanged -= Refresh;
            manager.ScoreAdded -= HandleScoreAdded;
        }

        if (recordsButton != null)
        {
            recordsButton.onClick.RemoveListener(ToggleRecordsPanel);
        }
    }

    public void ShowCurrentScore(int score)
    {
        if (currentScoreText != null)
        {
            currentScoreText.text = score.ToString();
        }
    }

    public void SetRecordsPanelVisible(bool visible)
    {
        if (recordsPanel != null)
        {
            recordsPanel.SetActive(visible);
        }
    }

    private void HandleScoreAdded(int score, bool isNewBest)
    {
        ShowCurrentScore(score);

        if (newBestMessage != null)
        {
            newBestMessage.SetActive(isNewBest);
        }
    }

    private void Refresh()
    {
        if (bestScoreText != null)
        {
            bestScoreText.text = manager.HasRecords ? manager.BestScore.ToString() : "-";
        }

        if (rowContainer == null || rowPrefab == null)
        {
            return;
        }

        EnsureRowCount(manager.Records.Count);

        for (int index = 0; index < rows.Count; index++)
        {
            bool hasRecord = index < manager.Records.Count;
            rows[index].gameObject.SetActive(hasRecord);

            if (hasRecord)
            {
                rows[index].SetRecord(index + 1, manager.Records[index]);
            }
        }

        if (emptyMessage != null)
        {
            emptyMessage.SetActive(!manager.HasRecords);
        }
    }

    private void EnsureRowCount(int requiredCount)
    {
        while (rows.Count < requiredCount)
        {
            rows.Add(Instantiate(rowPrefab, rowContainer));
        }
    }

    private void ToggleRecordsPanel()
    {
        if (recordsPanel != null)
        {
            recordsPanel.SetActive(!recordsPanel.activeSelf);
        }
    }
}
