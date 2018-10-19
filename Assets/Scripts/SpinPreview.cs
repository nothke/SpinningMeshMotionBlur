using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinPreview : MonoBehaviour
{
    public SpinVolumetricBlur wheel;
    public SpinVolumetricBlur prop;

    [Range(0, 876)]
    public float rpm;
    public int count;

    GameObject[] blades;
    bool pause;
    GUISkin skin;
    
    string[] objectSelectionText = { "Wheel", "Prop" };

    int currentObject;
    int lastObject;
    int prevCount;
    bool useVolumeBlur = true;
    bool lastUseVolumeBlur = true;

    float bladePitch = 20;

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

    // For some reason Find() doesn't find objects immediately, skipping a frame is required
    IEnumerator WaitFrame()
    {
        yield return null;
        GetBlades();
    }

    void Reinit()
    {
        wheel.meshCount = count;
        wheel.Reinit();

        prop.meshCount = count;
        prop.Reinit();
    }

    // A very bad way of doing this. Forgive me, this is just for the preview :D
    void GetBlades()
    {
        blades = GameObject.FindGameObjectsWithTag("Blade");
        Debug.Log("Found: " + blades.Length + " blades");
    }

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

    // Float slider
    void Slider(ref float value, string label, string numberFormat, float min, float max)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(label, GUILayout.Width(60));
        GUILayout.Label(value.ToString(numberFormat), GUILayout.Width(35));
        value = GUILayout.HorizontalSlider(value, min, max);
        GUILayout.EndHorizontal();
    }

    // Int slider
    void Slider(ref int value, string label, int min, int max)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(label, GUILayout.Width(60));
        GUILayout.Label(value.ToString(), GUILayout.Width(35));
        value = Mathf.RoundToInt(GUILayout.HorizontalSlider(value, min, max));
        GUILayout.EndHorizontal();
    }
}
