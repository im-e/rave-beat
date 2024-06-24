using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
[CreateAssetMenu(fileName ="New Chart", menuName = "Chart")]
public class Chart : ScriptableObject
{
    public int songID;
    public string songTitle;
    public string songArtist;
    public Sprite songImage;
    public int rating;

    public AudioClip file;
    public float bpm;
    public float songOffset;

    public float[] notes;
    public NoteType[] noteTypes;
    public NoteDir[] noteDirs;

}
