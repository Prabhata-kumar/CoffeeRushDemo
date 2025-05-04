using System.Collections.Generic;
using UnityEngine;

public class TrayPlacementManager : MonoBehaviour
{
    public List<Transform> trayPlacements; // Assigned in Inspector
    public List<TrayManager> activeTrays = new();

    public bool TryPlaceTray(TrayManager tray)
    {
        for (int i = 0; i < trayPlacements.Count; i++)
        {
            if (IsSlotEmpty(i))
            {
                tray.transform.position = trayPlacements[i].position;
                tray.transform.parent = trayPlacements[i];
                tray.SetPlacementIndex(i);
                activeTrays.Insert(i, tray);
                StartCoroutine(tray.FillTraySequentially());
                 // Tray will decide if cup matches
                return true;
            }
        }

        Debug.Log("No empty tray slots!");
        return false;
    }

    public void NotifyTrayDestroyed(int index)
    {
        if (index >= 0 && index < activeTrays.Count)
        {
            activeTrays[index] = null;
            activeTrays.RemoveAt(index);
        }

    }


    bool IsSlotEmpty(int index)
    {
        return index >= activeTrays.Count || activeTrays[index] == null;
    }

    public bool HasAnyEmptySlots()
    {
        for (int i = 0; i < trayPlacements.Count; i++)
        {
            if (IsSlotEmpty(i))
                return true;
        }
        return false;
    }

    public void TryIfExistingTrayMatches()
    {
        for (int i = 0; i < activeTrays.Count; i++)
        {
            StartCoroutine(activeTrays[i].FillTraySequentially());
        }
    }

}
