using UnityEngine;

public class MouseOrbit : MonoBehaviour
{
    public float distance = 5.0f;
    public float xSpeed = 10;
    public float ySpeed = 10;

    public float scrollSpeed = 1;
    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    public float distanceMin = .5f;
    public float distanceMax = 15f;

    public bool updateProbe;
    public bool locked;
    public bool useShift;

    Transform child;
    ReflectionProbe probe;
    float x = 0.0f;
    float y = 0.0f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        probe = FindObjectOfType<ReflectionProbe>();

        if (transform.childCount != 0)
            child = transform.GetChild(0);
    }

    //Vector3 position;

    void FixedUpdate()
    {
        bool condition = useShift ?
            Input.GetMouseButton(1) && Input.GetKey(KeyCode.LeftShift) :
            Input.GetMouseButton(1) && !Input.GetKey(KeyCode.LeftShift);

        if (!condition)
            return;

        if (updateProbe && probe)
            probe.RenderProbe();

        //DynamicGI.UpdateEnvironment();

        x += Input.GetAxis("Mouse X") * xSpeed;
        y -= Input.GetAxis("Mouse Y") * ySpeed;

        y = ClampAngle(y, yMinLimit, yMaxLimit);

        Quaternion rotation = Quaternion.Euler(y, x, 0);

        distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * scrollSpeed, distanceMin, distanceMax);


        transform.rotation = rotation;
        //transform.position = position;

        if (child)
            child.localPosition = new Vector3(0, 0, -distance);
    }

    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}