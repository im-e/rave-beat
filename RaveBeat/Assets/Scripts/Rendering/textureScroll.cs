using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class textureScroll : MonoBehaviour
{
    public bool verticalScroll = true;
    public bool downOrLeft = true;
    public float scrollSpeed = .5f;
    public Renderer _renderer;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float vOffset = Time.time * scrollSpeed;

        if (!downOrLeft) vOffset = -vOffset;

        if (verticalScroll)_renderer.material.SetTextureOffset("_MainTex", new Vector2(0, vOffset));
        else _renderer.material.SetTextureOffset("_MainTex", new Vector2(vOffset, 0));
    }
}
