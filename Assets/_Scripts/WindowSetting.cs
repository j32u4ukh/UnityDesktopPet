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

    // �̤W�h����
    // Places the window above all non-topmost windows. The window maintains its topmost position even when it is deactivated. 
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

    // �ѦҡGhttps://docs.microsoft.com/zh-tw/windows/win32/api/dwmapi/nf-dwmapi-dwmextendframeintoclientarea
    [DllImport("Dwmapi.dll")]
    private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

    // �ѦҡGhttps://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowlonga
    // �ѦҡGhttps://docs.microsoft.com/en-us/windows/win32/winmsg/extended-window-styles
    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    // �ѦҡGhttps://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowpos
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    private static extern int SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);
    #endregion
    #endregion

    void Start()
    {
        // �U��{�����T�O���b�s��Ҧ��U����A�n���]�������ɦA����
#if !UNITY_EDITOR
        window = GetActiveWindow();

        // �����L���
        MARGINS margins = new MARGINS() { cxLeftWidth = -1 };
        DwmExtendFrameIntoClientArea(window, ref margins);

        // GWL_EXSTYLE�G�X�R�����ݩ�(Sets a new extended window style. )
        // WS_EX_LAYERED�G�N��e�����]��"���h����"(The WS_EX_LAYERED style is supported for top-level windows and child windows.)
        // �ѦҡGhttps://docs.microsoft.com/en-us/windows/win32/winmsg/window-features
        SetWindowLong(window, GWL_EXSTYLE, WS_EX_LAYERED);

        // �Ϩ�"�z���B"(crKey -> alpha == 0)��z(�i�I��᭱������)
        // LWA_COLORKEY: �ϥ� crKey �өw�q�������z���סC Use crKey as the transparency color. 
        // bAlpha: �]�m�������z���סA0 ��ܧ����z��
        SetLayeredWindowAttributes(window, crKey: 0, bAlpha: 0, LWA_COLORKEY);

        // �ϵ����b�̤W�h
        SetWindowPos(window, HWND_TOPMOST, 0, 0, 0, 0, 0);

        // �ϵ����b�ϥΪ��I���F�᭱�������A���M�|�~��B��A�Ӥ��|���U��
        Application.runInBackground = true;
#endif
    }
}