using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    public Point point1;
    public Point point2;
    private Material material;
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

    // get starting point for a chosen direction
    public Point GetStartingPoint(Vector3 vector)
    {
        if ((point2.transform.position - point1.transform.position).normalized == vector.normalized) {
            return point1;
        } else if ((point1.transform.position - point2.transform.position).normalized == vector.normalized) {
            return point2;
        } else
        {
            return null;
        }
    }

    public float GetMagnitude()
    {
        return GetDirection().magnitude;
    }

    public Vector2 GetVector(Point origin)
    {
        if (point1 == origin)
        {
            return point2.transform.position - point1.transform.position;
        }
        else
        {
            return point1.transform.position - point2.transform.position;
        }
    }

    public void SetTiling()
    {
        this.gameObject.GetComponent<Renderer>().material.mainTextureScale =
            new Vector2(1, GetMagnitude());
    }
}
