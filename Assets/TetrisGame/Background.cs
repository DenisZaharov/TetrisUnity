using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Debug.Assert(meshFilter);

        Mesh mesh = meshFilter.mesh;

        var uv = mesh.uv;
        var colors = new Color[uv.Length];
        
        for (var i = 0; i < uv.Length; i++)
        {
            Color colorX = Color.Lerp(Color.white, Color.black, uv[i].x);
            Color colorY = Color.Lerp(Color.white, Color.black, uv[i].y);
            colors[i] = (colorX + colorY) / 2;
        }

        mesh.colors = colors;
    }
}
