using System.Collections.Generic;
using UnityEngine;

public class CollisionsList
{
    struct Collision
    {
        public Transform obj1;
        public Transform obj2;
    }

    List<Collision> collisions = new List<Collision>();

    public void AddCollision(Transform obj1, Transform obj2)
    {
        if (!obj1 || !obj2)
        {
            Debug.Assert(obj1 && obj2);
            return;
        }
        Collision coll;
        coll.obj1 = obj1;
        coll.obj2 = obj2;
        collisions.Add(coll);
    }

    public bool HasCollision(Transform obj1, Transform obj2)
    {
        if (!obj1 || !obj2)
        {
            Debug.Assert(obj1 && obj2);
            return false;
        }

        foreach (Collision coll in collisions)
        {
            if ((coll.obj1 == obj1 && coll.obj2 == obj2) || (coll.obj1 == obj2 && coll.obj2 == obj1))
                return true;
        }

        return false;
    }

    public void Clear()
    {
        collisions.Clear();
    }
}
