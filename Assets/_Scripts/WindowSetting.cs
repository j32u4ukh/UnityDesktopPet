using System;
using System.Runtime.InteropServices;
using UnityEngine;

/* NOTE: 
 * # Project Settings > Player > Resolution and Presentation > Use DXGI Flip Model Swapchain for D3D11(DO NOT CHECK) */
public class WindowSetting : MonoBehaviour
{
    IntPtr window;

    // Definitions of window styles
    const int GWL_STYLE = -20;
    const uint WS_LAYERED = 0x00080000;
    const uint LWA_COLORKEY = 0x00000001;
    static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

    #region 名稱應該不能亂改，因為是使用 Window 提供的 dll
    private struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    #region Define function signatures to import from Windows APIs (參數名稱可自訂)
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("Dwmapi.dll")]
    private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    private static extern int SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);
    #endregion
    #endregion

    // TODO: 重看教學，修正註解
    void Start()
    {
        // 下方程式應確保不在編輯模式下執行，要打包成執行檔再執行
#if !UNITY_EDITOR
        window = GetActiveWindow();

        // 視窗無邊框
        MARGINS margins = new MARGINS() { cxLeftWidth = -1 };
        DwmExtendFrameIntoClientArea(window, ref margins);

        // 使其在最上層
        SetWindowLong(window, GWL_STYLE, WS_LAYERED);

        // 使其透明處穿透(可點到後面的頁面)
        SetLayeredWindowAttributes(window, 0, 0, LWA_COLORKEY);

        // 使其可互動
        SetWindowPos(window, HWND_TOPMOST, 0, 0, 0, 0, 0);

        Application.runInBackground = true;
#endif
    }
}