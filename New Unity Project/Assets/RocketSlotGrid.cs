using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketSlotGrid : MonoBehaviour
{
    public Vector2 dimensions;
    public float slotSize;

    [HideInInspector]
    public Vector2 middle;
    /// <summary>
    /// Rocket Slots are indexed as x, then y, starting at the top left corner.
    /// </summary>
    RocketSlot[][] rocketSlots;
    // Start is called before the first frame update
    private void Awake()
    {
        middle = dimensions * 0.5f;
    }
    private void OnValidate()
    {
        middle = dimensions * 0.5f;
    }

    public class RocketSlot
    {

    }
}
