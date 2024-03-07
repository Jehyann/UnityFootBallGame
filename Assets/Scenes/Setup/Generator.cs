using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Generator : MonoBehaviour
{
    [SerializeField] private Transform startpos1;
    [SerializeField] private Transform startpos2;
    [SerializeField] private Transform startpos3;


    [SerializeField] private int obstacleCountTarget;
    [SerializeField] private float levelWidth;
    [SerializeField] private float levelDepth;
    [SerializeField] private float minScale;
    [SerializeField] private float maxScale;
    [SerializeField] private float spread;
    [SerializeField] private float minDistance;
    [SerializeField] private int triesCount;

    [SerializeField] public Transform obstaclePrefab;
    [SerializeField] public Transform grass;


    private List<Transform> obstacles;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Generate()
    {
        obstacles = new List<Transform>();

        Vector3 pos = new Vector3(Random.value * levelWidth - (levelWidth * 0.5f), 20, Random.value * levelDepth - (levelDepth * 0.5f));
        obstacles.Add(CreateObject(pos, grass));

        int count = 0, safe = 1000;
        while (count < obstacleCountTarget && safe >0)
        {
            pos = new Vector3(Random.value * levelWidth - (levelWidth) * 0.5f, 20, Random.value * levelDepth - (levelDepth * 0.5f));

            bool valid = true;
            for (int i = 0; i<obstacles.Count; ++i) 
            { 
                if (Vector3.Distance(new Vector3(pos.x,0,pos.z), new Vector3(obstacles[i].position.x, 0, obstacles[i].position.z)) < spread)
                {
                    valid = false; break;
                }
            }

            if (valid)
            {
                obstacles.Add(CreateObject(pos, grass));

                count++;
            }

            safe--;
        }
    }
    private Transform CreateObject(Vector3 position, Transform obj)
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(position, Vector3.down, out hitInfo,100, LayerMask.GetMask("Ground"))) 
        {
            position.y = hitInfo.point.y;
        }
        Transform transform = Instantiate(obj, position, Quaternion.Euler(0, Random.value * 360, 0));

        transform.localScale = transform.localScale + Vector3.one * Random.Range(minScale,maxScale);
        return transform;
    }

    public void GeneratePoisson(Transform obj, float distance, int countTarget)
    {
        obstacles = new List<Transform>();

        List<Vector3> points = new List<Vector3>();
        points.Add(Vector3.zero);

        int count = 0, safe = 1000;
        while (points.Count > 0 && count < countTarget && safe > 0)
        {
            int selectedIndex = Random.Range(0, points.Count);
            Vector3 point = points[selectedIndex];


            int tries = triesCount;
            while (tries > 0)
            {
                Vector3 newPos = point + GetRandomPoint();
                bool valid = true;
                for (int i = 0; i < obstacles.Count; i++)
                {
                    if (Vector3.Distance(newPos, new Vector3(obstacles[i].position.x, 0, obstacles[i].position.z)) < distance || !OnField(newPos) || TooClose(newPos))
                    {
                        valid = false;
                        break;
                    }
                }

                if (valid)
                {
                    obstacles.Add(CreateObject(newPos, obj));
                    points.Add(newPos);
                    count++;
                    break;
                }

                tries--;
            }

            if (tries <= 0)
            {
                points.RemoveAt(selectedIndex);
            }

            safe--;
        }
    }

    private Vector3 GetRandomPoint()
    {
        Vector2 randomPoint = Random.insideUnitCircle;
        return new Vector3(randomPoint.x, 0, randomPoint.y) * spread;
    }

    public void ClearObstacles()
    {
        foreach (Transform obstacle in obstacles)
        {
            GameObject.Destroy(obstacle.gameObject);
        }
    }

    private bool OnField(Vector3 pos)
    {
        bool check = pos.x > -levelDepth * 0.5 && pos.x < levelDepth * 0.5 && pos.z > -levelWidth * 0.5 && pos.z < levelWidth * 0.5;
        return check;
    }

    private bool TooClose(Vector3 pos)
    {
        List<Transform> startPoss = new List<Transform> { startpos1, startpos2, startpos3 };
        bool valid = false;


        foreach (Transform startPos in startPoss)
        {
            if (Vector3.Distance(pos, new Vector3(startPos.position.x, 0, startPos.position.z)) < 10)
            {
                valid = true; break;
            }
        }

        return valid;
    }
}

