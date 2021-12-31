using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisassembledTilemap : MonoBehaviour
{
    public List<SpriteRenderer> sprites;
    public string sortingLayer;
    public Transform spriteRoot;
    public PolygonCollider2D polygonCollider;
}
