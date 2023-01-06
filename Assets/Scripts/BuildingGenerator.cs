using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGenerator : MonoBehaviour
{
    public Generator generator;
    public BuildingModelGenerator modelGenerator;

    private List<Road> roads;
    private List<Building> buildings = new List<Building>();

    public Building building;

    bool generating = false;
    int roadsNumber = 0;
    int roadCounter = 0;

    public void Initialize(Generator generator)
    {
        this.generator = generator;
        roads = generator.roads;
        generating = true;  // no generating
        roadsNumber = roads.Count;
        roadCounter = 0;
    }

    private void Update()
    {
        if (generating && roadCounter < roadsNumber)
        {
            SpawnAlongRoad(roads[roadCounter], 0.4f);
            roadCounter++;
        }
    }

    void SpawnAlongRoad(Road road, float distance)
    {
        float magnitude = road.GetDirection().magnitude;
        int bdNumber = (int)(magnitude / distance);

        Vector2 offset = Vector2.Perpendicular(road.GetDirection().normalized);
        offset = offset * 0.7f;

        for (int i = 0; i < bdNumber; i++)
        {
            Vector3 position = road.point1.transform.position + (Vector3) road.GetDirection().normalized * distance * (i+1);
            Vector3 position1 = position + new Vector3(offset.x, offset.y, 0);
            Vector3 position2 = position + new Vector3(-offset.x, -offset.y, 0);
            if (IsPositionValid(position1))
            {
                Building bd = modelGenerator.CreateBuilding(position1, road.transform.rotation);
                buildings.Add(bd);
            }
            if (IsPositionValid(position2))
            {
                Building bd = modelGenerator.CreateBuilding(position2, road.transform.rotation);
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
        foreach (Building bd in buildings)
        {
            foreach (Transform child in bd.transform)
            {
                Destroy(child.gameObject);
            }
            Destroy(bd.gameObject);
        }
        buildings.Clear();
        generating = false;
        
    }

    public bool IsGenerating()
    {
        return generating;
    }
}
