using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveNotebook : MonoBehaviour
{
    public void HideNotebook()
    {
        Settings.instance.scalerBlock.ScaleDown();
    }
}
