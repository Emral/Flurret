using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderOnlyInEditor : MonoBehaviour
{
    private void Start()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        Array.ForEach(renderers, r => r.enabled = false);
    }
}