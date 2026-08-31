using System;
using System.IO;
using UnityEngine;

public sealed class RankingTestPanel : MonoBehaviour
{
    private readonly int[] sampleScores = { 12, 5, 20, 20, 1, 9, 15, 3, 18, 7, 11, 6 };

    private string scoreInput = "10";
    private string resultMessage = "점수를 입력하거나 샘플 기록을 추가하세요.";
    private Vector2 scrollPosition;

    private void OnGUI()
    {
        ScoreManager manager = ScoreManager.Instance;

        GUILayout.BeginArea(new Rect(20f, 20f, 560f, Screen.height - 40f), GUI.skin.box);
        GUILayout.Label("개인 최고 기록 TOP 10 검증");
        GUILayout.Space(8f);

        GUILayout.BeginHorizontal();
        GUILayout.Label("이번 점수", GUILayout.Width(80f));
        scoreInput = GUILayout.TextField(scoreInput, GUILayout.Width(120f));

        if (GUILayout.Button("기록 추가", GUILayout.Width(100f)))
        {
            AddEnteredScore(manager);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("샘플 12개 추가"))
        {
            AddSampleScores(manager);
        }

        if (GUILayout.Button("파일에서 다시 불러오기"))
        {
            manager.Reload();
            resultMessage = "저장 파일을 다시 불러왔습니다.";
        }

        if (GUILayout.Button("기록 초기화"))
        {
            ClearRecords(manager);
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(8f);
        GUILayout.Label(resultMessage);
        GUIStyle bestScoreStyle = new GUIStyle(GUI.skin.label);
        bestScoreStyle.fontStyle = FontStyle.Bold;
        bestScoreStyle.normal.textColor = Color.yellow;
        GUILayout.Label(
            $"최고 점수: {(manager.HasRecords ? manager.BestScore.ToString() : "-")}",
            manager.HasRecords ? bestScoreStyle : GUI.skin.label);
        GUILayout.Label($"저장된 기록: {manager.Records.Count} / 10");
        GUILayout.Label($"저장 경로: {manager.SaveFilePath}");

        GUILayout.Space(8f);
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        if (!manager.HasRecords)
        {
            GUILayout.Label("저장된 기록이 없습니다.");
        }

        for (int index = 0; index < manager.Records.Count; index++)
        {
            ScoreRecord record = manager.Records[index];
            DateTimeOffset localTime =
                DateTimeOffset.FromUnixTimeMilliseconds(record.achievedAtUnixMilliseconds).ToLocalTime();

            GUILayout.Label($"#{index + 1}   {record.score} SCORE   {localTime:yyyy-MM-dd HH:mm:ss}");
        }

        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    private void AddEnteredScore(ScoreManager manager)
    {
        if (!int.TryParse(scoreInput, out int score))
        {
            resultMessage = "정수 점수를 입력하세요.";
            return;
        }

        bool isNewBest = manager.AddScore(score);
        resultMessage = isNewBest
            ? $"{score}점 기록 완료 - 새로운 최고 기록!"
            : $"{score}점 기록 완료";
    }

    private void AddSampleScores(ScoreManager manager)
    {
        foreach (int score in sampleScores)
        {
            manager.AddScore(score);
        }

        resultMessage = "샘플 12개를 추가했습니다. TOP 10 제한과 동점 순서를 확인하세요.";
    }

    private void ClearRecords(ScoreManager manager)
    {
        try
        {
            if (File.Exists(manager.SaveFilePath))
            {
                File.Delete(manager.SaveFilePath);
            }

            manager.Reload();
            resultMessage = "저장 파일과 기록을 초기화했습니다.";
        }
        catch (Exception exception)
        {
            resultMessage = $"초기화 실패: {exception.Message}";
        }
    }
}
