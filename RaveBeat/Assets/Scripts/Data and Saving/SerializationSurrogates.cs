using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

public class PlayDataSerializationSurrogate : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        PlayData pd = (PlayData)obj;
        info.AddValue("cleared", pd.cleared);
        info.AddValue("grade", pd.grade);
        info.AddValue("score", pd.score);
        info.AddValue("highestCombo", pd.highestCombo);
        info.AddValue("hitCount", pd.hitCount);
        info.AddValue("missCount", pd.missCount);
        info.AddValue("nearCount", pd.nearCount);
        info.AddValue("earlyCount", pd.earlyCount);
        info.AddValue("lateCount", pd.lateCount);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        PlayData pd = (PlayData)obj;
        pd.cleared = (Cleared)info.GetValue("cleared", typeof(Cleared));
        pd.grade = (Grade)info.GetValue("grade", typeof(Grade));
        pd.score = (float)info.GetValue("score", typeof(float));
        pd.highestCombo = (int)info.GetValue("highestCombo", typeof(int));
        pd.hitCount = (int)info.GetValue("hitCount", typeof(int));
        pd.missCount = (int)info.GetValue("missCount", typeof(int));
        pd.nearCount = (int)info.GetValue("nearCount", typeof(int));
        pd.earlyCount = (int)info.GetValue("earlyCount", typeof(int));
        pd.lateCount = (int)info.GetValue("lateCount", typeof(int));
        obj = pd;
        return obj;
    }

    
}

public class PlayHistoryDataSerializationSurrogate : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        PlayHistory ph = (PlayHistory)obj;
        int x = 0;
        while(x < ph.plays.Count)
        {
            info.AddValue(x.ToString(), ph.plays[x]);
            x++;
        }
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        PlayHistory ph = (PlayHistory)obj;
        int x = 0;
        while (x < ph.plays.Count)
        {
            ph.plays[x] = (PlayData)info.GetValue(x.ToString(), typeof(PlayData));
            x++;
        }
        obj = ph;
        return obj;
    }
}

public class ChartRecordDataSerializationSurrogate : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        ChartRecordData pd = (ChartRecordData)obj;
        info.AddValue("playerBest", pd.playerBest);
        info.AddValue("playHistory", pd.playHistory);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        ChartRecordData pd = (ChartRecordData)obj;
        pd.playerBest = (PlayData)info.GetValue("playerBest", typeof(PlayData));
        pd.playHistory = (PlayHistory)info.GetValue("playHistory", typeof(PlayHistory));
        obj = pd;
        return obj;
    }
}


public class SaveDataSerializationSurrogate : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        SaveData pd = (SaveData)obj; 
        int x = 0;
        while (x < pd.playerChartRecordData.Length)
        {
            info.AddValue(x.ToString(), pd.playerChartRecordData[x]);
            x++;
        }
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        SaveData pd = (SaveData)obj;
        int x = 0;
        while (x < pd.playerChartRecordData.Length)
        {
            pd.playerChartRecordData[x] = (ChartRecordData)info.GetValue(x.ToString(), typeof(ChartRecordData));
            x++;
        }
        obj = pd;
        return obj;
    }
}
