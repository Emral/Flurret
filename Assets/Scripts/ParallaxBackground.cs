using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public enum VerticalAlignment
{
    Top = 1,
    Center = 0,
    Bottom = -1
}

public enum HorizontalAlignment
{
    Right = 1,
    Center = 0,
    Left = -1
}

[System.Serializable]
public class ParallaxLayer
{
    public Sprite image;
    public Color tint = Color.white;

    [Range(-1, 1000)]
    public float depth;

    public Vector2 speed = Vector2.zero;
    public Vector2 repeat = Vector2.zero;

    public HorizontalAlignment horizontalAlign;
    public VerticalAlignment verticalAlign;

    public bool hidden;

    public bool advanced = false;

    [AllowNesting]
    [ShowIf("advanced")]
    public string tag = "Default";

    [AllowNesting]
    [ShowIf("advanced")]
    public Animation animation;

    [AllowNesting]
    [ShowIf("advanced")]
    public Material spriteMaterial;

    [AllowNesting]
    [ShowIf("advanced")]
    public GameObject extraEffects;

    [AllowNesting]
    [ShowIf("advanced")]
    public bool overrideSpeed = false;

    [AllowNesting]
    [ShowIf(EConditionOperator.And, "advanced", "overrideSpeed")]
    public Vector2 speedOverride = Vector2.zero;

    [AllowNesting]
    [ShowIf("advanced")]
    public bool overridePriority = false;

    [AllowNesting]
    [ShowIf(EConditionOperator.And, "advanced", "overridePriority")]
    public float priority = 0;

    [AllowNesting]
    [ShowIf("advanced")]
    public Vector2 offset = Vector2.zero;

    [AllowNesting]
    [ShowIf("advanced")]
    public Vector2 gap = Vector2.zero;

    [AllowNesting]
    [ShowIf("advanced")]
    public RectOffset margin;
}

[CreateAssetMenu(menuName = "Create Parallax")]
public class ParallaxBackground : ScriptableObject
{
    public Color backdropColor = Color.black;

    public List<ParallaxLayer> layers = new List<ParallaxLayer>();
}