using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 개인 TOP 10 기록을 Text 하나에 1등부터 줄바꿈으로 채워 넣는다.
/// 켜고 꺼지는 랭킹 패널(SetActive 되는 오브젝트)에 붙이면
/// 패널이 열릴 때마다 OnEnable 로 최신 기록을 다시 그린다.
/// </summary>
public sealed class RankingBoard : MonoBehaviour
{
    [SerializeField] private Text rankingText;
    [SerializeField] private int maxRank = 10;
    [SerializeField] private string emptyMessage = "No Record";

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (rankingText == null)
        {
            return;
        }

        IReadOnlyList<ScoreRecord> records = ScoreManager.Instance.Records;

        if (records.Count == 0)
        {
            rankingText.text = emptyMessage;
            return;
        }

        int count = Mathf.Min(records.Count, maxRank);
        StringBuilder builder = new StringBuilder();

        for (int index = 0; index < count; index++)
        {
            ScoreRecord record = records[index];
            System.DateTimeOffset achievedAt =
                System.DateTimeOffset.FromUnixTimeMilliseconds(record.achievedAtUnixMilliseconds).ToLocalTime();

            builder.Append($"#{index + 1}. {achievedAt:yyyy-MM-dd HH:mm} Score : {record.score}");

            if (index < count - 1)
            {
                builder.Append('\n');
            }

            if(index == 0)
            {
                builder.Append('\n');
            }
        }

        rankingText.text = builder.ToString();
    }
}
