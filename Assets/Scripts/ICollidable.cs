using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollidable
{
    void CollisionEnter(GameObject other, Direction dir);
    void CollisionStay(GameObject other, Direction dir);
    void CollisionExit(GameObject other, Direction dir);
}
