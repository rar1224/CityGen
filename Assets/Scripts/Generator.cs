using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// points spawning on each other
// spawning perp points

public class Generator : MonoBehaviour
{
    public Road road;
    public Point point;

    private int counter = 0;

    private List<Point> points = new List<Point>();
    private List<Point> centre = new List<Point>();
    private List<Road> roads = new List<Road>();

    private List<Point> outsidePoints = new List<Point>();

    // Start is called before the first frame update
    void Start()
    {
        CreateCentre(CentreShape.TETRA);
    }

    void CreateCentre(CentreShape shape, int angles = 0, float pref = 1)
    {
        switch(shape)
        {
            case CentreShape.TRIANGLE:
                TriangleCentre();
                break;
            case CentreShape.TETRA:
                TetraCentre(pref);
                break;
            case CentreShape.ROUND:
                RoundCentre(angles, pref);
                break;
        }
    }

    void TriangleCentre()
    {
        for (int i = 0; i < 3; i++)
        {
            Point point = SpawnAnyPoint(new Vector2(0, 0), 3.0f);
            points.Add(point);
            centre.Add(point);
            outsidePoints.Add(point);
        }

        ConnectPoints(points[0], points[1]);
        ConnectPoints(points[1], points[2]);
        ConnectPoints(points[2], points[0]);
    }

    void TetraCentre(float squarePreference)
    {
        squarePreference += 1;
        float scale = Random.Range(0.3f, 1.0f);
        
        centre.Add(SpawnPoint(Random.Range(-2f, -1f * squarePreference) * scale,
                            Random.Range(1f * squarePreference, 2f) * scale));

        centre.Add(SpawnPoint(Random.Range(1f * squarePreference, 2f) * scale,
                            Random.Range(1f * squarePreference, 2f) * scale));

        centre.Add(SpawnPoint(Random.Range(1f * squarePreference, 2f) * scale,
                            Random.Range(-2f, -1f * squarePreference) * scale));

        centre.Add(SpawnPoint(Random.Range(-2f, -1f * squarePreference) * scale,
                            Random.Range(-2f, -1f * squarePreference) * scale));

        for (int i = 0; i < 4; i++)
        {
            points.Add(centre[i]);
            outsidePoints.Add(centre[i]);
        }

        for (int i = 0; i < 3; i++)
        {
            ConnectPoints(centre[i], centre[i + 1]);
        }

        ConnectPoints(centre[3], centre[0]);
    }

    void RoundCentre(int angles, float regularPreference)
    {

    }

    // Update is called once per frame
    void Update()
    {
        float contPreference = 1f;
        float perpPreference = 1f;
            
            int index = Random.Range(0, outsidePoints.Count);

            if (counter%2 == 1)
            {
                foreach (Point outp in outsidePoints)
                {
                    TryConnect(outp, contPreference, perpPreference);
                }
            }
            
            Point pt = SpawnAndConnect(outsidePoints[index], contPreference, perpPreference);
            if (pt)
            {
                points.Add(pt);
                outsidePoints.Add(pt);
            }
            counter++;
    }

    Point SpawnAndConnect(Point origin, float contPreference, float perpPreference)
    {
        Point pt = null;
        int counter = 0;

        while (pt == null)
        {
            if (counter > 20)
            {
                outsidePoints.Remove(origin);
                return null;
            }

            bool cont = contPreference > Random.Range(0.0f, 1.0f);

            if (cont) pt = SpawnContinuousPoint(origin, perpPreference);
            else pt = SpawnRandomPoint(origin);

            counter++;
        }

        return pt;
    }

    void TryConnect(Point origin, float contPreference, float perpPreference)
    {
        // scan for continuous points
        if (Random.Range(0.0f, 1.0f) < contPreference)
        {
            foreach (Road road in origin.connections)
            {
                Point other = road.GetOtherPoint(origin);
                Vector2 direction = origin.transform.position - other.transform.position;
                RaycastHit2D hit = Physics2D.Raycast(origin.transform.position, direction);

                if (hit.collider.tag == "Point")
                {
                    Point chosen = hit.collider.gameObject.GetComponent<Point>();

                    if (CheckConnectivity(origin, chosen))
                    {
                        ConnectPoints(origin, chosen);
                        return;
                    }
                }
            }
            
        }
        // scan for perpendicular points
        if (Random.Range(0.0f, 1.0f) < perpPreference)
        {
            foreach (Road road in origin.connections)
            {
                Point other = road.GetOtherPoint(origin);
                Vector2 left = Vector2.Perpendicular(origin.transform.position - other.transform.position);
                Vector2 right = -left;
                RaycastHit2D[] hits_left = Physics2D.RaycastAll(origin.transform.position, left);
                RaycastHit2D[] hits_right = Physics2D.RaycastAll(origin.transform.position, right);

                foreach (RaycastHit2D hit in hits_left)
                {
                    if (hit.collider.tag == "Road")
                    {
                        continue;
                    }

                    Point chosen = hit.collider.gameObject.GetComponent<Point>();

                    if (hit.collider.tag == "Point")
                    {
                        // check if point is valid
                        if (origin == chosen)
                        {
                            continue;
                        } else if (origin.IsConnectedTo(chosen))
                        {
                            break;
                        }

                        // check if possible to connect, otherwise leave the loop
                        if (CheckConnectivity(origin, chosen))
                        {
                            ConnectPoints(origin, chosen);
                            return;
                        } else
                        {
                            break;
                        }
                    }
                }

                foreach (RaycastHit2D hit in hits_right)
                {
                    if (hit.collider.tag == "Road")
                    {
                        continue;
                    }

                    Point chosen = hit.collider.gameObject.GetComponent<Point>();

                    if (hit.collider.tag == "Point" && !origin.IsConnectedTo(chosen))
                    {
                        if (CheckConnectivity(origin, chosen))
                        {
                            ConnectPoints(origin, chosen);
                            return;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return;
        }

        foreach (Point pt in outsidePoints)
        {
            if (CheckConnectivity(origin, pt))
            {
                ConnectPoints(origin, pt);
                return;
            }
        }
    }

    Point SpawnPoint(float x, float y)
    {
        Point pt = (Point)Instantiate(point, new Vector3(x, y, 0), Quaternion.identity);
        return pt;
    }

    Point SpawnAnyPoint(Vector2 origin_position, float range)
    {
        float x = Random.Range(origin_position.x - range, origin_position.x + range);
        float y = Random.Range(origin_position.x - range, origin_position.y + range);
        Point pt = (Point) Instantiate(point, new Vector3(x, y, 0), Quaternion.identity);
        return pt;
    }

    Point SpawnRandomPoint(Point origin)
    {
        Point chosen_point = SpawnAnyPoint(origin.transform.position, 5.0f);

        if (CheckConnectivity(origin, chosen_point))
        {
            ConnectPoints(origin, chosen_point);
        }
        else
        {
            Destroy(chosen_point.gameObject);
            return null;
        }

        return chosen_point;
    }

    Point SpawnContinuousPoint(Point origin, float perpPreference)
    {
        int roads_number = origin.connections.Count;
        Road road = origin.connections[Random.Range(0, roads_number)];

        bool perpendicular = false;

        if (Random.Range(0.0f, 1.0f) < perpPreference)
        {
            perpendicular = true;
        }

        if (road != null)
        {
            Vector3 direction = (origin.transform.position - road.transform.position).normalized;
            Vector2 point_position = origin.transform.position + direction * Random.Range(0.0f, 3.0f);

            Point pt = (Point)Instantiate(point, point_position, Quaternion.identity);

            if(!CheckConnectivity(origin, pt))
            {
                Destroy(pt.gameObject);
                return null;
            }

            // perpendicular
            if (perpendicular)
            {
                // spawn parallel point - available for perpendicular connecting
                Point parallel = SpawnParallelPoint(origin, pt, direction);
                if (parallel != null) {
                    Destroy(pt.gameObject);
                    pt = parallel;
                } 
                else
                {
                    // spawn perpendicular point if possible
                    Point perp = SpawnPerpendicularPoint(origin, direction);
                    if (perp != null)
                    {
                        Destroy(pt.gameObject);
                        pt = perp;
                    }
                }
            } 

            ConnectPoints(origin, pt);
            return pt;
            
        } else return null;
        
    }

    // spawn parallel when possible and nothing in the way
    Point SpawnParallelPoint(Point origin, Point pt, Vector3 direction)
    {
        Vector2 left = Vector2.Perpendicular(direction);
        Vector2 right = -left;

        RaycastHit2D[] hit_left = Physics2D.RaycastAll(pt.transform.position, left);
        RaycastHit2D[] hit_right = Physics2D.RaycastAll(pt.transform.position, right);
        Road matched_road = null;
        bool set = false;

        foreach (RaycastHit2D hit in hit_left)
        {
            if (hit.collider.gameObject.tag == "Road")
            {
                matched_road = hit.collider.gameObject.GetComponent<Road>();
                set = true;
                break;
            }
        }

        if (!set)
        {
            foreach (RaycastHit2D hit in hit_right)
            {
                if (hit.collider.gameObject.tag == "Road")
                {
                    set = true;
                    matched_road = hit.collider.gameObject.GetComponent<Road>();
                    break;
                }
            }
        }

        if (set)
        {
            Vector2 road_vector = matched_road.point1.transform.position - matched_road.point2.transform.position;
            float other_mag = road_vector.magnitude;
            Vector2 new_position = origin.transform.position + direction * other_mag;

            Destroy(pt.gameObject);
            Point pt2 = (Point)Instantiate(point, new_position, Quaternion.identity);

            if (!CheckConnectivity(origin, pt2))
            {
                Destroy(pt2.gameObject);
                return null;
            }
            else
            {
                ConnectPoints(origin, pt2);
                return pt2;
            }
        } else
        {
            return null;
        }
    }

    Point SpawnPerpendicularPoint(Point origin, Vector3 direction)
    {
        Vector2 left = Vector2.Perpendicular(direction);
        Vector2 right = -left;
        Vector2 new_dir;

        if (Random.Range(0, 2) == 0)
        {
            new_dir = left;
        } else
        {
            new_dir = right;
        }

        Vector2 point_position = new Vector2(origin.transform.position.x, origin.transform.position.y) + new_dir * Random.Range(0.0f, 3.0f);
        Point pt = (Point)Instantiate(point, point_position, Quaternion.identity);

        if (!CheckConnectivity(origin, pt))
        {
            Destroy(pt.gameObject);
            return null;
        } else
        {
            ConnectPoints(origin, pt);
            return pt;
        }
    }
    Road CreateRoad(Point point1, Point point2)
    {
        float x = (point1.transform.position.x + point2.transform.position.x) / 2;
        float y = (point1.transform.position.y + point2.transform.position.y) / 2;
        Vector2 directionVector = point1.transform.position - point2.transform.position;
        Road connect = (Road)Instantiate(road, new Vector3(x, y, 0), Quaternion.FromToRotation(Vector3.up, directionVector));
        connect.transform.localScale += new Vector3(0, directionVector.magnitude - 1, 0);

        connect.point1 = point1;
        connect.point2 = point2;

        return connect;
    }

    void ConnectPoints(Point point1, Point point2)
    {
        if (point1 == null || point2 == null)
        {
            throw new System.Exception();
        }

        Road road = CreateRoad(point1, point2);

        point1.connections.Add(road);
        point2.connections.Add(road);
        roads.Add(road);
    }

    public bool CheckConnectivity(Point point1, Point point2)
    {
        Vector2 direction = (point2.transform.position - point1.transform.position).normalized;
        RaycastHit2D[] hits = Physics2D.RaycastAll(point1.transform.position, direction);

        foreach (Road road in roads)
        {
            if (!point1.connections.Contains(road) && !point2.connections.Contains(road))
            {
                foreach (RaycastHit2D hit in hits) {
                    if (hit.collider.gameObject == road.gameObject)
                    {
                        return false;
                    }
                }
            } else if (point1.connections.Contains(road))
            {
                Vector2 road_dir = (road.transform.position - point1.transform.position).normalized;

                if (direction == road_dir)
                {
                    return false;
                }
            }
        }

        return true;
    }

    enum CentreShape
    {
        TRIANGLE, TETRA, ROUND
    }
}
