using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class ScoreRowUI : MonoBehaviour
{
    [SerializeField] private Text rankText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text achievedAtText;
    [SerializeField] private string dateFormat = "yyyy-MM-dd HH:mm:ss";

    public void SetRecord(int rank, ScoreRecord record)
    {
        rankText.text = $"#{rank}";
        scoreText.text = $"{record.score} SCORE";

        DateTimeOffset achievedAt =
            DateTimeOffset.FromUnixTimeMilliseconds(record.achievedAtUnixMilliseconds).ToLocalTime();
        achievedAtText.text = achievedAt.ToString(dateFormat);
    }
}
