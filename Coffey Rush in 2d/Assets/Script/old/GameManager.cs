using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static Action<bool> SetOtherRigidBodys;

    [Header("Grid Settings")]
    public Transform startPoint;
    public int rows = 5;
    public int columns = 6;
    public float cellSize = 1f;

    [Header("Optional Tile Visuals")]
    public GameObject tilePrefab;
    public GameObject borderBlockPrefab;
    public GameObject glassObject;

    [Header("Gameplay")]
    public List<Transform> traySpaces;
    public List<GameObject> allBlockPrefabs;
    public List<BlockDetails> allBlocks;

    [Header("Gizmos")]
    public Color gizmoColor = Color.cyan;
    public bool showGizmos = true;

    public DraggableBlock[,] blockGrid;

    [Header("Glass Conveyor Path")]
    public List<Transform> glassPathPoints;
    public List<GameObject> allGleass;


    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        blockGrid = new DraggableBlock[columns, rows];
        LoadAllBlockDetails();
    }

    public void GenerateGrid()
    {
        if (startPoint == null)
        {
            Debug.LogWarning("Start Point not assigned!");
            return;
        }

        Vector3 origin = startPoint.position;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 spawnPos = origin + new Vector3(col * cellSize, -row * cellSize, 0f);
                if (tilePrefab != null)
                {
                    Instantiate(tilePrefab, spawnPos, Quaternion.identity, transform);
                }
            }
        }
    }

    public void GenerateBorder()
    {
        if (borderBlockPrefab == null || startPoint == null) return;

        Vector3 origin = startPoint.position;

        for (int col = -1; col <= columns; col++)
        {
            Vector3 top = origin + new Vector3(col * cellSize, cellSize, 0);
            Vector3 bottom = origin + new Vector3(col * cellSize, -rows * cellSize, 0);
            Instantiate(borderBlockPrefab, top, Quaternion.identity, transform);
            Instantiate(borderBlockPrefab, bottom, Quaternion.identity, transform);
        }

        for (int row = 0; row < rows; row++)
        {
            Vector3 left = origin + new Vector3(-cellSize, -row * cellSize, 0);
            Vector3 right = origin + new Vector3(columns * cellSize, -row * cellSize, 0);
            Instantiate(borderBlockPrefab, left, Quaternion.identity, transform);
            Instantiate(borderBlockPrefab, right, Quaternion.identity, transform);
        }
    }

    void OnDrawGizmos()
    {
        if (!showGizmos || startPoint == null) return;

        Gizmos.color = gizmoColor;
        Vector3 origin = startPoint.position;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 pos = origin + new Vector3(col * cellSize, -row * cellSize, 0f);
                Gizmos.DrawWireCube(pos, Vector3.one * cellSize);
            }
        }

        Vector3 topLeft = startPoint.position;
        Vector3 bottomRight = topLeft + new Vector3((columns - 1) * cellSize, -(rows - 1) * cellSize, 0f);
        Vector3 center = (topLeft + bottomRight) / 2f;
        Vector3 size = new Vector3(columns * cellSize, rows * cellSize, 0f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, size);
    }

    public Vector3 GetClosestGridPoint(Vector3 worldPos)
    {
        Vector3 origin = startPoint.position;
        float x = Mathf.Round((worldPos.x - origin.x) / cellSize) * cellSize + origin.x;
        float y = Mathf.Round((worldPos.y - origin.y) / cellSize) * cellSize + origin.y;
        return new Vector3(x, y, 0f);
    }

    public void LoadAllBlockDetails()
    {
        allBlocks.Clear(); // optional: reset list

        for (int i = 0; i < allBlockPrefabs.Count; i++)
        {
            if (allBlockPrefabs[i].TryGetComponent(out DraggableBlock db))
            {
                BlockDetails details = new BlockDetails
                {
                    glassColor = db.spr, // assuming spr is a Color, if not fix this
                    glassPositions = db.dropPos
                };
                allBlocks.Add(details);
            }
        }

        //CreateGlass();
    }

    public void CreateGlass()
    {
        for (int i = 0; i < allBlocks.Count; i++)
        {
            for (int j = 0; j < allBlocks[i].glassPositions.Count; j++)
            {
                GameObject glasses = Instantiate(glassObject, transform.position, Quaternion.identity);
                Glasses glass = glasses.GetComponent<Glasses>();
                glass.SetSpritColor(allBlocks[i].glassColor);
                allGleass.Add(glasses);
            }
        }
        
        for (int i = 0;i < allGleass.Count; i++)
        {
            allGleass[i].transform.position = glassPathPoints[i].position;
        }
    }




    public void PlaceForTray(Transform obj)
    {
        foreach (Transform space in traySpaces)
        {
            if (space.childCount == 0)
            {
                obj.transform.position = space.position;
                PlacingGlassesInBoard(obj);

                return;
            }
        }
    }

    public void PlacingGlassesInBoard(Transform obj)
    {
       
       /* if (obj.TryGetComponent(out DraggableBlock tray) )
        {
            if(allGleass[0].glasscor == tray.color)
            {
                tray.dropPos
            }
           
        }*/
    }


}

[System.Serializable]
public class BlockDetails
{
    public Color glassColor;
    public List<Transform> glassPositions;
}
