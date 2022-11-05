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
        roadsDone = true;
    }

    private void Update()
    {
        if (roadsDone)
        {
            SpawnAlongRoad(roads[0], 0.4f);
            roadsDone = false;
        }
    }

    void SpawnAlongRoad(Road road, float distance)
    {
        road.GetComponent<Renderer>().material.color = Color.green;
        float magnitude = road.GetDirection().magnitude;
        int bdNumber = (int)(magnitude / distance);

        for (int i = 0; i < bdNumber; i++)
        {
            Vector3 position = road.point1.transform.position + (Vector3) road.GetDirection().normalized * distance * i;
            CreateBuilding(position, Quaternion.identity);
        }

    }

    Building CreateBuilding(Vector3 position, Quaternion rotation)
    {
        Building bd = (Building)Instantiate(building, position, rotation);
        return bd;
    }
}
