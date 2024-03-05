using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{

    [SerializeField] private int obstacleCountTarget;
    [SerializeField] private float levelWidth;
    [SerializeField] private float levelDepth;
    [SerializeField] private float minScale;
    [SerializeField] private float maxScale;
    [SerializeField] private float spread;
    [SerializeField] private float minDistance;
    [SerializeField] private int triesCount;

    [SerializeField] private Transform obstaclePrefab;

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
        obstacles.Add(CreateObject(pos));

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
                obstacles.Add(CreateObject(pos));

                count++;
            }

            safe--;
        }
    }
    private Transform CreateObject(Vector3 position)
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(position, Vector3.down, out hitInfo,100)) 
        {
            position.y = hitInfo.point.y;
        }

        Transform transform = Instantiate(obstaclePrefab, position, Quaternion.Euler(0, Random.value * 360, 0));

        transform.localScale = Vector3.one * Random.Range(minScale,maxScale);
        return transform;
    }
}
