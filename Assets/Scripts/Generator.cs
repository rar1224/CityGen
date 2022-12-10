using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// points spawning on each other
// spawning perp points

public class Generator : MonoBehaviour
{
    public Road road;
    public Point point;
    public BuildingGenerator bdGenerator;

    public float contPreference = 1;
    public float perpPreference = 1;

    public float lower_range = 3;
    public float upper_range = 3;

    public int iterations = 10;
    public float roadColliderRadius = 0.15f;


    private int counter = 0;

    private List<Point> points = new List<Point>();
    private List<Point> centre = new List<Point>();
    public List<Road> roads = new List<Road>();

    private List<Point> outsidePoints = new List<Point>();

    // Start is called before the first frame update
    void Start()
    {
        CreateCentre(CentreShape.SQUARE, 3f);
    }

    void CreateCentre(CentreShape shape, float pref = 1, int angles = 0)
    {
        switch(shape)
        {
            case CentreShape.TRIANGLE:
                TriangleCentre();
                break;
            case CentreShape.SQUARE:
                SquareCentre(pref);
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

    void SquareCentre(float length)
    {
        float pos = length / 2;
        centre.Add(SpawnPoint(-pos, -pos));
        centre.Add(SpawnPoint(-pos, pos));
        centre.Add(SpawnPoint(pos, pos));
        centre.Add(SpawnPoint(pos, -pos));

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
      if (counter == iterations)
      {
            bdGenerator.Initialize(this);
            counter++;
            return;

      } else if (counter > iterations)
      {
            return;
      }

            int index = Random.Range(0, outsidePoints.Count);

            foreach (Point outp in outsidePoints)
            {
                TryConnect(outp, contPreference, perpPreference);
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
            if (counter > 50)
            {
                origin.gameObject.GetComponent<Renderer>().material.color = Color.red;
                outsidePoints.Remove(origin);
                return null;
            }

            bool cont = contPreference > Random.Range(0.0f, 1.0f);

            if (cont) pt = SpawnContinuousPoint(origin, perpPreference, lower_range, upper_range);
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
                    Vector2 vector = chosen.transform.position - origin.transform.position;

                    if (chosen != origin && CheckConnectivity(origin, chosen))
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
                    if (CheckPointHit(origin, hit, left))
                    {
                        if (CheckConnectivity(origin, hit.collider.gameObject.GetComponent<Point>()))
                        {
                            ConnectPoints(origin, hit.collider.gameObject.GetComponent<Point>());
                            return;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                foreach (RaycastHit2D hit in hits_right)
                {
                    if (CheckPointHit(origin, hit, right))
                    {
                        if (CheckConnectivity(origin, hit.collider.gameObject.GetComponent<Point>()))
                        {
                            ConnectPoints(origin, hit.collider.gameObject.GetComponent<Point>());
                            return;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            return;

        } else
        {
            Point pt = outsidePoints[Random.Range(0, outsidePoints.Count)];
           if (CheckConnectivity(origin, pt))
           {
                    ConnectPoints(origin, pt);
                    return;
           }
        }
    }

    bool CheckPointHit(Point origin, RaycastHit2D hit, Vector2 direction)
    {
        if (hit.collider.tag == "Road")
        {
            return false;
        } else
        {
            if (hit.collider.tag == "Point")
            {
                Point hitPoint = hit.collider.gameObject.GetComponent<Point>();
                Vector2 vector = hitPoint.transform.position - origin.transform.position;

                if (hitPoint == origin || hitPoint.IsConnectedTo(origin))
                {
                    return false;
                } else
                {
                    return true;
                }
            } else
            {
                return false;
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
        return SpawnPoint(x, y);
    }

    Point SpawnRandomPoint(Point origin)
    {
        Point chosen_point = SpawnAnyPoint(origin.transform.position, 3.0f);

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

    Point SpawnContinuousPoint(Point origin, float perpPreference, float lower_range, float upper_range)
    {
        int roads_number = origin.connections.Count;
        Road current_road = origin.connections[Random.Range(0, roads_number)];

        bool perpendicular = false;

        if (Random.Range(0.0f, 1.0f) < perpPreference)
        {
            perpendicular = true;
        }

        if (current_road != null)
        {
            Vector3 direction = (origin.transform.position - current_road.transform.position).normalized;
            Vector2 point_position = origin.transform.position + direction * Random.Range(lower_range, upper_range);

            Point regular_point = SpawnPoint(point_position.x, point_position.y);

            // perpendicular
            if (perpendicular)
            {
                // spawn parallel point - available for perpendicular connecting
                Point parallel = SpawnParallelPoint(origin, current_road, regular_point, direction);
                if (parallel != null) {
                    Destroy(regular_point.gameObject);
                    ConnectPoints(origin, parallel);
                    return parallel;
                } else {
                    // spawn perpendicular point if possible
                    Point perp = SpawnPerpendicularPoint(origin, direction, lower_range, upper_range);
                    if (perp != null)
                    {
                        Destroy(regular_point.gameObject);
                        ConnectPoints(origin, perp);
                        return perp;
                    } else
                    {
                        Destroy(regular_point.gameObject);
                        return null;
                    }
                }
            } else {
                if (!CheckConnectivity(origin, regular_point))
                {
                    Destroy(regular_point.gameObject);
                    return null;
                }
                ConnectPoints(origin, regular_point);
                return regular_point;
            }

            
            
        } else return null;
        
    }

    // spawn parallel when possible and nothing in the way
    Point SpawnParallelPoint(Point origin, Road current_road, Point pt, Vector3 direction)
    {
        Vector2 left = Vector2.Perpendicular(direction);
        Vector2 right = -left;

        RaycastHit2D[] hit_left = Physics2D.RaycastAll(pt.transform.position, left);
        RaycastHit2D[] hit_right = Physics2D.RaycastAll(pt.transform.position, right);
        float distance_to_hit = 0;
        Road matched_road = null;
        bool set = false;

        foreach (RaycastHit2D hit in hit_left)
        {
            if (hit.collider.gameObject.tag == "Road")
            {
                matched_road = hit.collider.gameObject.GetComponent<Road>();
                if (matched_road.point1 != origin && matched_road.point2 != origin)
                {
                    distance_to_hit = hit.distance;
                    set = true;
                }
                break;
            }
        }

        if (!set)
        {
            foreach (RaycastHit2D hit in hit_right)
            {
                if (hit.collider.gameObject.tag == "Road")
                {
                    matched_road = hit.collider.gameObject.GetComponent<Road>();
                    if (matched_road.point1 != origin && matched_road.point2 != origin)
                    {
                        distance_to_hit = hit.distance;
                        set = true;
                    }
                    break;
                }
            }
        }

        if (set)
        {
            Destroy(pt.gameObject);
            distance_to_hit += roadColliderRadius;

            // get starting points of x and a
            Point start_current = current_road.GetOtherPoint(origin);
            Point start_other = matched_road.GetStartingPoint(direction);

            if (start_other != null)
            {

            // the other road - road x
            Vector2 road_vector = matched_road.GetOtherPoint(start_other).transform.position - start_other.transform.position;
            float other_mag = Vector3.Distance(matched_road.point1.transform.position, matched_road.point2.transform.position);

            // the current road - road a
            float current_mag = current_road.GetMagnitude();

            // angle between starting points of roads x and a
                Vector2 angle_vector = start_current.transform.position - start_other.transform.position;
                float angle = Vector2.Angle(road_vector, angle_vector);

                // defining the difference (y) between current (a) and matched (x) roads
                float y;

                if (angle > 90)
                {
                    // matched road is longer
                    angle = 90 - angle;
                    y = distance_to_hit * Mathf.Tan(angle);
                }
                else if (angle == 0 || angle == 90)
                {
                    y = 0;
                }
                else
                {
                    y = -distance_to_hit * Mathf.Tan(angle);
                }

                // calculating b
                float b = y + other_mag - current_mag;


                float angle2 = Vector2.Angle(direction, road_vector);
                //Vector2 new_position = origin.transform.position + direction.normalized * (other_mag * Mathf.Cos(angle2));
                Vector2 new_position = origin.transform.position + direction.normalized * b;

                Point pt2 = SpawnPoint(new_position.x, new_position.y);

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
 
        } else
        {
            Destroy(pt.gameObject);
            return null;
        }
    }

    Point SpawnPerpendicularPoint(Point origin, Vector3 direction, float lower_range, float upper_range)
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

        Vector2 point_position = new Vector2(origin.transform.position.x, origin.transform.position.y) + new_dir * Random.Range(lower_range, upper_range);
        Point pt = SpawnPoint(point_position.x, point_position.y);

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
        if (point1.connections.Count > 5 || point2.connections.Count > 5)
        {
            return false;
        }

        // check the possible length of road between
        Vector2 pointVector = point2.transform.position - point1.transform.position;
        if (pointVector.magnitude < lower_range || pointVector.magnitude > upper_range)
        {
            return false;
        }

        // check if points are placed correctly
        if (point1 == point2)
        {
            return false;
        }
        if (!CheckPlacement(point2))
        {
            return false;
        }

        // check if possible roads don't cut across other roads
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
                if (point2.connections.Contains(road))
                {
                    return false;
                }

                Vector2 road_dir = (road.transform.position - point1.transform.position).normalized;

                if (direction == road_dir)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public bool CheckPlacement(Point pt)
    {
        foreach (Point other in points)
        {
            if (other == pt)
            {
                continue;
            }
            if (Vector3.Distance(pt.transform.position, other.transform.position) < 0.5)
            {
                return false;
            }
        }

        return true;
    }

    enum CentreShape
    {
        TRIANGLE, TETRA, ROUND, SQUARE
    }
}
