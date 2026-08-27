using System.Collections.Generic;

public static class ScoreSorter
{
    public static void SelectionSort(List<ScoreRecord> records)
    {
        if (records == null)
        {
            return;
        }

        for (int sortedIndex = 0; sortedIndex < records.Count - 1; sortedIndex++)
        {
            int bestIndex = sortedIndex;

            for (int candidateIndex = sortedIndex + 1; candidateIndex < records.Count; candidateIndex++)
            {
                if (ComesBefore(records[candidateIndex], records[bestIndex]))
                {
                    bestIndex = candidateIndex;
                }
            }

            if (bestIndex == sortedIndex)
            {
                continue;
            }

            // Shift instead of swapping so completely tied records keep their play order.
            ScoreRecord bestRecord = records[bestIndex];
            for (int index = bestIndex; index > sortedIndex; index--)
            {
                records[index] = records[index - 1];
            }

            records[sortedIndex] = bestRecord;
        }
    }

    private static bool ComesBefore(ScoreRecord left, ScoreRecord right)
    {
        if (left.score != right.score)
        {
            return left.score > right.score;
        }

        return left.achievedAtUnixMilliseconds < right.achievedAtUnixMilliseconds;
    }
}
