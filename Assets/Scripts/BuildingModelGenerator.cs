using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingModelGenerator : MonoBehaviour
{
    public GameObject fourSideModel;
    public GameObject fiveSideModel;

    // Start is called before the first frame update
    void Start()
    {
        CreateBuilding();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateBuilding()
    {
        // number of segments on the ground floor
        int segmentNumber = Random.Range(1, 5);
        List<GameObject> groundFloor = new List<GameObject>();

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
                GameObject segment = Instantiate(model, new Vector3(0, 0, 0), Quaternion.identity);
                groundFloor.Add(segment);
            } else
            {
                // fix
                GameObject neighbor = groundFloor[Random.Range(0, segmentNumber - 1)];
                int side = Random.Range(0, 3);
                Quaternion rotation = Quaternion.Euler(0, 0, 90 * side);
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
                GameObject segment = Instantiate(model, neighbor.transform.position + offset, Quaternion.identity);
                groundFloor.Add(segment);
            }
        }

        foreach (GameObject groundSegment in groundFloor) {
            int height = Random.Range(1, 4);

            for (int i = 0; i < height; i++)
            {
                GameObject newFloor = Instantiate(groundSegment, groundSegment.transform.position + new Vector3(0, 0, -0.1f * (i + 1)), groundSegment.transform.rotation);
            }
        }
        // height of segments
    }
}
