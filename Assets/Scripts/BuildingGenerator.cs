using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGenerator : MonoBehaviour
{
    public Generator generator;
    private List<Road> roads;

    public Building building;

    bool roadsDone = false;

    public void Initialize(Generator generator)
    {
        this.generator = generator;
        roads = generator.roads;
        roadsDone = false;
    }

    private void Update()
    {
        if (roadsDone)
        {
            foreach (Road rd in roads)
            {
                SpawnAlongRoad(rd, 0.4f);
            }
            roadsDone = false;
        }
    }

    void SpawnAlongRoad(Road road, float distance)
    {
        road.GetComponent<Renderer>().material.color = Color.green;
        float magnitude = road.GetDirection().magnitude;
        int bdNumber = (int)(magnitude / distance);

        Vector2 offset = Vector2.Perpendicular(road.GetDirection().normalized);
        offset = offset * 0.2f;

        for (int i = 0; i < bdNumber; i++)
        {
            Vector3 position = road.point1.transform.position + (Vector3) road.GetDirection().normalized * distance * (i+1);
            Vector3 position1 = position + new Vector3(offset.x, offset.y, 0);
            Vector3 position2 = position + new Vector3(-offset.x, -offset.y, 0);
            CreateBuilding(position1, Quaternion.identity);
            CreateBuilding(position2, Quaternion.identity);
        }

    }

    Building CreateBuilding(Vector3 position, Quaternion rotation)
    {
        Building bd = (Building)Instantiate(building, position, rotation);
        return bd;
    }
}
