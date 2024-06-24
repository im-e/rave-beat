using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class EditorConductor : Conductor
{

    public GameObject GroundNoteNonePrefab;
    public GameObject GroundNoteLeftPrefab;
    public GameObject GroundNoteUpPrefab;
    public GameObject GroundNoteRightPrefab;
    public GameObject GroundNoteDownPrefab;

    public GameObject leftSkyNotePrefab;
    public GameObject rightSkyNotePrefab;

    private float noteStartZ;
    private float noteEndZ;
    private float noteGroundY;
    private float noteSkyY;
    private float noteGroundBlueX;
    private float noteGroundRedY;
    private float noteSkyRedX;
    private float noteSkyBlueX;


    private List<float> noteBeats;
    private List<NoteType> noteTypes;
    private List<NoteDir> noteDirs;
    private float chartOffset;


    private int nextNoteIndex;
    private Queue<Note> notesOnScreen;
    private float songCurrentPosition;
    private float songBeatDuration;
    private float songElapsed;



    private bool editorPaused;
    private bool editorEnded;
    private bool audioPaused;
    private float pauseTime;
    private float pausedTime;

 
    public bool leftTriggerInUse;
    public bool rightTriggerInUse;
    public bool leftXInUse;
    public bool leftYInUse;
    public bool rightXInUse;
    public bool rightYInUse;


    void Start()
    {
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
        editorEnded = false;
        editorPaused = true;
        audioPaused = false;
        nextNoteIndex = 0;

        //input checking
        leftTriggerInUse = false;
        rightTriggerInUse = false;
        leftXInUse = false;
        leftYInUse = false;
        rightXInUse = false;
        rightYInUse = false;

        //set by the player
        //songBeatDuration = 60f / chart.bpm;
       // GetComponent<AudioSource>().clip = chart.file;

        //chart data
        noteBeats = new List<float>();
        noteTypes = new List<NoteType>();
        noteDirs = new List<NoteDir>();
        chartOffset = 0f;



        GetComponent<AudioSource>().Play();
        songElapsed = (float)AudioSettings.dspTime;
    }

    void Update()
    {
        //if chart hasnt ended
        if (!editorEnded)
        {
            //Chart Loop  -  Chart is in progress - could be paused or unpaused
            
            //If game has been paused
            if (editorPaused)
            {
                pausedTime = (float)AudioSettings.dspTime;
                GetComponent<AudioSource>().Pause();
                audioPaused = true;
            }
            //If game has been unpaused, but audio is still paused
            else if (!editorPaused && audioPaused)
            {
                GetComponent<AudioSource>().UnPause();
                songElapsed += pauseTime;
                audioPaused = false;
            }

            //if game is not paused, and the audio is not paused
            if (!editorPaused && !audioPaused)
            {
                //Game Loop  - Chart is being played - game is unpaused

                //Get the songs current position in time
                songCurrentPosition = (float)((AudioSettings.dspTime - songElapsed) - chartOffset);

                //Convert the songs current time into beats
                songCurrentPosInBeats = songCurrentPosition / songBeatDuration;

                //Spawn next note - if there is one and its time in the song has come
                //If there is a next note and it's beat is less than the songs position on the track
                if (nextNoteIndex < noteBeats.Count && noteBeats[nextNoteIndex] < songCurrentPosInBeats + beatsOnTrack)
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
                    if (noteDuration < 0)
                    {
                        currentNote.gameObject.SetActive(false);
                        notesOnScreen.Dequeue();
                    }
                }

            }
        }
    }


    void CheckInput()
    {

        if (Input.GetButtonDown("GamePause"))
        {
            if (!editorPaused)
            {
                editorPaused = true;
            }
            else
            {
                pauseTime = (float)(AudioSettings.dspTime - pausedTime);
            }
        }

        if (Input.GetAxis("LeftTrigger") > 0.2f)
        {
            if (!leftTriggerInUse)
            {
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
                leftYInUse = true;
            }
        }
        else if (Input.GetAxis("RightVertical") == 0)
        {
            leftYInUse = false;
        }

    }
}
            

  