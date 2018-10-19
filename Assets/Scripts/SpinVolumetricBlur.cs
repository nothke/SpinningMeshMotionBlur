using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinVolumetricBlur : MonoBehaviour
{
    public GameObject volumetricWheelModelRoot;

    public bool useVolumetricWheelBlur;
    public Vector3 rotationAxis = new Vector3(1, 0, 0);
    public int volumetricWheelCount = 20;
    public Material volumetricWheelMaterial;
    public float volumetricWheelRPMMult = 0.02f;

    public const float RPM2DPS = 6; // revolution per minute to degrees per second

    Transform[] volumetricWheelModels;
    float offsetAngle;

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
        Color color = volumetricWheelMaterial.color;
        color.a = (1.0f / volumetricWheelCount) * 2;
        volumetricWheelMaterial.color = color;

        // Calculate angle
        offsetAngle = volumetricWheelRPMMult / volumetricWheelCount;

        // Make duplicates
        volumetricWheelModels = new Transform[volumetricWheelCount];

        for (int i = 0; i < volumetricWheelCount; i++)
        {
            volumetricWheelModels[i] = Instantiate(
                volumetricWheelModelRoot,
                volumetricWheelModelRoot.transform.parent, false).transform;

            Renderer[] renderers = volumetricWheelModels[i].GetComponentsInChildren<Renderer>();

            for (int r = 0; r < renderers.Length; r++)
            {
                renderers[r].sharedMaterial = volumetricWheelMaterial;
            }
        }

        volumetricWheelModelRoot.SetActive(false);
    }

    public void UpdateVisibility(bool enable)
    {
        volumetricWheelModelRoot.SetActive(!enable);

        for (int i = 0; i < volumetricWheelCount; i++)
            volumetricWheelModels[i].gameObject.SetActive(enable);
    }

    public void UpdateSpin(float rpm)
    {
        // Calculate angle - this is just for preview, but you don't need to calculate it every frame
        offsetAngle = volumetricWheelRPMMult / volumetricWheelCount;

        volumetricWheelModelRoot.SetActive(false);

        for (int i = 0; i < volumetricWheelCount; i++)
        {
            volumetricWheelModels[i].gameObject.SetActive(true);
            float angle = i * rpm * offsetAngle;
            volumetricWheelModels[i].localEulerAngles = rotationAxis * angle;
        }
    }
}