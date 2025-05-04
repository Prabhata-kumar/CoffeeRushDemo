using System.Collections.Generic;
using UnityEngine;

public class CoffeeLineManager : MonoBehaviour
{
    [System.Serializable]
    public class TrayData
    {
        public TrayManager tray;
        public int slotCount;
        public Color trayColor;
    }

    public List<TrayData> trays;
    public Transform spawnLineStart;
    public float cupSpacing = 1f;
    public GameObject cupPrefab;

    private List<GameObject> cupQueue = new();

    void Start()
    {
        GenerateCupsFromTrays();
    }

    void GenerateCupsFromTrays()
    {
        int totalCups = 0;

        foreach (var data in trays)
        {
            for (int i = 0; i < data.slotCount; i++)
            {
                GameObject cup = Instantiate(cupPrefab);
                cup.GetComponent<SpriteRenderer>().color = data.trayColor;
                cup.transform.position = spawnLineStart.position + Vector3.right * (totalCups * cupSpacing);
                cupQueue.Add(cup);
                totalCups++;
            }
        }
    }

    public GameObject PeekTopCup()
    {
        return cupQueue.Count > 0 ? cupQueue[0] : null;
    }

    public void RemoveTopCup()
    {
        if (cupQueue.Count > 0)
        {
            cupQueue.RemoveAt(0);
            ReorderCups();
        }
    }

    void ReorderCups()
    {
        for (int i = 0; i < cupQueue.Count; i++)
        {
            Vector3 targetPos = spawnLineStart.position + Vector3.right * (i * cupSpacing);
            cupQueue[i].transform.position = targetPos;
        }
    }


}
