using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ReplayFrame
{
    public Vector3 position;
    public Vector3 rotationEuler;   // in degrees

    public ReplayFrame(Vector3 pos, Quaternion rot)
    {
        position = pos;
        rotationEuler = rot.eulerAngles;
    }
}

[System.Serializable]
public class ReplayData
{
    public float frameRate = 30f;                 // frames per second
    public Vector3 startForward;                  // player's forward at start
    public List<ReplayFrame> frames = new List<ReplayFrame>();
}