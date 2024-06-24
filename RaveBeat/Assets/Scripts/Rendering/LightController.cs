using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    private GameConductor conductor;
    public GameObject skyLine;
    public GameObject groundLine;
    public Material lineMaterial;
    public Material lineMaterialPressed;

    private void Start()
    {
        conductor = gameObject.GetComponent<GameConductor>();
    }

    // Update is called once per frame
    void Update()
    {
        if (conductor.leftTriggerInUse || conductor.rightTriggerInUse)
        { skyLine.GetComponent<MeshRenderer>().material = lineMaterialPressed; }
        else
        { skyLine.GetComponent<MeshRenderer>().material = lineMaterial; }

        if (conductor.leftXInUse || conductor.leftYInUse || conductor.rightXInUse || conductor.rightYInUse)
        { groundLine.GetComponent<MeshRenderer>().material = lineMaterialPressed; }
        else
        { groundLine.GetComponent<MeshRenderer>().material = lineMaterial; }


    }
}
