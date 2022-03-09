using UnityEditor;
using UnityEngine;

public class DualScreenManager : MonoBehaviour
{
    public static bool use_dual_screen;

    [SerializeField] Camera camera1;
    [SerializeField] Camera camera2;

    // Start is called before the first frame update
    void Awake()
    {
        if (use_dual_screen)
        {
            camera1.rect = new Rect(0.0f, 0.0f, 0.5f, 1.0f);
            camera2.rect = new Rect(0.5f, 0.0f, 0.5f, 1.0f);
        }
        else
        {
            camera1.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
        }

        for (int i = 0; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
            Screen.SetResolution(Display.displays[i].renderingWidth, Display.displays[i].renderingHeight, true);
        }
    }
}
