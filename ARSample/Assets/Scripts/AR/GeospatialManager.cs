using Google.XR.ARCoreExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeospatialManager : MonoBehaviour
{
    private AREarthManager _arEarthManager;
    private IGPS _gps;


    private void Awake()
    {
        _arEarthManager = GetComponent<AREarthManager>();
    }

    private void Start()
    {
#if UNITY_EDITOR
        _gps = new MockUnitOfWork().gps;
#elif UNITY_ANDROID
        _gps = new UnitOfWork().gps;
#else
#endif
        StartCoroutine(C_CheckValidation());
    }

    IEnumerator C_CheckValidation()
    {
        FeatureSupported featureSupported = FeatureSupported.Unsupported;

        yield return new WaitUntil(() =>
        {
            featureSupported = _arEarthManager.IsGeospatialModeSupported(GeospatialMode.Enabled);
            return featureSupported != FeatureSupported.Unknown; // Unknown : ���� �� ��⿡�� GeospatialMode �� �����Ǵ��� �����ȵǾ��ٴ� �� (�����ɶ����� ��ٷ�����)
        });

        switch (featureSupported)
        {
            case FeatureSupported.Supported:
                {
                    // todo -> �ʱ�ȭ�Ұ� ����
                    Debug.Log("This device supports geospatial mode !");
                }
                break;
            default:
                {
                    Debug.LogWarning($"This device does not support geospatial mode.. ");
                    Application.Quit();
                }
                break;
        }

        yield return new WaitUntil(() => _gps.isValid);
        VpsAvailabilityPromise vpsAvailablilityPromise = AREarthManager.CheckVpsAvailabilityAsync(_gps.latitude, _gps.longitude);
        yield return vpsAvailablilityPromise;
        Debug.Log($"{_gps.latitude}. {_gps.longitude}. {_gps.altitude}, {vpsAvailablilityPromise.Result}");
    }
}
