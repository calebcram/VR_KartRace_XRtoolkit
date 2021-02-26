using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarSelector : MonoBehaviour
{
    public static int currentSelectedAvatarID = 0;

    public void SetAvatarID(int index)
    {
        currentSelectedAvatarID = index;
    }
}
