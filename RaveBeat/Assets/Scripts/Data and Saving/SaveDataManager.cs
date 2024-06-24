using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveDataManager
{ 
    public SaveData saveData;

    private static SaveDataManager _current;
    public static SaveDataManager current
    {
        get
        {
            if (_current == null)
            {
                _current = new SaveDataManager();
            }
            return _current;
        }
        set
        {
            if (value != null)
            {
                _current = value;
            }
        }
    }

    public void SetupData()
    {
        SaveDataManager save = (SaveDataManager)SavingSerialization.loadPlayerData(Application.persistentDataPath + "/savedata/data.txt");
        if (save == null)
        {
            Debug.Log("Previous Save Data not found!");
            Debug.Log("Creating Fresh Data...");
            saveData = new SaveData();
        }
        else
        {
            Debug.Log("Previous Save Data Found!");
            current = save;
            saveData = save.saveData;
        }
    }

    public bool LoadData()
    {
        SaveDataManager save = (SaveDataManager)SavingSerialization.loadPlayerData(Application.persistentDataPath + "/savedata/data.txt");
        if (save == null)
        {
            Debug.Log("Unable to load save file!");
            return false;
        }
        else
        {
            Debug.Log("Loading Saved Data...");
            current = save;
            return true;
        }
    }

    public void SaveData()
    {
        SavingSerialization.SavePlayerData("data", current);
    }

    public void AddChartPlayData(int id, PlayData data)
    {
        saveData.playerChartRecordData[id].AddPlay(data);
    }


}
