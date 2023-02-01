using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGenerator : MonoBehaviour
{
    public BuildingModelGenerator modelGenerator;
    public GameObject model;

    private List<Road> roads;
    private List<GameObject> buildings = new List<GameObject>();

    bool generating = false;
    int roadsNumber = 0;
    int roadCounter = 0;

    public void Initialize(List<Road> roads)
    {
        this.roads = roads;
        generating = true;  // no generating
        roadsNumber = roads.Count;
        roadCounter = 0;
    }

    private void Update()
    {
        if (generating && roadCounter < roadsNumber)
        {
            SpawnAlongRoad(roads[roadCounter], 0.05f, 0.5f);
            roadCounter++;
        } else if (roadCounter == roadsNumber)
        {
            generating = false;
        }
    }

    void SpawnAlongRoad(Road road, float averageDistance, float roadOffset)
    {
        float magnitude = road.GetDirection().magnitude;
        int bdNumber = (int)(magnitude / averageDistance);

        Vector2 offset = Vector2.Perpendicular(road.GetDirection().normalized);
        offset *= roadOffset;

        SpawnOneSide(bdNumber, offset, road, magnitude);
        SpawnOneSide(bdNumber, -offset, road, magnitude);

    }

    void SpawnOneSide(int bdNumber, Vector2 offset, Road rd, float magnitude)
    {
        for (int i = 0; i < bdNumber; i++)
        {
            Vector3 position = rd.point1.transform.position + (Vector3)rd.GetDirection().normalized * Random.Range(0.0f, 1.0f) * magnitude;
            Vector3 position1 = position + new Vector3(offset.x, offset.y, 0);

            if (IsPositionValid(position1))
            {
                GameObject bd = modelGenerator.CreateBuilding(position1, rd.transform.rotation);
                bd.transform.parent = model.transform;
                buildings.Add(bd);
            }
        }
    }

    bool IsPositionValid(Vector3 position)
    {
        Collider2D[] hitColliders2D = Physics2D.OverlapCircleAll(position, 0.15f);
        Collider[] hitColliders = Physics.OverlapSphere(position, 0.15f);

        if (hitColliders.Length > 0 || hitColliders2D.Length > 0)
        {
            return false;
        } else
        {
            return true;
        }
    }


    public void RemoveBuildings()
    {
        foreach (GameObject bd in buildings)
        {
            foreach (Transform child in bd.transform)
            {
                Destroy(child.gameObject);
            }
            Destroy(bd);
        }
        buildings.Clear();
        generating = false;
        
    }

    public bool IsGenerating()
    {
        return generating;
    }

    public void SetBdHeight(int bdHeight)
    {
        modelGenerator.SetBdHeight(bdHeight);
    }
}
