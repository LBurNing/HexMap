using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FreeCamera : MonoBehaviour
{
    private Vector3 m_vecLasMouseClickPosition;

    ///【2】用于控制幅度的变量
    private float m_Scaling;
    //缩放速度
    public float m_ScalingSpeed = 15;

    //手型工具幅度;
    public float m_HandToolSpeed = -0.07f;
    public float m_Limit = 10;

    public float scale = 10;

    private float m_SpeedUp = 1;
    private new Camera camera;

    private Vector3 max;
    private Vector3 min;

    void Start()
    {
        camera = Camera.main;
        Event.Register(Event.CREATE_CELL_FINISH, CreateCellFinish);
    }

    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            float mouseScrollWheel = -Input.GetAxis("Mouse ScrollWheel");
            m_Scaling = camera.orthographicSize + mouseScrollWheel * m_ScalingSpeed * m_SpeedUp * Time.deltaTime;
            camera.orthographicSize = Mathf.Clamp(m_Scaling, 1, Mathf.Max(max.y, 1));
        }

        if (Input.GetMouseButtonDown(2))
        {
            m_vecLasMouseClickPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(2))
        {
            Vector3 NowHitPosition = Input.mousePosition;
            Vector3 offsetVec = NowHitPosition - m_vecLasMouseClickPosition;
            offsetVec = transform.rotation * offsetVec;
            Vector3 pos = transform.localPosition + offsetVec * (m_HandToolSpeed) * m_SpeedUp;
            transform.localPosition = PosLimit(pos);
            m_vecLasMouseClickPosition = Input.mousePosition;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            m_SpeedUp = 10;
        }
        else
        {
            m_SpeedUp = 1;
        }
    }

    private void CreateCellFinish()
    {
        max = Global.instance.GetMaxPos();
        camera.orthographicSize = Global.instance.diameter * 2;
        camera.transform.position = new Vector3(max.x / 2, max.y / 2, 0);
    }

    private Vector3 PosLimit(Vector3 pos)
    {
        Vector3 posLimit = pos;
        if (pos.x < -m_Limit)
        {
            posLimit.x = -m_Limit;
        }

        if (pos.x > max.x + m_Limit)
        {
            posLimit.x = max.x + m_Limit;
        }

        if (pos.y < -m_Limit)
        {
            posLimit.y = -m_Limit;
        }

        if (pos.y > max.y + m_Limit)
        {
            posLimit.y = max.y + m_Limit;
        }

        return posLimit;
    }
}