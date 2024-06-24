using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameConductor : Conductor
{
    private Chart chart;
    public GameObject GroundNoteNonePrefab;
    public GameObject GroundNoteLeftPrefab;
    public GameObject GroundNoteUpPrefab;
    public GameObject GroundNoteRightPrefab;
    public GameObject GroundNoteDownPrefab;

    public GameObject leftSkyNotePrefab;
    public GameObject rightSkyNotePrefab;

    public TMPro.TextMeshProUGUI hitsText;
    public TMPro.TextMeshProUGUI missesText;
    public TMPro.TextMeshProUGUI earliesText;
    public TMPro.TextMeshProUGUI latesText;
    public TMPro.TextMeshProUGUI scoreText;
    public TMPro.TextMeshProUGUI comboText;
    public TMPro.TextMeshProUGUI maxComboText;
    public Image redHealthBar;
    public Image greenHealthBar;
    public Image hardHealthBar;

    public GameObject earlyIndicator;
    public GameObject lateIndicator;
    public GameObject missIndicator;
    public GameObject wrongIndicator;
    public GameObject comboIndicator;
    public GameObject hitIndicator;

    private float[] noteBeats;
    private NoteType[] noteTypes;
    private NoteDir[] noteDirs;

    private float noteStartZ;
    private float noteEndZ;
    private float noteGroundY;
    private float noteSkyY;
    private float noteGroundBlueX;
    private float noteGroundRedY;
    private float noteSkyRedX;
    private float noteSkyBlueX;

    private float maxScore;
    private float hitScore;
    private float nearScore;
    private float playerScore;
    private int playerCombo;
    private int playerComboMax;

    private int hitCount;
    private int earlyCount;
    private int lateCount;
    private int missCount;

    private float clearHP;
    private float maxHP;
    private float missScale;
    private float hitScale;
    private float playerHP;
    private float missHP;
    private float hitHP;

    private bool chartEnded;
    private bool gameEnded;
    private bool audioPaused;
    private bool hardMode;

    public Image chartImage;
    public TMPro.TextMeshProUGUI chartNameText;
    public TMPro.TextMeshProUGUI chartArtistText;
    public TMPro.TextMeshProUGUI chartBPMText;
    public TMPro.TextMeshProUGUI chartGradeText;
    public TMPro.TextMeshProUGUI chartClearText;
    public TMPro.TextMeshProUGUI chartScoreText;
    public TMPro.TextMeshProUGUI chartPlayerComboText;
    public TMPro.TextMeshProUGUI chartMaxComboText;
    public TMPro.TextMeshProUGUI chartHitsText;
    public TMPro.TextMeshProUGUI chartNearText;
    public TMPro.TextMeshProUGUI chartMissText;
    public TMPro.TextMeshProUGUI chartEarlyText;
    public TMPro.TextMeshProUGUI chartLateText;

    public GameObject scoreMenuUI;
    public GameObject gameMenuUI;
    public GameObject scoreMenuSelected;

    private Queue<Note> notesOnScreen;
    private int nextNoteIndex;
    private float songCurrentPosition;
    private float songBeatDuration;
    private float songElapsed;

    public float hitOffset;
    public float nearOffset;

    public bool leftTriggerInUse;
    public bool rightTriggerInUse;
    public bool leftXInUse;
    public bool leftYInUse;
    public bool rightXInUse;
    public bool rightYInUse;


    void Start()
    {
        if(!MainMenu.gameFirstStart)
        {
            SaveDataManager.current.LoadData();
            MainMenu.gameFirstStart = true;
        }
        string filepath = PlayerPrefs.GetString("chart");
        chart = Resources.Load<Chart>("Charts/" + filepath);
        hardMode = MainMenu.hardMode;

        //variable init and setting
        notesOnScreen = new Queue<Note>();

        //note positions
        noteStartZ = 15f;
        noteEndZ = 0.02f;
        noteGroundY = 0.25f;
        noteSkyY = 0.75f;
        noteGroundBlueX = -0.45f;
        noteGroundRedY = 0.45f;
        noteSkyBlueX = -0.4f;
        noteSkyRedX = 0.4f;

        //looping
        chartEnded = false;
        gameEnded = false;
        audioPaused = false;
        nextNoteIndex = 0;

        //input checking
        leftTriggerInUse = false;
        rightTriggerInUse = false;
        leftXInUse = false;
        leftYInUse = false;
        rightXInUse = false;
        rightYInUse = false;

        //chart data
        noteBeats = chart.notes;
        noteTypes = chart.noteTypes;
        noteDirs = chart.noteDirs;
        songBeatDuration = 60f / chart.bpm;
        GetComponent<AudioSource>().clip = chart.file;

        //score
        maxScore = 100000;
        hitScore = maxScore / noteBeats.Length;
        nearScore = hitScore / 2;
        playerScore = 0;
        playerCombo = 0;
        playerComboMax = 0;

        //accuracy
        hitCount = 0;
        missCount = 0;
        earlyCount = 0;
        lateCount = 0;

        //hp
        redHealthBar.enabled = false;
        greenHealthBar.enabled = false;
        hardHealthBar.enabled = false;

        maxHP = 100;

        if (hardMode) { playerHP = 100; clearHP = 1; hardHealthBar.enabled = true; }
        else { playerHP = 0; clearHP = 80; }

        missScale = 0.5f;
        hitScale = 0.7f;
        float perHitHP = maxHP / (noteBeats.Length * hitScale);
        float perMissHP = maxHP / (noteBeats.Length * missScale);
        hitHP = perHitHP;
        missHP = -perMissHP;


        //
        hitsText.text = "0";
        missesText.text = "0";
        earliesText.text = "0";
        latesText.text = "0";
        comboText.text = "0";
        maxComboText.text = "0";
        scoreText.text = "0";

        GetComponent<AudioSource>().Play();
        songElapsed = (float)AudioSettings.dspTime;
    }

    void Update()
    {
        //if chart hasnt ended
        if (!chartEnded)
        {
            //Chart Loop  -  Chart is in progress - could be paused or unpaused
            
            //If game has been paused
            if (PauseMenu.gameIsPaused)
            {
                GetComponent<AudioSource>().Pause();
                audioPaused = true;
            }
            //If game has been unpaused, but audio is still paused
            else if (!PauseMenu.gameIsPaused && audioPaused)
            {
                GetComponent<AudioSource>().UnPause();
                songElapsed += PauseMenu.pauseTime;
                audioPaused = false;
            }

            //if game is not paused, and the audio is not paused
            if (!PauseMenu.gameIsPaused && !audioPaused)
            {
                //Game Loop  - Chart is being played - game is unpaused

                //Get the songs current position in time
                songCurrentPosition = (float)((AudioSettings.dspTime - songElapsed) - chart.songOffset);

                //Convert the songs current time into beats
                songCurrentPosInBeats = songCurrentPosition / songBeatDuration;

                //Spawn next note - if there is one and its time in the song has come
                //If there is a next note and it's beat is less than the songs position on the track
                if (nextNoteIndex < noteBeats.Length && noteBeats[nextNoteIndex] < songCurrentPosInBeats + beatsOnTrack)
                {
                    GameObject prefab = GroundNoteNonePrefab;

                    switch (noteDirs[nextNoteIndex])
                    {
                        case NoteDir.None: prefab = GroundNoteNonePrefab; break;
                        case NoteDir.Up: prefab = GroundNoteUpPrefab; break;
                        case NoteDir.Down: prefab = GroundNoteDownPrefab; break;
                        case NoteDir.Left: prefab = GroundNoteLeftPrefab; break;
                        case NoteDir.Right: prefab = GroundNoteRightPrefab; break;
                    }

                    //what note to spawn
                    switch (noteTypes[nextNoteIndex])
                    {
                        case NoteType.GroundLeft:
                            {   //spawn note    - ground left note 
                                Note note = ((GameObject)Instantiate(prefab)).GetComponent<Note>();
                                note.PlaceNote(this, noteBeats[nextNoteIndex], noteStartZ, noteEndZ, noteGroundBlueX, noteGroundY, noteTypes[nextNoteIndex], noteDirs[nextNoteIndex]);
                                notesOnScreen.Enqueue(note);
                                break;
                            }
                        case NoteType.GroundRight:
                            {   //spawn note    - ground right note
                                Note note = ((GameObject)Instantiate(prefab)).GetComponent<Note>();
                                note.PlaceNote(this, noteBeats[nextNoteIndex], noteStartZ, noteEndZ, noteGroundRedY, noteGroundY, noteTypes[nextNoteIndex], noteDirs[nextNoteIndex]);
                                notesOnScreen.Enqueue(note);
                                break;
                            }
                        case NoteType.SkyLeft:
                            {   //spawn note    - sky left note
                                Note note = ((GameObject)Instantiate(leftSkyNotePrefab)).GetComponent<Note>();
                                note.PlaceNote(this, noteBeats[nextNoteIndex], noteStartZ, noteEndZ, noteSkyBlueX, noteSkyY, noteTypes[nextNoteIndex], NoteDir.None);
                                notesOnScreen.Enqueue(note);
                                break;
                            }
                        case NoteType.SkyRight:
                            {   //spawn note    - sky right note
                                Note note = ((GameObject)Instantiate(rightSkyNotePrefab)).GetComponent<Note>();
                                note.PlaceNote(this, noteBeats[nextNoteIndex], noteStartZ, noteEndZ, noteSkyRedX, noteSkyY, noteTypes[nextNoteIndex], NoteDir.None);
                                notesOnScreen.Enqueue(note);
                                break;
                            }
                    }
                    //increment the note index
                    nextNoteIndex++;
                }

                //Check Inputs
                CheckInput();

                //Has Player missed any notes
                //Checks if the player has missed any notes
                if (notesOnScreen.Count > 0)
                {
                    //Find the front note
                    Note currentNote = notesOnScreen.Peek();
                    //Find its duration until the beat it is on (can be positive- time left to go, or negative- player has missed the note)
                    float noteDuration = currentNote.beat - songCurrentPosInBeats;
                    //If it falls behind the negative bound for the near offset
                    if (noteDuration < -nearOffset)
                    {
                        currentNote.gameObject.SetActive(false);
                        AddHealth(missHP);
                        missCount++;
                        playerCombo = 0;
                        StartCoroutine(IndicatorPosition(missIndicator, currentNote.indicator));
                        notesOnScreen.Dequeue();
                    }
                }

                //Has player's combo surpassed their max combo 
                if (playerCombo > playerComboMax)
                {
                    //update max combo to current combo
                    playerComboMax = playerCombo;
                }


                //Has chart finished
                //if there is not a next note and there isnt any notes on screen
                if (nextNoteIndex >= noteBeats.Length && notesOnScreen.Count <= 0)
                {
                    Debug.Log("Chart Finished!");
                    chartEnded = true;
                }

                //Has the player lost all their HP
                //Hard Mode
                if (hardMode)
                {
                    if (playerHP <= 0)
                    {
                        Debug.Log("Player Died!");
                        chartEnded = true;
                    }
                }

                //Low Priority Updates
                //Update Text
                TextUpdate();
            }
        }

        if (chartEnded && !gameEnded) //
        {   //game ended

            GetComponent<AudioSource>().Pause();

            Cleared clear;
            Grade grade;
            float score;
            int highCombo;
            int hits;
            int miss;
            int nears;
            int earlys;
            int lates;

            clear = Cleared.Fail;
            if (playerHP >= clearHP)
            {
                if (hardMode) clear = Cleared.HardClear;
                else clear = Cleared.Clear;
                Debug.Log("You cleared!");
            }
            else if (playerHP < clearHP)
            {
                clear = Cleared.Fail;
                Debug.Log("You failed!");
            }

            grade = Grade.None;
            if (playerScore == maxScore) grade = Grade.S;
            else if (playerScore >= (maxScore * 0.99)) grade = Grade.AAA; 
            else if (playerScore >= (maxScore * 0.95)) grade = Grade.AA; 
            else if (playerScore >= (maxScore * 0.9)) grade = Grade.A; 
            else if (playerScore >= (maxScore * 0.85)) grade = Grade.B; 
            else if (playerScore >= (maxScore * 0.8)) grade = Grade.C; 
            else if (playerScore >= (maxScore * 0.7)) grade = Grade.D;
            else if (playerScore < (maxScore * 0.7)) grade = Grade.U;

            score = playerScore;
            highCombo = playerComboMax;
            hits = hitCount;
            nears = earlyCount + lateCount;
            earlys = earlyCount;
            lates = lateCount;
            miss = missCount;

            PlayData data = new PlayData(clear, grade, score, highCombo, hits, miss, nears, earlys, lates);
            UpdateScoreMenu(data);
            SaveDataManager.current.AddChartPlayData(chart.songID, data);
            SaveDataManager.current.SaveData();

            gameEnded = true;
        }

        if (chartEnded && gameEnded)
        {
            gameMenuUI.SetActive(false);
            scoreMenuUI.SetActive(true);
        }
    }

    void PlayerInputted(NoteType type, NoteDir dir)
    {
        //Map Maker Timer
        //Debug.Log("Hit:" + songCurrentPosInBeats.ToString("F2"));

        //mark the song position where the player hit
        float hit = songCurrentPosInBeats;

        //are there any notes to be hit
        if (notesOnScreen.Count > 0)
        {
            // Get the current note
            Note currentNote = notesOnScreen.Peek();

            // calculate the timing between the player hit and the note's beat  
            float hitTiming = currentNote.beat - hit;

            bool playerHit = false;
            bool dirNote = false;
            if(currentNote.dir != NoteDir.None)
            {
                dirNote = true;
            }

            if(dirNote)
            {
                if (currentNote.type == type && currentNote.dir == dir) playerHit = true;
            }
            else
            {
                if (currentNote.type == type) playerHit = true;
            }

            //if the note type of the current note is equal to button hit
            if (playerHit)
            {
                //if the hit timing is within the positive and negative bounds of the hit offset
                if (hitTiming <= hitOffset && hitTiming >= -hitOffset)
                {   //player hit the note
                    currentNote.gameObject.SetActive(false);
                    playerScore += hitScore;
                    playerCombo++;
                    StartCoroutine(IndicatorSlow(comboIndicator));
                    StartCoroutine(IndicatorPosition(hitIndicator, currentNote.indicator));
                    notesOnScreen.Dequeue();
                    AddHealth(hitHP);
                    hitCount++;
                } //else if the hit timing is within the positive and negative bounds of the near offset 
                else if (hitTiming <= nearOffset && hitTiming >= -nearOffset)
                {   //player neared the note
                    currentNote.gameObject.SetActive(false);
                    playerScore += nearScore;
                    playerCombo++;
                    StartCoroutine(IndicatorSlow(comboIndicator));
                    notesOnScreen.Dequeue();
                    //if the hit timing was positive
                    if (hitTiming > 0)
                    {   //player hit the note early
                        earlyCount++;
                        StartCoroutine(IndicatorFast(earlyIndicator));
                    }
                    else
                    {   //player hit the note late
                        lateCount++;
                        StartCoroutine(IndicatorFast(lateIndicator));
                    }
                }
            }
            //else if the hit timing is less than the near off set- they were going for that note but didnt hit the right button
            else if (hitTiming < nearOffset)
            {
                //didnt hit the right button
                currentNote.gameObject.SetActive(false);
                AddHealth(missHP);
                missCount++;
                playerCombo = 0;
                StartCoroutine(IndicatorPosition(wrongIndicator,currentNote.indicator));
                notesOnScreen.Dequeue();
            }
        }

        

    }

    void CheckInput()
    {
        if (Input.GetAxis("LeftTrigger") > 0.2f)
        {
            if (!leftTriggerInUse)
            {
                PlayerInputted(NoteType.SkyLeft, NoteDir.None);
                leftTriggerInUse = true;
            }
        }
        else if (Input.GetAxis("LeftTrigger") == 0)
        {
            leftTriggerInUse = false;
        }

        if (Input.GetAxis("RightTrigger") > 0.2f)
        {
            if (!rightTriggerInUse)
            {
                PlayerInputted(NoteType.SkyRight, NoteDir.None);
                rightTriggerInUse = true;
            }
        }
        else if (Input.GetAxis("RightTrigger") == 0)
        {
            rightTriggerInUse = false;
        }

        if (Input.GetAxis("LeftHorizontal") > 0.6f )
        {
            if (!leftXInUse)
            {
                PlayerInputted(NoteType.GroundLeft, NoteDir.Right);
                leftXInUse = true;
            }
        }
        else if (Input.GetAxis("LeftHorizontal") == 0)
        {
            leftXInUse = false;
        }

        if (Input.GetAxis("LeftHorizontal") < -0.6f)
        {
            if (!leftXInUse)
            {
                PlayerInputted(NoteType.GroundLeft, NoteDir.Left);
                leftXInUse = true;
            }
        }
        else if (Input.GetAxis("LeftHorizontal") == 0)
        {
            leftXInUse = false;
        }

        if (Input.GetAxis("LeftVertical") > 0.6f)
        {
            if (!leftYInUse)
            {
                PlayerInputted(NoteType.GroundLeft, NoteDir.Up);
                leftYInUse = true;
            }
        }
        else if (Input.GetAxis("LeftVertical") == 0)
        {
            leftYInUse = false;
        }

        if (Input.GetAxis("LeftVertical") < -0.6f)
        {
            if (!leftYInUse)
            {
                PlayerInputted(NoteType.GroundLeft, NoteDir.Down);
                leftYInUse = true;
            }
        }
        else if (Input.GetAxis("LeftVertical") == 0)
        {
            leftYInUse = false;
        }

        if (Input.GetAxis("RightHorizontal") > 0.6f)
        {
            if (!leftXInUse)
            {
                PlayerInputted(NoteType.GroundRight, NoteDir.Right);
                leftXInUse = true;
            }
        }
        else if (Input.GetAxis("RightHorizontal") == 0)
        {
            leftXInUse = false;
        }

        if (Input.GetAxis("RightHorizontal") < -0.6f)
        {
            if (!leftXInUse)
            {
                PlayerInputted(NoteType.GroundRight, NoteDir.Left);
                leftXInUse = true;
            }
        }
        else if (Input.GetAxis("RightHorizontal") == 0)
        {
            leftXInUse = false;
        }


        if (Input.GetAxis("RightVertical") > 0.6f)
        {
            if (!leftYInUse)
            {
                PlayerInputted(NoteType.GroundRight, NoteDir.Up);
                leftYInUse = true;
            }
        }
        else if (Input.GetAxis("RightVertical") == 0)
        {
            leftYInUse = false;
        }

        if (Input.GetAxis("RightVertical") < -0.6f)
        {
            if (!leftYInUse)
            {
                PlayerInputted(NoteType.GroundRight, NoteDir.Down);
                leftYInUse = true;
            }
        }
        else if (Input.GetAxis("RightVertical") == 0)
        {
            leftYInUse = false;
        }

    }

    void TextUpdate()
    {
        hitsText.text = hitCount.ToString("F0");
        missesText.text = missCount.ToString("F0");
        earliesText.text = earlyCount.ToString("F0");
        latesText.text = lateCount.ToString("F0");
        comboText.text = playerCombo.ToString("F0");
        maxComboText.text = playerComboMax.ToString("F0");
        scoreText.text = playerScore.ToString("F0");
        
        if(!hardMode) 
        {
            redHealthBar.fillAmount = playerHP / maxHP;
            greenHealthBar.fillAmount = playerHP / maxHP;
            if (playerHP >= clearHP) { redHealthBar.enabled = false; greenHealthBar.enabled = true; }
            else { redHealthBar.enabled = true; greenHealthBar.enabled = false; }
        }
        else hardHealthBar.fillAmount = playerHP / maxHP;

    }

    //adds or removes health and clamps the value between 0-100
    void AddHealth(float health)
    {
        //take player hp and add new hp value to it
        float newHealth = playerHP + health;

        //if new health is over 100
        if (newHealth > 100)
        {//clamp to 100
            playerHP = 100;
        }//if new health is less than 0
        else if (newHealth < 0)
        {//clamp to 0
            playerHP = 0;
        }//means newHealth is within our range so:
        else
        {//set new hp to players hp
            playerHP = newHealth;
        }

    }

    void UpdateScoreMenu(PlayData data)
    {
        chartImage.sprite = chart.songImage;
        chartNameText.text = chart.name;
        chartArtistText.text = chart.songArtist;
        chartBPMText.text = chart.bpm.ToString();

        string grade = "";
        switch (data.grade)
        {

            case Grade.S: grade = "S"; break;
            case Grade.AAA: grade = "AAA"; break;
            case Grade.AA:  grade = "AA"; break;
            case Grade.A: grade = "A"; break;
            case Grade.B: grade = "B"; break;
            case Grade.C: grade = "C"; break;
            case Grade.D: grade = "D"; break;
            case Grade.U: grade = "U"; break;

        }
        chartGradeText.text = grade;

        string clear = "";
        switch (data.cleared)
        {
            case Cleared.Clear: clear = "Cleared!"; break;
            case Cleared.Fail: clear = "Failed!"; break;
            case Cleared.HardClear: clear = "Hard Cleared!"; break;
;        }
        chartClearText.text = clear;

        chartScoreText.text = data.score.ToString("F0");
        chartPlayerComboText.text = data.highestCombo.ToString("F0");
        chartMaxComboText.text = chart.notes.Length.ToString("F0");
        chartHitsText.text = data.hitCount.ToString("F0");
        chartNearText.text = data.nearCount.ToString("F0");
        chartMissText.text = data.missCount.ToString("F0");
        chartEarlyText.text = data.earlyCount.ToString("F0");
        chartLateText.text = data.lateCount.ToString("F0");
    }

    private IEnumerator IndicatorFast(GameObject indicator)
    {
        indicator.SetActive(true);

        yield return new WaitForSeconds(0.25f);

        indicator.SetActive(false);
    }

    private IEnumerator IndicatorSlow(GameObject indicator)
    {
        indicator.SetActive(true);

        yield return new WaitForSeconds(0.7f);

        indicator.SetActive(false);
    }

    private IEnumerator IndicatorPosition(GameObject indicator, Vector3 pos)
    {
        indicator.transform.localPosition = pos;
        indicator.SetActive(true);

        yield return new WaitForSeconds(0.25f);

        indicator.SetActive(false);
    }



}
            

  