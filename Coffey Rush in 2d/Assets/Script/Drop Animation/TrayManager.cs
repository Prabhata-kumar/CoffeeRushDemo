using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class TrayManager : MonoBehaviour
{
    public Color trayColor;
    public List<Transform> cupSlots; // Assign in inspector
    public CoffeeLineManager coffeeLineManager;
    public TrayPlacementManager trayPlacementManager; // 👈 Reference set during tray spawn

    private int currentCupCount = 0;
    private Collider2D trayCollider;
    private bool isFilling = false;
    private int placementIndex = -1;

    private void Start()
    {
        trayCollider = GetComponent<Collider2D>();
    }
    public void SetPlacementIndex(int index)
    {
        placementIndex = index;
    }

    private void OnMouseDown()
    {
        // Only trigger placement, not cup fill
        if (trayPlacementManager != null)
        {
            trayPlacementManager.TryPlaceTray(this);
        }
    }


    public IEnumerator FillTraySequentially()
    {
        isFilling = true;

        while (currentCupCount < cupSlots.Count)
        {
            GameObject topCup = coffeeLineManager.PeekTopCup();
            if (topCup == null) break;

            Color cupColor = topCup.GetComponent<SpriteRenderer>().color;

            if (!ColorsAreClose(cupColor, trayColor))
                break;

            coffeeLineManager.RemoveTopCup();

            Transform cupTransform = topCup.transform;
            Vector3 endPos = cupSlots[currentCupCount].position;

            // Use DOTween jump animation
            Sequence jumpSeq = DOTween.Sequence();
            jumpSeq.Append(cupTransform.DOJump(endPos, 2f, 1, 0.4f)).SetEase(Ease.Linear);
            cupTransform.parent = cupSlots[currentCupCount];
            yield return jumpSeq.WaitForCompletion();

            currentCupCount++;
            yield return new WaitForSeconds(0.1f);
        }

        if (currentCupCount >= cupSlots.Count)
        {
            trayCollider.enabled = false;
            Debug.Log("Tray full and collider disabled: " + trayColor);

            // 🔥 Sink and destroy
            SinkAndDestroy();
        }

        isFilling = false;


        if (currentCupCount >= cupSlots.Count)
        {
            trayCollider.enabled = false;

            // Check if we should sink
            if (trayPlacementManager != null)
            {
                bool anyEmpty = trayPlacementManager.HasAnyEmptySlots();
                if (!anyEmpty)
                {
                    SinkAndDestroy();

                }
            }
        }
    }

    void SinkAndDestroy()
    {
       
        transform.DOScale(0,1).SetEase(Ease.Linear).OnComplete(() => { 
        trayPlacementManager?.NotifyTrayDestroyed(placementIndex);
        trayPlacementManager.TryIfExistingTrayMatches();
        Destroy(gameObject);
        });
    }



    bool ColorsAreClose(Color a, Color b, float tolerance = 0.01f)
    {
        return Mathf.Abs(a.r - b.r) < tolerance &&
               Mathf.Abs(a.g - b.g) < tolerance &&
               Mathf.Abs(a.b - b.b) < tolerance;
    }
}
