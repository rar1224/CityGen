using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGenerator : MonoBehaviour
{
    public Generator generator;
    private List<Road> roads;
    private List<Building> buildings = new List<Building>();

    public Building building;

    bool roadsDone = false;
    int roadsNumber = 0;
    int roadCounter = 0;

    public void Initialize(Generator generator)
    {
        this.generator = generator;
        roads = generator.roads;
        roadsDone = true;
        roadsNumber = roads.Count;
    }

    private void Update()
    {
        if (roadsDone && roadCounter < roadsNumber)
        {
            SpawnAlongRoad(roads[roadCounter], 0.4f);
            roadCounter++;
        }
    }

    void SpawnAlongRoad(Road road, float distance)
    {
        road.GetComponent<Renderer>().material.color = Color.green;
        float magnitude = road.GetDirection().magnitude;
        int bdNumber = (int)(magnitude / distance);

        Vector2 offset = Vector2.Perpendicular(road.GetDirection().normalized);
        offset = offset * 0.5f;

        for (int i = 0; i < bdNumber; i++)
        {
            Vector3 position = road.point1.transform.position + (Vector3) road.GetDirection().normalized * distance * (i+1);
            Vector3 position1 = position + new Vector3(offset.x, offset.y, 0);
            Vector3 position2 = position + new Vector3(-offset.x, -offset.y, 0);
            if (IsPositionValid(position1)) CreateBuilding(position1, road.transform.rotation);
            if (IsPositionValid(position2)) CreateBuilding(position2, road.transform.rotation);
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

    Building CreateBuilding(Vector3 position, Quaternion rotation)
    {
        Building bd;

        if (rotation.x > 0)
        {
            
            bd = (Building)Instantiate(building, position, rotation);

        } else
        {
            Quaternion correct = Quaternion.Euler(Vector3.right * 180);
            bd = (Building)Instantiate(building, position, rotation * correct);
        }
        
        bd.transform.localScale += new Vector3(2f, 2f, 2f);
        buildings.Add(bd);
        return bd;
    }
}
