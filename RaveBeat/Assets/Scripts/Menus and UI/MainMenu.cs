using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    static public bool hardMode = false;
    static public bool gameFirstStart = true;
    private bool chartButtonSpawn = false;
    private bool chartMenu = false;
    private bool chartExpand = false;

    public GameObject titleUi;
    public GameObject mainMenuUi;
    public GameObject chartSelectUi;

    public GameObject chartsUI;
    public GameObject recordsUi;

    public GameObject menuFirstSelected;
    public GameObject pakFirstSelected;

    public GameObject chartButtonPrefab;
    public GameObject chartButtonsContainer;
    public GameObject backButtonPrefab;
    public GameObject chartRecordPrefab;
    public GameObject chartRecordContainer;
    public GameObject hardButtonPrefab;

    private GameObject chartFirstSelected;
    static public GameObject chartLastOnScreen;
    private Chart[] charts;

    public Image backgroundRating;
    public Image backgroundGrade;
    public Image backgroundClear;

    public Image failedBackground;
    public Image clearedBackground;
    public Image hardClearedBackground;
    public Image superBackground;
    public Image normalBackground;

    public Image chartImage;
    public TMPro.TextMeshProUGUI chartNameText;
    public TMPro.TextMeshProUGUI chartArtistText;
    public TMPro.TextMeshProUGUI chartBPMText;
    public TMPro.TextMeshProUGUI chartGradeText;
    public TMPro.TextMeshProUGUI chartClearText;
    public TMPro.TextMeshProUGUI chartScoreText;
    public TMPro.TextMeshProUGUI chartMaxComboText;
    public TMPro.TextMeshProUGUI chartPlayerComboText;
    public TMPro.TextMeshProUGUI chartRatingText;
    public TMPro.TextMeshProUGUI chartPlayCountText;

    private GameObject lastChartSelected;
    private GameObject hardModeButton;

    private List<GameObject> spawnedRecords;
    // Start is called before the first frame update
    void Start()
    {
        if (gameFirstStart)
        {
            SaveDataManager.current.SetupData();
            titleUi.SetActive(true);
            GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(null);
            GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(pakFirstSelected);
        }
        else
        {
            titleUi.SetActive(false);
            mainMenuUi.SetActive(true);
            SaveDataManager.current.SaveData();
            GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(menuFirstSelected);
        }
    }

    void Update()
    {
        if (chartMenu)
        {
            UpdateCurrentChart();
            UpdateRecords();
        }
    }



    public void AnyKeyPressed()
    {
        titleUi.SetActive(false);
        mainMenuUi.SetActive(true);
        GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(menuFirstSelected);
        gameFirstStart = false;
    }

    public void ChartSelectPressed()
    {
        chartMenu = true;
        mainMenuUi.SetActive(false);
        chartSelectUi.SetActive(true);

        if (!chartButtonSpawn)
        {
            GameObject back = Instantiate(backButtonPrefab) as GameObject;
            back.transform.SetParent(chartButtonsContainer.transform);
            back.transform.localScale = new Vector3(1f, 1f, 1f);
            back.GetComponent<Button>().onClick.AddListener(() => BackToMenu());
            int x = 0;
            charts = Resources.LoadAll<Chart>("Charts");
            foreach (Chart chart in charts)
            {
                GameObject button = Instantiate(chartButtonPrefab) as GameObject;
                button.transform.SetParent(chartButtonsContainer.transform);
                button.transform.localScale = new Vector3(1f, 1f, 1f);
                button.GetComponent<ChartButton>().chart = chart;
                button.GetComponent<Button>().onClick.AddListener(() => LoadChart()); 
                if (x == 0) 
                {
                    GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(button); 
                    chartFirstSelected = button;
                }
                if(x == 5)chartLastOnScreen = button;
                x++;
            }
            chartButtonSpawn = true;
        }
        else
        {
            GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(chartFirstSelected);
        }
    }

    void UpdateRecords()
    {
        if (Input.GetButtonDown("MenuCancel"))
        {
            chartExpand = !chartExpand;
            if (chartExpand)
            {
                chartsUI.SetActive(false);
                recordsUi.SetActive(true);
                GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(null);
                ChartButton cb = lastChartSelected.GetComponent<ChartButton>();
                PlayHistory ph = SaveDataManager.current.saveData.playerChartRecordData[cb.chart.songID].playHistory;

                spawnedRecords = new List<GameObject>();
                GameObject hard = Instantiate(hardButtonPrefab) as GameObject;
                hard.transform.SetParent(chartRecordContainer.transform);
                hard.transform.localScale = new Vector3(1f, 1f, 1f);
                hard.GetComponent<Button>().onClick.AddListener(() => HardModePressed());
                GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(hard);
                spawnedRecords.Add(hard);
                hardModeButton = hard;

                ph.plays.Sort((b, a) => a.score.CompareTo(b.score));
                int x = 0;
                foreach (PlayData playData in ph.plays)
                {
                    if (x < (ph.plays.Count - 1))
                    {
                        GameObject button = Instantiate(chartRecordPrefab) as GameObject;
                        button.transform.SetParent(chartRecordContainer.transform);
                        button.transform.localScale = new Vector3(1f, 1f, 1f);
                        button.GetComponent<ChartRecordButton>().data = playData;
                        button.GetComponent<ChartRecordButton>().rank = (x + 1);
                        spawnedRecords.Add(button);
                        x++;
                    }
                }
            }
            else
            {
                foreach (GameObject record in spawnedRecords)
                {
                    Destroy(record);
                }

                chartsUI.SetActive(true);
                recordsUi.SetActive(false);
                GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(null);
                GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(lastChartSelected);
            }

        }
        if (chartExpand)
        {
            if (hardMode) hardModeButton.GetComponent<Image>().color = hardClearedBackground.color;
            else hardModeButton.GetComponent<Image>().color = normalBackground.color;
        }
    }

    void UpdateCurrentChart()
    {
        GameObject g = GameObject.Find("EventSystem").GetComponent<EventSystem>().currentSelectedGameObject;

        if (g != null)
        {
            if (g.name == "ChartButton(Clone)")
            {
                lastChartSelected = g;
                ChartButton b = g.GetComponent<ChartButton>();
                string grade = "";
                switch (SaveDataManager.current.saveData.playerChartRecordData[b.chart.songID].playerBest.grade)
                {
                    case Grade.S: grade = "S"; backgroundGrade.color = superBackground.color; break;
                    case Grade.AAA: grade = "AAA"; backgroundGrade.color = hardClearedBackground.color; break;
                    case Grade.AA: grade = "AA"; backgroundGrade.color = hardClearedBackground.color; break;
                    case Grade.A: grade = "A"; backgroundGrade.color = hardClearedBackground.color; break;
                    case Grade.B: grade = "B"; backgroundGrade.color = clearedBackground.color; break;
                    case Grade.C: grade = "C"; backgroundGrade.color = clearedBackground.color; break;
                    case Grade.D: grade = "C"; backgroundGrade.color = failedBackground.color; break;
                    case Grade.U: grade = "U";  backgroundGrade.color = failedBackground.color; break;
                    case Grade.None: grade = " ";  break;
                }
                chartGradeText.text = grade;

                string clear = "";
                switch (SaveDataManager.current.saveData.playerChartRecordData[b.chart.songID].playerBest.cleared)
                {
                    case Cleared.Clear: clear = "C"; backgroundClear.color = clearedBackground.color; break;
                    case Cleared.Fail: clear = "X"; backgroundClear.color = failedBackground.color; break;
                    case Cleared.HardClear: clear = "HC"; backgroundClear.color = hardClearedBackground.color; break;
                    case Cleared.None: clear = " "; break;
                }
                chartClearText.text = clear;
                chartImage.sprite = b.chart.songImage;
                chartScoreText.text = SaveDataManager.current.saveData.playerChartRecordData[b.chart.songID].playerBest.score.ToString("F0");
                chartPlayerComboText.text = SaveDataManager.current.saveData.playerChartRecordData[b.chart.songID].playerBest.highestCombo.ToString("F0");
                chartRatingText.text = b.chart.rating.ToString("F0");
                chartNameText.text = b.chart.songTitle;
                chartArtistText.text = b.chart.songArtist;
                chartBPMText.text = b.chart.bpm.ToString("F0");
                chartMaxComboText.text = b.chart.notes.Length.ToString("F0");
                chartPlayCountText.text = (SaveDataManager.current.saveData.playerChartRecordData[b.chart.songID].playHistory.plays.Count - 1).ToString("F0");

                if (b.chart.rating <= 5) backgroundRating.color = clearedBackground.color;
                else if (b.chart.rating <= 10) backgroundRating.color = hardClearedBackground.color;
                else if (b.chart.rating <= 15) backgroundRating.color = superBackground.color;
                else backgroundRating.color = failedBackground.color;

            }
        }
    }


    public void HardModePressed()
    {
        hardMode = !hardMode;
    }

    public void BackToMenu()
    {
        chartSelectUi.SetActive(false);
        mainMenuUi.SetActive(true);
        GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(menuFirstSelected);
    }

    public void ChartMakerPressed()
    {

    }

    public void OptionsPressed()
    {

    }

    public void QuitPressed()
    {
        SaveDataManager.current.SaveData();
        Application.Quit();
    }

    public void LoadChart()
    {
        GameObject chart =  GameObject.Find("EventSystem").GetComponent<EventSystem>().currentSelectedGameObject;
        PlayerPrefs.SetString("chart", chart.GetComponent<ChartButton>().chart.songTitle);
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }



}
