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

    #region �W�����Ӥ���ç�A�]���O�ϥ� Window ���Ѫ� dll
    private struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    #region Define function signatures to import from Windows APIs (�ѼƦW�٥i�ۭq)
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

    // TODO: ���ݱоǡA�ץ�����
    void Start()
    {
        // �U��{�����T�O���b�s��Ҧ��U����A�n���]�������ɦA����
#if !UNITY_EDITOR
        window = GetActiveWindow();

        // �����L���
        MARGINS margins = new MARGINS() { cxLeftWidth = -1 };
        DwmExtendFrameIntoClientArea(window, ref margins);

        // �Ϩ�b�̤W�h
        SetWindowLong(window, GWL_STYLE, WS_LAYERED);

        // �Ϩ�z���B��z(�i�I��᭱������)
        SetLayeredWindowAttributes(window, 0, 0, LWA_COLORKEY);

        // �Ϩ�i����
        SetWindowPos(window, HWND_TOPMOST, 0, 0, 0, 0, 0);

        Application.runInBackground = true;
#endif
    }
}