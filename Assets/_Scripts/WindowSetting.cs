using System;
using System.Runtime.InteropServices;
using UnityEngine;

/* NOTE: 
 * # Project Settings > Player > Resolution and Presentation > Use DXGI Flip Model Swapchain for D3D11(DO NOT CHECK) */
public class WindowSetting : MonoBehaviour
{
    IntPtr window;

    // Definitions of window styles
    const int GWL_EXSTYLE = -20;
    const uint WS_EX_LAYERED = 0x00080000;
    const uint LWA_COLORKEY = 0x00000001;

    // 最上層視窗
    // Places the window above all non-topmost windows. The window maintains its topmost position even when it is deactivated. 
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

    // 參考：https://docs.microsoft.com/zh-tw/windows/win32/api/dwmapi/nf-dwmapi-dwmextendframeintoclientarea
    [DllImport("Dwmapi.dll")]
    private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

    // 參考：https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowlonga
    // 參考：https://docs.microsoft.com/en-us/windows/win32/winmsg/extended-window-styles
    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    // 參考：https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowpos
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    private static extern int SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);
    #endregion
    #endregion

    void Start()
    {
        // 下方程式應確保不在編輯模式下執行，要打包成執行檔再執行
#if !UNITY_EDITOR
        window = GetActiveWindow();

        // 視窗無邊框
        MARGINS margins = new MARGINS() { cxLeftWidth = -1 };
        DwmExtendFrameIntoClientArea(window, ref margins);

        // GWL_EXSTYLE：擴充視窗屬性(Sets a new extended window style. )
        // WS_EX_LAYERED：將當前視窗設為"分層視窗"(The WS_EX_LAYERED style is supported for top-level windows and child windows.)
        // 參考：https://docs.microsoft.com/en-us/windows/win32/winmsg/window-features
        SetWindowLong(window, GWL_EXSTYLE, WS_EX_LAYERED);

        // 使其"透明處"(crKey -> alpha == 0)穿透(可點到後面的頁面)
        // LWA_COLORKEY: 使用 crKey 來定義視窗不透明度。 Use crKey as the transparency color. 
        // bAlpha: 設置視窗不透明度，0 表示完全透明
        SetLayeredWindowAttributes(window, crKey: 0, bAlpha: 0, LWA_COLORKEY);

        // 使視窗在最上層
        SetWindowPos(window, HWND_TOPMOST, 0, 0, 0, 0, 0);

        // 使視窗在使用者點擊了後面的視窗，仍然會繼續運行，而不會停下來
        Application.runInBackground = true;
#endif
    }
}