using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    public List<Road> connections = new List<Road>();

    public bool IsConnectedTo(Point other)
    {
        foreach (Road road in connections)
        {
            if (road.GetOtherPoint(this) == other)
            {
                return true;
            }
        }

        return false;
    }
}
