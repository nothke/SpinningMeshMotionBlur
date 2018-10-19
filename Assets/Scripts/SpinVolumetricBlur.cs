using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinVolumetricBlur : MonoBehaviour
{
    public Vector3 rotationAxis = new Vector3(1, 0, 0);

    public bool useVolumetricWheelBlur;
    public GameObject volumetricWheelModelRoot;
    public int volumetricWheelCount = 100;
    Transform[] volumetricWheelModels;
    public Material volumetricWheelMaterial;
    public float volumetricWheelRPMMult = 0.1f;
    public float volumetricWheelRPMThreshold = 100;

    float offsetAngle;

    public const float RPM2APS = 6; // revolution per minute to angles per second

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

    public void UpdateVisibility()
    {

    }

    public void UpdateSpin(float rpm)
    {
        // Calculate angle - this is just for preview, but you don't need to calculate it every frame
        offsetAngle = volumetricWheelRPMMult / volumetricWheelCount;

        if (!useVolumetricWheelBlur)
        {
            for (int i = 0; i < volumetricWheelCount; i++)
                volumetricWheelModels[i].gameObject.SetActive(false);

            volumetricWheelModelRoot.SetActive(true);
        }
        else
        {
            volumetricWheelModelRoot.SetActive(false);

            for (int i = 0; i < volumetricWheelCount; i++)
            {
                volumetricWheelModels[i].gameObject.SetActive(true);
                float angle = i * rpm * offsetAngle;
                volumetricWheelModels[i].localEulerAngles = rotationAxis * angle;
            }
        }
    }
}