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
            GameObject model;

            if (Random.Range(0f, 1f) > 0.5)
            {
                model = fiveSideModel;
            } else
            {
                model = fourSideModel;
            }

            if (groundFloor.Count == 0)
            {
                GameObject segment;

                if (model == fourSideModel) {
                    Quaternion rotation = Quaternion.Euler(-90, 0, 0);
                    segment = Instantiate(model, basicPosition, basicRotation * rotation);
                } else
                {
                    segment = Instantiate(model, basicPosition, basicRotation);
                }

                groundFloor.Add(segment);
                allSegments.Add(segment);
            } else
            {
                GameObject neighbor = groundFloor[Random.Range(0, groundFloor.Count)];
                Vector3 offset = Vector3.zero;
                Vector3 fixing_rotation = Vector3.zero;

                if (model.tag == "FourSideModel")
                {
                    // offseting other segments

                    int side = Random.Range(0, 3);
                    //Quaternion rotation = Quaternion.Euler(0, 0, 90 * side);
                    

                    switch (side)
                    {
                        case 0:
                            offset = new Vector3(0, -0.2f, 0);
                            break;
                        case 1:
                            offset = new Vector3(-0.2f, 0, 0);
                            break;
                        case 2:
                            offset = new Vector3(0, 0.2f, 0);
                            break;
                        case 3:
                            offset = new Vector3(0.2f, 0, 0);
                            break;
                    }
                } else
                {
                    // offsetting pentagon

                    int side = Random.Range(0, 4);

                    float distance = 0.2f / Mathf.Tan((Mathf.PI / 180) * 36);
                    fixing_rotation = new Vector3(0, 0, -36);
                    Vector3 vector_rotation = new Vector3(0, 0, -72 * side);
                    offset = Quaternion.Euler(vector_rotation) * Vector3.right * distance;
                }
               

                if (Physics.Raycast(neighbor.transform.position, offset, 0.3f))
                {
                    continue;
                }

                    // fix rotation
                GameObject segment = Instantiate(model, neighbor.transform.position, neighbor.transform.rotation);
                segment.transform.Translate(offset);
                segment.transform.Rotate(fixing_rotation);
                groundFloor.Add(segment);
                allSegments.Add(segment);
            }
        }

        
        // height of segments

        /*
        foreach (GameObject groundSegment in groundFloor) {
            int height = Random.Range(1, 4);

            for (int i = 0; i < height; i++)
            {
                GameObject newFloor = Instantiate(groundSegment, groundSegment.transform.position + new Vector3(0, 0, -0.2f * (i + 1)), groundSegment.transform.rotation);
                allSegments.Add(newFloor);
            }
        }
        */
        

        Building currentBuilding = (Building) Instantiate(building, basicPosition, basicRotation);

        foreach(GameObject segment in allSegments)
        {
            segment.transform.parent = currentBuilding.transform;
        }

        return currentBuilding;
    }
}
