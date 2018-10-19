using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinPreview : MonoBehaviour
{

    [Range(0, 876)]
    public float rpm;
    public int count;

    public SpinVolumetricBlur wheel;
    public SpinVolumetricBlur prop;

    void Start()
    {
        wheel.meshCount = count;
        wheel.meshCount = count;

        wheel.Init();
        prop.Init();

        prop.gameObject.SetActive(false);

        prevCount = count;

        skin = Resources.Load("Skin") as GUISkin;
    }

    GameObject[] blades;

    bool pause;

    GUISkin skin;

    void Update()
    {

        if (count != prevCount)
        {
            Reinit();

            if (currentObject == 1)
            {
                StopAllCoroutines();
                StartCoroutine(WaitFrame());
            }
        }

        if (currentObject != lastObject)
        {
            if (currentObject == 0)
            {
                prop.gameObject.SetActive(false);
                wheel.gameObject.SetActive(true);
            }
            else
            {
                prop.gameObject.SetActive(true);
                wheel.gameObject.SetActive(false);
                GetBlades();
            }
        }

        if (currentObject == 0)
        {
            wheel.UpdateSpin(rpm);

            if (!pause)
                wheel.transform.Rotate(wheel.rotationAxis, rpm * SpinVolumetricBlur.RPM2DPS / 60);
        }
        else
        {
            prop.UpdateSpin(rpm);

            if (!pause)
                prop.transform.Rotate(prop.rotationAxis, rpm * SpinVolumetricBlur.RPM2DPS / 60);

            if (blades.Length != 0)
                for (int i = 0; i < blades.Length; i++)
                {
                    if (blades[i] == null) continue;
                    blades[i].transform.localEulerAngles = new Vector3(0, 45.0f - bladePitch, 0);
                }
        }

        prevCount = count;
        lastObject = currentObject;

        if (lastUseVolumeBlur != useVolumeBlur)
        {
            wheel.UpdateVisibility(useVolumeBlur);
            prop.UpdateVisibility(useVolumeBlur);
        }
    }

    IEnumerator WaitFrame()
    {
        yield return null;
        GetBlades();
    }

    int currentObject;
    string[] objectSelectionText = { "Wheel", "Prop" };
    int lastObject;
    int prevCount;

    bool useVolumeBlur = true;
    bool lastUseVolumeBlur = true;

    float bladePitch = 20;

    private void OnGUI()
    {
        GUI.skin = skin;

        Rect rect = new Rect(50, 50, 300, 1000);

        GUILayout.BeginArea(rect);

        useVolumeBlur = GUILayout.Toggle(useVolumeBlur, "Use Volumetric Mesh Blur");
        currentObject = GUILayout.SelectionGrid(currentObject, objectSelectionText, 2);


        SpinVolumetricBlur cur = currentObject == 0 ? wheel : prop;

        Slider(ref rpm, "RPM", "F0", 0, 875.23f);
        Slider(ref cur.spreadPerRPM, "Spread", "F2", 0, 0.1f);

        Slider(ref count, "Mesh count", 2, 100);

        Slider(ref bladePitch, "Prop pitch", "F1", 0, 45);

        pause = GUILayout.Toggle(pause, "Pause");

        GUILayout.Label("Hold right mouse button to rotate the camera");
        GUILayout.Label("Hold shift and right mouse button to rotate the sun");

        GUILayout.EndArea();
    }

    void Reinit()
    {
        wheel.meshCount = count;
        wheel.Reinit();

        prop.meshCount = count;
        prop.Reinit();

    }

    void GetBlades()
    {
        blades = GameObject.FindGameObjectsWithTag("Blade");
        Debug.Log("Found: " + blades.Length + " blades");
    }

    void Slider(ref float value, string label, string numberFormat, float min, float max)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(label, GUILayout.Width(60));
        GUILayout.Label(value.ToString(numberFormat), GUILayout.Width(35));
        value = GUILayout.HorizontalSlider(value, min, max);
        GUILayout.EndHorizontal();
    }

    void Slider(ref int value, string label, int min, int max)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(label, GUILayout.Width(60));
        GUILayout.Label(value.ToString(), GUILayout.Width(35));
        value = Mathf.RoundToInt(GUILayout.HorizontalSlider(value, min, max));
        GUILayout.EndHorizontal();
    }
}
