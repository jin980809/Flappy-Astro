using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public sealed class ScoreManager : MonoBehaviour
{
    private const int MaximumRecordCount = 10;
    private const string SaveFileName = "personal_top_10.json";
    private const long MaximumUnixMilliseconds = 253402300799999L;

    private static ScoreManager instance;

    private ScoreData scoreData = new ScoreData();
    private string saveFilePath;

    public static ScoreManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<ScoreManager>();

                if (instance == null)
                {
                    GameObject managerObject = new GameObject(nameof(ScoreManager));
                    instance = managerObject.AddComponent<ScoreManager>();
                }
            }

            return instance;
        }
    }

    public event Action RecordsChanged;
    public event Action<int, bool> ScoreAdded;

    public IReadOnlyList<ScoreRecord> Records => scoreData.records;
    public int BestScore => scoreData.records.Count == 0 ? 0 : scoreData.records[0].score;
    public bool HasRecords => scoreData.records.Count > 0;
    public string SaveFilePath => saveFilePath;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        saveFilePath = Path.Combine(Application.persistentDataPath, SaveFileName);
        Load();
    }

    public bool AddScore(int finalScore)
    {
        bool isNewBest = !HasRecords || finalScore > BestScore;
        long achievedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        scoreData.records.Add(new ScoreRecord(finalScore, achievedAt));
        ScoreSorter.SelectionSort(scoreData.records);
        TrimToMaximum();
        Save();

        ScoreAdded?.Invoke(finalScore, isNewBest);
        RecordsChanged?.Invoke();
        return isNewBest;
    }

    public void Reload()
    {
        Load();
        RecordsChanged?.Invoke();
    }

    private void Load()
    {
        scoreData = new ScoreData();

        try
        {
            if (!File.Exists(saveFilePath))
            {
                return;
            }

            string json = File.ReadAllText(saveFilePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                return;
            }

            ScoreData loadedData = JsonUtility.FromJson<ScoreData>(json);
            if (loadedData?.records == null)
            {
                return;
            }

            scoreData = loadedData;
            RemoveInvalidRecords();
            ScoreSorter.SelectionSort(scoreData.records);
            TrimToMaximum();
        }
        catch (Exception exception)
        {
            scoreData = new ScoreData();
            Debug.LogWarning($"Ranking data could not be loaded and was reset. {exception.Message}");
        }
    }

    private void Save()
    {
        try
        {
            string directory = Path.GetDirectoryName(saveFilePath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string json = JsonUtility.ToJson(scoreData, true);
            File.WriteAllText(saveFilePath, json);
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"Ranking data could not be saved. {exception.Message}");
        }
    }

    private void RemoveInvalidRecords()
    {
        for (int index = scoreData.records.Count - 1; index >= 0; index--)
        {
            ScoreRecord record = scoreData.records[index];
            if (record == null ||
                record.achievedAtUnixMilliseconds < 0 ||
                record.achievedAtUnixMilliseconds > MaximumUnixMilliseconds)
            {
                scoreData.records.RemoveAt(index);
            }
        }
    }

    private void TrimToMaximum()
    {
        while (scoreData.records.Count > MaximumRecordCount)
        {
            scoreData.records.RemoveAt(scoreData.records.Count - 1);
        }
    }
}
