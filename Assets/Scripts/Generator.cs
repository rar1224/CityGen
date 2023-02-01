using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// points spawning on each other
// spawning perp points

public class Generator : MonoBehaviour
{
    public Road road;
    public Point point;

    public float contPreference;
    public float perpPreference;
    public float connectPreference;

    public float lower_range;
    public float upper_range;

    public int iterations;
    public float roadColliderRadius;

    public CentreShape shape = CentreShape.SQUARE;

    private int counter = 0;
    private bool generating = false;

    private int maxCount = 50;

    public GameObject model;

    private List<Point> points = new List<Point>();
    private List<Point> centre = new List<Point>();
    public List<Road> roads = new List<Road>();
    private List<Point> outsidePoints = new List<Point>();

    void Start()
    {
    }

    public void CreateCentre(CentreShape shape, float width = 20f, float height = 20f, float lineSmoothness = 0.8f)
    {
        switch(shape)
        {
            case CentreShape.TRIANGLE:
                TriangleCentre();
                break;
            case CentreShape.SQUARE:
                SquareCentre();
                break;
            case CentreShape.TETRAGON:
                TetragonCentre();
                break;
            case CentreShape.LINE:
                LineCentre(width, height, lineSmoothness);
                break;
        }
    }

    void TriangleCentre()
    {
        Point pt1 = SpawnAnyPoint(new Vector2(0, 0), upper_range, lower_range);
        points.Add(pt1);
        centre.Add(pt1);
        outsidePoints.Add(pt1);

        for (int i = 0; i < 2; i++)
        {
            Vector2 dir = Random.insideUnitCircle.normalized * Random.Range(lower_range, upper_range);
            Point pt = SpawnPoint(centre[i].transform.position.x + dir.x, centre[i].transform.position.y + dir.y);

            ConnectPoints(pt, centre[i]);
            points.Add(pt);
            centre.Add(pt);
            outsidePoints.Add(pt);
        }

        ConnectPoints(points[2], points[0]);
    }

    void TetragonCentre()
    {
        Point pt1 = SpawnAnyPoint(new Vector2(0, 0), upper_range, lower_range);
        points.Add(pt1);
        centre.Add(pt1);
        outsidePoints.Add(pt1);

        for (int i = 0; i < 3; i++)
        {
            Vector2 dir = Random.insideUnitCircle.normalized * Random.Range(lower_range, upper_range);
            Point pt = SpawnPoint(centre[i].transform.position.x + dir.x, centre[i].transform.position.y + dir.y);

            ConnectPoints(pt, centre[i]);
            points.Add(pt);
            centre.Add(pt);
            outsidePoints.Add(pt);
        }

        ConnectPoints(centre[3], centre[0]);
    }

    void SquareCentre()
    {
        float length = Random.Range(lower_range, upper_range);
        float pos = length / 2;
        centre.Add(SpawnPoint(-length/2, -length/2));
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

    void LineCentre(float width, float height, float lineSmoothness)
    {
        Point begin;
        Point end;
        int pointsBetween = Random.Range(2, 7);
        lineSmoothness = 1 - lineSmoothness;

        if (Random.Range(0, 2) == 0)
        {
            // vertical line

            begin = SpawnPoint(Random.Range(-width * lineSmoothness / 2, width * lineSmoothness / 2), height/2);
            end = SpawnPoint(Random.Range(-width * lineSmoothness / 2, width * lineSmoothness / 2), -height / 2);

            float distance = height / (pointsBetween + 1);

            centre.Add(begin);
            
            for (int i = 0; i < pointsBetween; i++)
            {
                centre.Add(SpawnPoint(Random.Range(-width * lineSmoothness / 2, width * lineSmoothness / 2), height / 2 - (i + 1) * distance));
            }

            centre.Add(end);
        } else
        {
            // horizontal line 

            begin = SpawnPoint(width/2, Random.Range(-height * lineSmoothness / 4, height * lineSmoothness / 4));
            end = SpawnPoint(-width / 2, Random.Range(-height * lineSmoothness / 4, height * lineSmoothness / 4));

            float distance = width / (pointsBetween + 1);

            centre.Add(begin);

            for (int i = 0; i < pointsBetween; i++)
            {
                centre.Add(SpawnPoint(width / 2 - (i + 1) * distance, Random.Range(-height * lineSmoothness / 4, height * lineSmoothness / 4)));
            }

            centre.Add(end);
        }

        for (int i = 0; i < 4; i++)
        {
            points.Add(centre[i]);
            outsidePoints.Add(centre[i]);
        }

        for (int i = 0; i < centre.Count - 1; i++)
        {
            ConnectPoints(centre[i], centre[i + 1]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (generating)
        {
            if (counter == iterations)
            {
                //bdGenerator.Initialize(this);
                counter++;
                generating = false;
                return;

            }

            if (outsidePoints.Count > 0)
            {
                // try connecting

                int ptsForConnecting = (int) (outsidePoints.Count * connectPreference);

                List<int> indices = new List<int>();
                for (int i = 0; i < ptsForConnecting; i++) {
                    int ind = Random.Range(0, outsidePoints.Count);
                    if (indices.Contains(ind))
                    {
                        i--;
                    } else
                    {
                        indices.Add(ind);
                    }
                }

                foreach(int j in indices)
                {
                    TryConnect(outsidePoints[j], contPreference, perpPreference);
                }

                // try making new points

                int index = Random.Range(0, outsidePoints.Count);
                Point pt = SpawnAndConnect(outsidePoints[index], contPreference, perpPreference);
                if (pt)
                {
                    points.Add(pt);
                    outsidePoints.Add(pt);
                }
            }
            
            counter++;
        }  
    }

    Point SpawnAndConnect(Point origin, float contPreference, float perpPreference)
    {
        Point pt = null;
        int counter = 0;

        while (pt == null)
        {
            if (counter > maxCount)
            {
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
                RaycastHit[] hits = Physics.RaycastAll(origin.transform.position, direction);

                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.tag == "Point")
                    {
                        Point chosen = hit.collider.gameObject.GetComponent<Point>();

                        if (chosen != origin && CheckConnectivity(origin, chosen))
                        {
                            ConnectPoints(origin, chosen);
                            return;
                        }

                        break;
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
                RaycastHit[] hits_left = Physics.RaycastAll(origin.transform.position, left);
                RaycastHit[] hits_right = Physics.RaycastAll(origin.transform.position, right);

                foreach (RaycastHit hit in hits_left)
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

                foreach (RaycastHit hit in hits_right)
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

    bool CheckPointHit(Point origin, RaycastHit hit, Vector2 direction)
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
        Point pt = (Point)Instantiate(point, new Vector3(x, y, -0.02f), Quaternion.identity);
        return pt;
    }

    Point SpawnAnyPoint(Vector2 origin_position, float upper_range, float lower_range)
    {
        float length = Random.Range(lower_range, upper_range);
        Vector2 vector = Random.insideUnitCircle.normalized;

        Vector2 pos = origin_position + vector * length;
        return SpawnPoint(pos.x, pos.y);
    }

    Point SpawnRandomPoint(Point origin)
    {
        Point chosen_point = SpawnAnyPoint(origin.transform.position, upper_range, lower_range);

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

    Road FindValidHit(RaycastHit hit, Point origin)
    {
        if (hit.collider.gameObject.tag == "Road")
        {
            Road matched_road = hit.collider.gameObject.GetComponent<Road>();
            if (matched_road.point1 != origin && matched_road.point2 != origin)
            {
                return matched_road;
            }
            return null;
        }
        return null;
    }


    // spawn parallel when possible and nothing in the way
    Point SpawnParallelPoint(Point origin, Road current_road, Point pt, Vector3 direction)
    {
        Vector2 left = Vector2.Perpendicular(direction);
        Vector2 right = -left;

        RaycastHit[] hit_left = Physics.RaycastAll(pt.transform.position, left);
        RaycastHit[] hit_right = Physics.RaycastAll(pt.transform.position, right);
        float distance_to_hit = 0;
        Road matched_road = null;

        foreach (RaycastHit hit in hit_left)
        {
            Road rd = FindValidHit(hit, origin);

            if (rd != null)
            {
                distance_to_hit = hit.distance;
                matched_road = rd;
            }
        }

        if (matched_road == null)
        {
            foreach (RaycastHit hit in hit_right)
            {
                Road rd = FindValidHit(hit, origin);

                if (rd != null)
                {
                    distance_to_hit = hit.distance;
                    matched_road = rd;
                }
            }
        }

        

        if (matched_road != null)
        {
            Destroy(pt.gameObject);
            distance_to_hit += roadColliderRadius;

            // get starting points of x and a
            Point start_current = current_road.GetOtherPoint(origin);
            Point start_other = matched_road.GetStartingPoint(direction);

            if (start_other != null)
            {

            // the other road - road x
            Vector2 road_x_vector = matched_road.GetOtherPoint(start_other).transform.position - start_other.transform.position;
            float road_x_mag = matched_road.GetMagnitude();

            // angle between starting points of roads x and a
            Vector2 angle_vector = origin.transform.position - start_other.transform.position;
            float angle = Mathf.Abs(Vector2.Angle(road_x_vector, angle_vector));

                // defining the difference (y) between current (a) and matched (x) roads
            float y;

            if (angle != 90 && angle != 0) y = -distance_to_hit / Mathf.Tan((Mathf.PI / 180) * angle);
            else y = 0;

                // calculating b
            float b = y + road_x_mag;

            Vector2 new_position = new Vector2(origin.transform.position.x, origin.transform.position.y) + road_x_vector.normalized * b;

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
        Vector2 new_dir;

        if (Random.Range(0, 2) == 0)
        {
            new_dir = Vector2.Perpendicular(direction);
        } else
        {
            new_dir = -Vector2.Perpendicular(direction);
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
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, directionVector);
        rotation.x = 0;

        Road connect = (Road)Instantiate(road, new Vector3(x, y, -0.02f), rotation);
        connect.transform.parent = model.transform;
        connect.transform.localScale = Vector3.Scale(connect.transform.localScale, new Vector3(1, directionVector.magnitude, 1));

        connect.point1 = point1;
        connect.point2 = point2;

        connect.SetTiling();

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
        // check if points are placed correctly
        if (point1 == point2)
        {
            return false;
        }
        if (!CheckPlacement(point2))
        {
            return false;
        }

        // check if there aren't too many lanes
        if (point1.connections.Count > 5 || point2.connections.Count > 5)
        {
            return false;
        }

        // check if the angle of road isn't too narrow
        Vector2 possibleRoad = point1.transform.position - point2.transform.position;

        foreach(Road rd in point1.connections)
        {
            if (Vector2.Angle(rd.GetVector(point1), -possibleRoad) < 20)
            {
                return false;
            }
        }

        foreach (Road rd in point2.connections)
        {
            if (Vector2.Angle(rd.GetVector(point2), possibleRoad) < 20)
            {
                return false;
            }
        }

        // check the possible length of road between
        Vector2 pointVector = point2.transform.position - point1.transform.position;
        if (pointVector.magnitude < lower_range || pointVector.magnitude > upper_range)
        {
            return false;
        }

        // check if possible roads don't cut across other roads
        Vector2 direction = (point2.transform.position - point1.transform.position).normalized;
        RaycastHit[] hits = Physics.RaycastAll(point1.transform.position, direction);

        foreach (Road road in roads)
        {
            if (!point1.connections.Contains(road) && !point2.connections.Contains(road))
            {
                foreach (RaycastHit hit in hits) {
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

    public void RestartParameters(float contPreference, float perpPreference, float connectPreference, float lower_range, float upper_range, int iterations)
    {
        this.contPreference = contPreference;
        this.perpPreference = perpPreference;
        this.connectPreference = connectPreference;
        this.lower_range = lower_range;
        this.upper_range = upper_range;
        this.iterations = iterations;
        generating = true;
    }

    public void RemoveModel()
    {
        //bdGenerator.RemoveBuildings();
        outsidePoints.Clear();
        centre.Clear();

        foreach (Road rd in roads)
        {
            Destroy(rd.gameObject);
        }

        foreach (Point pt in points)
        {
            Destroy(pt.gameObject);
        }

        roads.Clear();
        points.Clear();
    }

    public bool IsGenerating()
    {
        return generating;
    }

    public void StartOver()
    {
        counter = 0;
        generating = true;
        CreateCentre(shape);
    }

    public void Continue()
    {
        counter = 0;
        generating = true;
    }

    public enum CentreShape
    {
        TRIANGLE, TETRAGON, LINE, SQUARE
    }
}
