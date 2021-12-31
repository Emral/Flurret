using System.Collections.Generic;
using UnityEngine;

public enum CollisionAction
{
    None = 0,
    Solid = 1,
    Semi_Up = 2,
    Semi_Right = 3,
    Semi_Down = 4,
    Semi_Left = 5,
}

public enum Axis
{
    X = 0,
    Y = 1,
    Z = 2
}

public enum ActionState
{
    Hit = 1,
    Stay = 3,
    Leave = 4,
    None = 12
}

public enum Direction
{
    Right = 1,
    Left = 2,
    Up = 4,
    Down = 8
}

public enum BlockCollisionType
{
    Normal = 1,
    Sticky = 2,
    Slippery = 3,
    Bouncy = 4
}

public static class HelperMaps
{
    public static Dictionary<Direction, Vector2> DirectionVectorMap = new Dictionary<Direction, Vector2>()
    {
        [Direction.Up] = Vector2.up,
        [Direction.Left] = Vector2.left,
        [Direction.Right] = Vector2.right,
        [Direction.Down] = Vector2.down,
    };

    public static Dictionary<Direction, Vector2> DirectionPerpendicularMap = new Dictionary<Direction, Vector2>()
    {
        [Direction.Up] = Vector2.right,
        [Direction.Left] = Vector2.up,
        [Direction.Right] = Vector2.up,
        [Direction.Down] = Vector2.right,
    };

    public static bool IsState(object a, object b)
    {
        return ((int)a & (int)b) > 0;
    }
}