using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public enum Grade
{
    S, AAA, AA,
    A, B, C,
    D, U, None 
}
[System.Serializable]
public enum Cleared
{
    Fail, Clear, HardClear, None
}

[System.Serializable]
public class PlayData
{
    public Cleared cleared;
    public Grade grade;
    public float score;
    public int highestCombo;
    public int hitCount;
    public int missCount;
    public int nearCount;
    public int earlyCount;
    public int lateCount;

    public PlayData(Cleared clear, Grade grd, float scr, int highCombo,
    int hits, int miss, int nears, int earlies, int lates)
    {
        cleared = clear;
        grade = grd;
        score = scr;
        highestCombo = highCombo;
        hitCount = hits;
        missCount = miss;
        nearCount = nears;
        earlyCount = earlies;
        lateCount = lates;
    }
}

[System.Serializable]
public class PlayHistory
{
    public List<PlayData> plays;
    public PlayHistory(PlayData data)
    {
        plays = new List<PlayData>{data};
    }
}

[System.Serializable]
public class ChartRecordData
{
    public PlayData playerBest;
    public PlayHistory playHistory;

    public void AddPlay(PlayData data)
    {
        playHistory.plays.Add(data);
        if (data.score > playerBest.score)
        {
            playerBest = data;
        }

        if(data.cleared == Cleared.HardClear)
        {
            playerBest.cleared = Cleared.HardClear;
        }
    }

    public ChartRecordData()
    {
        playerBest = new PlayData(Cleared.None, Grade.None, 0, 0, 0, 0, 0, 0, 0);
        playHistory = new PlayHistory(playerBest);
    }

}