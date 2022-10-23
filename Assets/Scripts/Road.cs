using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    public Point point1;
    public Point point2;
    private void Awake()
    {
        
    }

    public Vector2 GetDirection()
    {
        return point2.transform.position - point1.transform.position;
    }

    public Point GetOtherPoint(Point current)
    {
        if (point1 == current) return point2;
        else if (point2 == current) return point1;
        else return null;
    }
}
