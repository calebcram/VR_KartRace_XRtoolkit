using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartSelection : MonoBehaviour
{
    public int initialIndex = 0;
    public GameObject[] karts;

    private void Start()
    {
        ShowKartAtIndex(initialIndex);
    }

    public void HideAll()
    {
        foreach (var item in karts)
        {
            item.SetActive(false);
        }
    }

    public void SelectAndShowKartAtIndex(int index)
    {
        AvatarSelector.currentSelectedAvatarID = index;
        ShowKartAtIndex(index);
    }

    public void ShowKartAtIndex(int index)
    {
        for (int i = 0; i < karts.Length; i++)
        {
            karts[i].SetActive(index == i);
        }
    }
}
