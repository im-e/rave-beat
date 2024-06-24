using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SaveData
{
    public ChartRecordData[] playerChartRecordData;

    public SaveData()
    {
        Chart[] charts;
        charts = Resources.LoadAll<Chart>("Charts");
        playerChartRecordData = new ChartRecordData[charts.Length];
        int x = 0;
        while (x < charts.Length)
        {
            playerChartRecordData[x] = new ChartRecordData(); 
            x++;
        }
    }
}


