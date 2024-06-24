using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
public static class SavingSerialization
{
    public static bool SavePlayerData(string saveName, object saveData)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        if (!Directory.Exists(Application.persistentDataPath + "/savedata"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/savedata");
        }

        string path = Application.persistentDataPath + "/savedata/" + saveName + ".txt";
        FileStream stream = File.Create(path);

        formatter.Serialize(stream, saveData);

        stream.Close();

        Debug.Log("Saved to: " + path);
        return true;
    }

    public static object loadPlayerData(string path)
    {
        if(!File.Exists(path))
        {
            return null;
        }

        BinaryFormatter formatter = new BinaryFormatter();

        FileStream stream = File.Open(path, FileMode.Open);

        try
        {
            object save = formatter.Deserialize(stream);
            stream.Close();
            return save;
        }
        catch
        {
            Debug.Log("Failed to load save data at: " + path);
            stream.Close();
            return null;
        }
    }

    public static BinaryFormatter GetBinaryFormatter()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        SurrogateSelector selector = new SurrogateSelector();

        PlayDataSerializationSurrogate playDataSurrogate = new PlayDataSerializationSurrogate();
        PlayHistoryDataSerializationSurrogate playHistorySurrogate = new PlayHistoryDataSerializationSurrogate();
        ChartRecordDataSerializationSurrogate chartDataSurrogate = new ChartRecordDataSerializationSurrogate();
        SaveDataSerializationSurrogate playerDataSurrogate = new SaveDataSerializationSurrogate();

        selector.AddSurrogate(typeof(PlayData), new StreamingContext(StreamingContextStates.All), playDataSurrogate);
        selector.AddSurrogate(typeof(PlayHistory), new StreamingContext(StreamingContextStates.All), playHistorySurrogate);
        selector.AddSurrogate(typeof(ChartRecordData), new StreamingContext(StreamingContextStates.All), chartDataSurrogate);
        selector.AddSurrogate(typeof(SaveData), new StreamingContext(StreamingContextStates.All), playerDataSurrogate);

        formatter.SurrogateSelector = selector;

        return formatter;
    }

}
