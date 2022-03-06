using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class PetManger : MonoBehaviour
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern bool ReleaseCapture();

    [DllImport("user32.dll")]
    private static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParm, int lParm);

    IntPtr window;

    // Start is called before the first frame update
    void Start()
    {
        //window = GetActiveWindow();
        window = GetForegroundWindow();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            ReleaseCapture();
            SendMessage(window, 0xA1, 0x02, 0);
            SendMessage(window, 0x0202, 0, 0);
        }

        if (Input.GetMouseButton(1))
        {
            // TODO: Settings
        }
    }
}
