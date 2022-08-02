using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;
using UnityEngine;

public class ChartRecordButton : MonoBehaviour
{
    public PlayData data;
    public int rank;

    public Image failedBackground;
    public Image clearedBackground;
    public Image hardClearedBackground;
    public Image superBackground;

    public Image backgroundRank;
    public Image backgroundGrade;
    public Image backgroundClear;

    public TMPro.TextMeshProUGUI chartGradeText;
    public TMPro.TextMeshProUGUI chartClearText;
    public TMPro.TextMeshProUGUI chartScoreText;
    public TMPro.TextMeshProUGUI chartRankText;
    public TMPro.TextMeshProUGUI chartHitText;
    public TMPro.TextMeshProUGUI chartNearText;
    public TMPro.TextMeshProUGUI chartMissText;



    // Start is called before the first frame update
    void Start()
    {
        string grade = "";
        switch (data.grade)
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
        chartGradeText.text = grade;

        string clear = "";
        switch (data.cleared)
        {
            case Cleared.Clear: clear = "C"; backgroundClear.color = clearedBackground.color; break;
            case Cleared.Fail: clear = "X"; backgroundClear.color = failedBackground.color; break;
            case Cleared.HardClear: clear = "HC"; backgroundClear.color = hardClearedBackground.color; break;
            case Cleared.None: clear = " "; break;
        }
        chartClearText.text = clear;

        chartScoreText.text = data.score.ToString("F0");

        switch (rank)
        {
            case 1: backgroundRank.color = superBackground.color; break;
            case 2: backgroundRank.color = hardClearedBackground.color; break;
            case 3: backgroundRank.color = clearedBackground.color; break;
        }
        
        chartRankText.text = rank.ToString("F0");
        chartHitText.text = data.hitCount.ToString("F0");
        chartNearText.text = data.nearCount.ToString("F0"); 
        chartMissText.text = data.missCount.ToString("F0");

    }


}
