using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NoteType
{
    GroundLeft,
    GroundRight,
    SkyLeft,
    SkyRight,
}

public enum NoteDir
{
    None,
    Left,
    Up,
    Right,
    Down
}
public class Note : MonoBehaviour
{
   
    //Public
    public NoteType type;
    public NoteDir dir;
    public float beat;
    public Vector3 indicator;

    public Material leftMaterial;
    public Material rightMaterial;

    //Private
    private Conductor conductor;
    private Vector3 spawnPos;
    private Vector3 endPos;

    public void PlaceNote(Conductor gameConductor, float noteBeat, float zStart, float zEnd, float xPos, float yPos, NoteType ntype, NoteDir direction)
    {
        conductor = gameConductor;
        beat = noteBeat;
        type = ntype;
        dir = direction;

        spawnPos = new Vector3(xPos, yPos, zStart);
        endPos = new Vector3(xPos, yPos, zEnd);
        
        transform.position = new Vector3(xPos, yPos, zStart);

        switch (type)
        {
            case NoteType.GroundLeft:
                {
                    indicator = new Vector3(-375, -150, 0);
                    GetComponent<MeshRenderer>().material = leftMaterial;
                    break;
                }
            case NoteType.GroundRight:
                {
                    indicator = new Vector3(375, -150, 0);
                    GetComponent<MeshRenderer>().material = rightMaterial;
                    break;
                }
            case NoteType.SkyLeft:
                {
                    indicator = new Vector3(-375, 200, 0);
                    break;
                }
            case NoteType.SkyRight:
                {
                    indicator = new Vector3(375, 200, 0);
                    break;
                }
            default:
                {
                    break;
                }
        }

    }

    private void Start()
    {

    }

    void Update()
    {
        float timingGap = beat - conductor.songCurrentPosInBeats;
        float noteInterpolateRatio = (conductor.beatsOnTrack - timingGap) / conductor.beatsOnTrack;

        transform.position = Vector3.Lerp(spawnPos, endPos, noteInterpolateRatio);

    }


}
