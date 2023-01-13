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
        //int segmentNumber = 7;
        List<GameObject> groundFloor = new List<GameObject>();
        List<GameObject> allSegments = new List<GameObject>();

        // defining the floor plan
        for (int i = 0; i < segmentNumber; i++)
        {
            GameObject model;

            if (Random.Range(0.0f, 1.0f) > 0.9)
            {
                model = fiveSideModel;
            }
            else
            {
                model = fourSideModel;
            }

            if (groundFloor.Count == 0)
            {

                GameObject segment;

                if (model.tag == "FiveSideModel") segment = Instantiate(model, new Vector3(basicPosition.x, basicPosition.y, -0.2f), basicRotation);
                else segment = Instantiate(model, new Vector3(basicPosition.x, basicPosition.y, -0.1f), basicRotation);
                groundFloor.Add(segment);
                allSegments.Add(segment);

            }
            else
            {

                GameObject neighbor = groundFloor[Random.Range(0, groundFloor.Count)];
                Vector3 offset = Vector3.zero;
                Vector3 fixing_rotation = Vector3.zero;

                float modelDistance;
                if (model.tag == "FourSideModel") modelDistance = 0.1f;
                else modelDistance = 0.1f / Mathf.Tan((Mathf.PI / 180) * 36);

                if (neighbor.tag == "FourSideModel")
                {
                    // offseting other segments

                    int side = Random.Range(0, 3);

                    float distance = 0.1f + modelDistance;
                    if (model.tag == "FiveSideModel") fixing_rotation = new Vector3(0, 0, -36 - 90 * side); // one square, one pentagon
                    else fixing_rotation = new Vector3(0, 0, -90);                                               // two squares

                    Vector3 vector_rotation = new Vector3(0, 0, -90 * side);
                    offset = Quaternion.Euler(vector_rotation) * Vector3.right * distance;
                }
                else
                {
                    // offsetting pentagon

                    int side = Random.Range(0, 4);

                    float distance = 0.1f / Mathf.Tan((Mathf.PI / 180) * 36) + modelDistance;
                    if (model.tag == "FiveSideModel") fixing_rotation = new Vector3(0, 0, -36); // two pentagons
                    else fixing_rotation = new Vector3(0, 0, side * -72);                       // one pentagon, one square

                    Vector3 vector_rotation = new Vector3(0, 0, -72 * side);
                    offset = Quaternion.Euler(vector_rotation) * Vector3.right * distance;
                }

                GameObject segment = Instantiate(model, neighbor.transform.position, neighbor.transform.rotation);
                segment.transform.Translate(offset);
                segment.transform.Rotate(fixing_rotation);



                if (model.tag == "FourSideModel") segment.transform.position = new Vector3(segment.transform.position.x, segment.transform.position.y, -0.1f);
                else segment.transform.position = new Vector3(segment.transform.position.x, segment.transform.position.y, -0.2f);

                
                foreach (GameObject sg in groundFloor)
                {
                    if ((segment.transform.localPosition - neighbor.transform.localPosition).magnitude < 0.1f)
                    {
                        Destroy(segment);
                        continue;
                    }
                }
                

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
        
        

        Building currentBuilding = (Building)Instantiate(building, basicPosition, basicRotation);

        foreach (GameObject segment in allSegments)
        {
            segment.transform.parent = currentBuilding.transform;
        }

        return currentBuilding;
    }
}
