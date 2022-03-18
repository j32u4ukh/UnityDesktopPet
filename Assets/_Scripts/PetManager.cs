using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class PetManager : MonoBehaviour
{
    SingleScreenManager single;
    DualScreenManager dual;

    private void Awake()
    {
        single = GetComponent<SingleScreenManager>();
        dual = GetComponent<DualScreenManager>();

        if (Config.use_dual_screen)
        {
            single.enabled = false;
            dual.enabled = true;
        }
        else
        {
            single.enabled = true;
            dual.enabled = false;
        }
    }
}
