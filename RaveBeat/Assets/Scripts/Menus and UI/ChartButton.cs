using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChartButton : MonoBehaviour
{
    public Chart chart;

    public Image chartButtonImage;
    public TMPro.TextMeshProUGUI chartButtonName;
    public TMPro.TextMeshProUGUI chartButtonGrade;
    public TMPro.TextMeshProUGUI chartButtonClear;
    public TMPro.TextMeshProUGUI chartButtonRating;

    public Image backgroundRating;
    public Image backgroundGrade;
    public Image backgroundClear;

    public Image failedBackground;
    public Image clearedBackground;
    public Image hardClearedBackground;
    public Image superBackground;

    // Start is called before the first frame update
    void Start()
    {
        chartButtonImage.sprite = chart.songImage;
        chartButtonName.text = chart.songTitle;
        chartButtonRating.text = chart.rating.ToString("F0");
        if (chart.rating <= 5) backgroundRating.color = clearedBackground.color;
        else if (chart.rating <= 10) backgroundRating.color = hardClearedBackground.color;
        else if (chart.rating <= 15) backgroundRating.color = superBackground.color;
        else backgroundRating.color = failedBackground.color;

        string grade = "";
        switch (SaveDataManager.current.saveData.playerChartRecordData[chart.songID].playerBest.grade)
        {
            case Grade.S: grade = "S"; backgroundGrade.color = superBackground.color; break;
            case Grade.AAA: grade = "AAA"; backgroundGrade.color = hardClearedBackground.color; break;
            case Grade.AA: grade = "AA"; backgroundGrade.color = hardClearedBackground.color; break;
            case Grade.A: grade = "A"; backgroundGrade.color = hardClearedBackground.color; break;
            case Grade.B: grade = "B"; backgroundGrade.color = clearedBackground.color; break;
            case Grade.C: grade = "C"; backgroundGrade.color = clearedBackground.color; break;
            case Grade.D: grade = "D"; backgroundGrade.color = failedBackground.color; break;
            case Grade.U: grade = "U"; backgroundGrade.color = failedBackground.color; break;
            case Grade.None: grade = " "; break;
        }
        chartButtonGrade.text = grade;

        string clear = "";
        switch (SaveDataManager.current.saveData.playerChartRecordData[chart.songID].playerBest.cleared)
        {
            case Cleared.Clear: clear = "C"; backgroundClear.color = clearedBackground.color; break;
            case Cleared.Fail: clear = "X"; backgroundClear.color = failedBackground.color; break;
            case Cleared.HardClear: clear = "HC"; backgroundClear.color = hardClearedBackground.color; break;
        }
        chartButtonClear.text = clear;
    }

}
