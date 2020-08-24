using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct serializableVector3
{
    public float x;
    public float y;
    public float z;

    public Vector3 GetPos()
    {
        return new Vector3 ( x, y, z );
    }
}

[DefaultExecutionOrder ( -110)]
[System.Serializable]
public class Stats
{
    public int lives = 4;
    public float health = 1f;
    public int wealth = 0;
    public int weapons = 1;
    public int level = 1;
    //public serializableVector3 myPos;

}
