using UnityEngine;

public class PetManager : MonoBehaviour
{
    [SerializeField] private bool dual_screen = false;
    public static bool use_dual_screen = false;

    SingleScreenManager single;
    DualScreenManager dual;

    private void Awake()
    {
        use_dual_screen = dual_screen;

        single = GetComponent<SingleScreenManager>();
        dual = GetComponent<DualScreenManager>();

        if (use_dual_screen)
        {
            single.enabled = false;
            dual.enabled = true;
        }
        else
        {
            single.enabled = true;
            dual.enabled = false;
        }

        Debug.Log($"[PetManager] Awake | #Display: {Display.displays.Length}");

        for (int i = 0; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }
    }
}
