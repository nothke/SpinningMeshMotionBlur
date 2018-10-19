using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinVolumetricBlur : MonoBehaviour
{
    public GameObject spinningModelRoot;

    public Vector3 rotationAxis = new Vector3(1, 0, 0);
    public int meshCount = 20;
    public Material transparentMaterial;
    public float spreadPerRPM = 0.02f;

    public const float RPM2DPS = 6; // revolution per minute to degrees per second

    Transform[] volumetricWheelModels;
    float offsetAngle;

    const float ALPHA_MULT = 2; // I found that multiplying alpha by 2 produces more realistic results

    public void Reinit()
    {
        for (int i = 0; i < volumetricWheelModels.Length; i++)
        {
            Destroy(volumetricWheelModels[i].gameObject);
        }

        Init();
    }

    public void Init()
    {
        // Set transparent material alpha
        Color color = transparentMaterial.color;
        color.a = (1.0f / meshCount) * ALPHA_MULT;
        transparentMaterial.color = color;

        // Calculate angle
        offsetAngle = spreadPerRPM / meshCount;

        // Make duplicates
        volumetricWheelModels = new Transform[meshCount];

        for (int i = 0; i < meshCount; i++)
        {
            volumetricWheelModels[i] = Instantiate(
                spinningModelRoot,
                spinningModelRoot.transform.parent, false).transform;

            Renderer[] renderers = volumetricWheelModels[i].GetComponentsInChildren<Renderer>();

            for (int r = 0; r < renderers.Length; r++)
            {
                renderers[r].sharedMaterial = transparentMaterial;
            }
        }

        spinningModelRoot.SetActive(false);
    }

    public void UpdateVisibility(bool enable)
    {
        spinningModelRoot.SetActive(!enable);

        for (int i = 0; i < meshCount; i++)
            volumetricWheelModels[i].gameObject.SetActive(enable);
    }

    public void UpdateSpin(float rpm)
    {
        // Calculate angle - this is just for preview, but you don't need to calculate it every frame
        offsetAngle = spreadPerRPM / meshCount;

        spinningModelRoot.SetActive(false);

        for (int i = 0; i < meshCount; i++)
        {
            volumetricWheelModels[i].gameObject.SetActive(true);
            float angle = i * rpm * offsetAngle;
            volumetricWheelModels[i].localEulerAngles = rotationAxis * angle;
        }
    }
}