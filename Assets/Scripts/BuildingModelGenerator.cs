using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingModelGenerator : MonoBehaviour
{
    public GameObject fourSideModel;
    public GameObject fiveSideModel;
    public Building building;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Building CreateBuilding(Vector3 basicPosition, Quaternion basicRotation)
    {
        // number of segments on the ground floor
        int segmentNumber = Random.Range(1, 5);
        List<GameObject> groundFloor = new List<GameObject>();
        List<GameObject> allSegments = new List<GameObject>();

        // defining the floor plan
        for (int i = 0; i < segmentNumber; i++)
        {
            GameObject model = null;

            if (Random.Range(0f, 1f) > 0.95)
            {
                model = fiveSideModel;
            } else
            {
                model = fourSideModel;
            }

            if (groundFloor.Count == 0)
            {
                Quaternion rotation = Quaternion.Euler(-90, 0, 0);
                GameObject segment = Instantiate(model, basicPosition, basicRotation * rotation);
                groundFloor.Add(segment);
                allSegments.Add(segment);
            } else
            {
                GameObject neighbor = groundFloor[Random.Range(0, groundFloor.Count)];
                int side = Random.Range(0, 3);
                Quaternion rotation = Quaternion.Euler(-90, 0, 90 * side);
                Quaternion neighbor_rotation = Quaternion.Euler(0, 0, neighbor.transform.rotation.z);
                Vector3 offset = new Vector3();

                switch(side)
                {
                    case 0:
                        offset = new Vector3(0, -0.1f, 0);
                        break;
                    case 1:
                        offset = new Vector3(-0.1f, 0, 0);
                        break;
                    case 2:
                        offset = new Vector3(0, 0.1f, 0);
                        break;
                    case 3:
                        offset = new Vector3(0.1f, 0, 0);
                        break;
                }

                // fix rotation
                GameObject segment = Instantiate(model, neighbor.transform.position + offset, rotation * neighbor_rotation);
                groundFloor.Add(segment);
                allSegments.Add(segment);
            }
        }

        // height of segments
        foreach (GameObject groundSegment in groundFloor) {
            int height = Random.Range(1, 4);

            for (int i = 0; i < height; i++)
            {
                GameObject newFloor = Instantiate(groundSegment, groundSegment.transform.position + new Vector3(0, 0, -0.2f * (i + 1)), groundSegment.transform.rotation);
                allSegments.Add(newFloor);
            }
        }

        Building currentBuilding = (Building) Instantiate(building, basicPosition, basicRotation);

        foreach(GameObject segment in allSegments)
        {
            segment.transform.parent = currentBuilding.transform;
        }

        return currentBuilding;
    }
}
