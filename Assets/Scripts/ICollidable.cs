using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollidable
{
    void CollisionEnter(Entity other, Direction dir);
    void CollisionStay(Entity other, Direction dir);
    void CollisionExit(Entity other, Direction dir);
}
