using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class TransparentWindow : MonoBehaviour
{
    private struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    // Define function signatures to import from Windows APIs

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("Dwmapi.dll")]
    private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

    [DllImport("user32.dll")]
    private static extern bool ReleaseCapture();

    [DllImport("user32.dll")]
    private static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParm, int lParm);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    // Definitions of window styles
    const int GWL_STYLE = -16;
    const uint WS_POPUP = 0x80000000;
    const uint WS_VISIBLE = 0x10000000;

    const int GWL_EX_STYLE = -20;
    const uint WS_TOPMOST = 0x00000008;
    const uint WS_LAYERED = 0x00080000;
    const uint WS_TRANSPARENT = 0x00000020;

    const uint SWP_SHOWWINDOW = 0x0040;

    IntPtr window;

    static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

    /* NOTE: 
     * # Project Settings > Player > Resolution and Presentation > Use DXGI Flip Model Swapchain for D3D11(DO NOT CHECK)
     */

    void Start()
    {
//#if !UNITY_EDITOR
        window = GetActiveWindow();

        // Make background transparent
        MARGINS margins = new MARGINS() { cxLeftWidth = -1 };
        DwmExtendFrameIntoClientArea(window, ref margins);

        //Resolution[] resolutions = Screen.resolutions;
        //Screen.SetResolution(resolutions[0].width, resolutions[0].height, true);

        //bool result = SetWindowPos(GetForegroundWindow(), window, 0, 0, resolutions[0].width, resolutions[0].height, SWP_SHOWWINDOW);

        //SetWindowLong(window, GWL_STYLE, WS_POPUP | WS_VISIBLE);
        SetWindowLong(window, GWL_EX_STYLE, WS_LAYERED | WS_TRANSPARENT);

        //SetWindowLong(window, GWL_EX_STYLE, WS_TOPMOST | WS_LAYERED | WS_TRANSPARENT);
        // 狀態機，呼叫一次後就會修改狀態，除非再次呼叫，否則下次執行不同場景也會被影響
        //SetWindowPos(window, HWND_TOPMOST, 0, 0, Screen.width, Screen.height, 0);
        SetWindowPos(window, HWND_TOPMOST, 0, 0, 0, 0, 0);
        //#endif

        Application.runInBackground = true;
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