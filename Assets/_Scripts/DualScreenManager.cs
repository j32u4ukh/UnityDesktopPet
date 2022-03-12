using UnityEditor;
using UnityEngine;

public class DualScreenManager : ScreenManager
{
    Camera camera1;
    Camera camera2;

    public override void Start()
    {
        base.Start();
        camera1 = GameObject.Find("Camera1").GetComponent<Camera>();
        camera2 = GameObject.Find("Camera2").GetComponent<Camera>();

        #region 邊界值考慮 SpriteRenderer 尺寸，避免 SpriteRenderer 跑到視窗之外
        Vector3 min_point = camera1.ScreenToWorldPoint(Vector3.zero);
        x_min = min_point.x + size;
        y_min = min_point.y + size;
        Debug.Log($"x_min: {x_min}, y_min: {y_min}");

        Vector3 max_point = camera2.ScreenToWorldPoint(new Vector3(Screen.width * 2f, Screen.height, 0f));
        x_max = max_point.x - size;
        y_max = max_point.y - size;
        Debug.Log($"x_max: {x_max}, y_max: {y_max}");
        #endregion
    }

    public override void getLeftMouseDown()
    {
        throw new System.NotImplementedException();
    }

    public override void getLeftMouse()
    {
        throw new System.NotImplementedException();
    }

    public override void getLeftMouseUp()
    {
        throw new System.NotImplementedException();
    }

    public override void getRightMouseDown()
    {
        throw new System.NotImplementedException();
    }

    public override void getRightMouse()
    {
        throw new System.NotImplementedException();
    }

    public override void getRightMouseUp()
    {
        throw new System.NotImplementedException();
    }
}
