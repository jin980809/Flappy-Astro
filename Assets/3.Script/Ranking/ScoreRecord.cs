using System;

[Serializable]
public class ScoreRecord
{
    public int score;
    public long achievedAtUnixMilliseconds;

    public ScoreRecord(int score, long achievedAtUnixMilliseconds)
    {
        this.score = score;
        this.achievedAtUnixMilliseconds = achievedAtUnixMilliseconds;
    }
}
