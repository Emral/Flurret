using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilemapMergerUIHelper : MonoBehaviour
{
    public void Save()
    {
        GetComponentInParent<TilemapMerger>().Save();
    }
}
