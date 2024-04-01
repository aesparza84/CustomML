using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingArena : MonoBehaviour
{
    /// <summary>
    /// Array to store all rings in arena
    /// </summary>
    private GameObject[] RingArray;

    [SerializeField] private Color DefaultColor;
    [SerializeField] private Color ActiveColor;
    private void Awake()
    {
        RingArray = new GameObject[transform.childCount];
        for (int i = 0; i < RingArray.Length; i++)
        {
            RingArray[i] = transform.GetChild(i).gameObject;
        }
    }

    public void ResetAllRings()
    {
        MeshRenderer render;
        for (int i = 0; i < RingArray.Length; i++)
        {
            RingArray[i].gameObject.SetActive(true);
            render = RingArray[i].GetComponent<MeshRenderer>();
            render.material.color = DefaultColor;
        }
    }

    public GameObject GetRing(int index)
    {
        if (RingArray[index] != null)
        {
            SetActiveColor(index);  
            return RingArray[index].gameObject;
        }

        return null;
    }

    public int GetMaxIndex()
    {
        int n = 0;
        if (RingArray.Length>0)
        {
            n = RingArray.Length - 1;
        }

        return n;
    }

    private void SetActiveColor(int index)
    {
        RingArray[index].GetComponent<MeshRenderer>().material.color = ActiveColor;
    }
}
