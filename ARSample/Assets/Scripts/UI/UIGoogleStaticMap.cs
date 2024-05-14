using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UIGoogleStaticMap : MonoBehaviour, InputActions.IUIActions
{
    public int zoomLevel
    {
        get => _zoomLevel;
        set
        {
            if (_zoomLevel == value)
                return;

            _zoomLevel = value;
            onZoomLevelChanged?.Invoke(value);
        }
    }

    private enum Resolution
    {
        Low = 1,
        High = 2,
    }
    private Resolution _resolution = Resolution.Low;
    private enum TextureStyle
    {
        Roadmap,
        Satellite,
        Gybrid,
        Terrain,
    }
    private TextureStyle _textureStyle = TextureStyle.Roadmap;
    private readonly string GOOGLE_MAP_API_KEY = "AIzaSyB3NA2eKGtGUYxPbxZWaHtijNfB8M6oWRo";
    private RectTransform _rawImageRectTransform;
    private RawImage _rawImage;
    [SerializeField] private int _zoomLevel = 10;
    private Vector2 _imageSize;
    private IGPS _gps;
    public event Action<int> onZoomLevelChanged;
    private InputActions _inputActions;

    private void Awake()
    {
        _inputActions = new InputActions();
        _inputActions.UI.SetCallbacks(this);
        _inputActions.UI.Enable();

        _rawImage = GetComponentInChildren<RawImage>();
        _rawImageRectTransform = _rawImage.GetComponent<RectTransform>();
        _imageSize = new Vector2(_rawImageRectTransform.rect.width, _rawImageRectTransform.rect.height);
        _gps = new MockUnitOfWork().gps;

        Button zoomIn = _rawImage.transform.Find("Button - ZoomIn").GetComponent<Button>();
        Button zoomOut = _rawImage.transform.Find("Button - ZoomOut").GetComponent<Button>();
        zoomIn.onClick.AddListener(() => zoomLevel++);
        zoomOut.onClick.AddListener(() => zoomLevel--);
        onZoomLevelChanged += value =>
        {
            zoomIn.interactable = value < 20;
            zoomOut.interactable = value > 1;
            StartCoroutine(C_RequestMap());
        };
    }

    private void Start()
    {
        StartCoroutine(C_RequestMap());
    }

    private void Update()
    {
        if (_gps.isDirty)
            StartCoroutine(C_RequestMap());
    }

    IEnumerator C_RequestMap()
    {
        string url = $"https://maps.googleapis.com/maps/api/staticmap?center={_gps.latitude},{_gps.longitude}&zoom={_zoomLevel}&size={_imageSize.x}x{_imageSize.y}&scale={_resolution}&maptype={_textureStyle}&key={GOOGLE_MAP_API_KEY}";
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest(); // �� ��û �� ���� ���

        if (www.result == UnityWebRequest.Result.Success)
        {
            _rawImage.texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        }
        else if (www.result == UnityWebRequest.Result.InProgress)
        {
            Debug.Log($"[UIGoogleStaticMap] : WebRequest is not finished...");
        }
        else
        {
            Debug.LogWarning($"[UIGoogleStaticMap] : WebRequest has an error {www.error}");
        }
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
        }
    }

    public void OnScrollWheel(InputAction.CallbackContext context)
    {
    }

    public void OnMiddleClick(InputAction.CallbackContext context)
    {
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
    }

    public void OnTrackedDevicePosition(InputAction.CallbackContext context)
    {
    }

    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
    {
    }


}
