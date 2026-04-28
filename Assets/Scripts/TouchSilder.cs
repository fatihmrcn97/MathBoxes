using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TouchSilder : MonoBehaviour
{

    public UnityAction OnPointerDownEvent;
    public UnityAction<float> OnPointerDragEvent;
    public UnityAction OnPointerUpEvent;

    [SerializeField] private float activeScreenHeight = 0.5f;

    private bool isPointerDown;
    private int activeFingerId = -1;

    private void Awake()
    {
        DisableLegacySlider();
    }

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseInput();
#endif
        HandleTouchInput();
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
            TryStartPointer(Input.mousePosition);

        if (Input.GetMouseButton(0) && isPointerDown)
            OnPointerDrag(Input.mousePosition);

        if (Input.GetMouseButtonUp(0) && isPointerDown)
            EndPointer();
    }

    private void HandleTouchInput()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            if (touch.phase == TouchPhase.Began && !isPointerDown)
            {
                if (TryStartPointer(touch.position))
                    activeFingerId = touch.fingerId;
            }

            if (touch.fingerId != activeFingerId)
                continue;

            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                OnPointerDrag(touch.position);

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                EndPointer();
        }
    }

    private bool TryStartPointer(Vector2 screenPosition)
    {
        if (screenPosition.y > Screen.height * activeScreenHeight)
            return false;

        isPointerDown = true;
        activeFingerId = -1;

        OnPointerDownEvent?.Invoke();
        OnPointerDrag(screenPosition);

        return true;
    }

    private void OnPointerDrag(Vector2 screenPosition)
    {
        if (Screen.width <= 0)
            return;

        float xMovement = Mathf.Clamp((screenPosition.x / Screen.width * 2f) - 1f, -1f, 1f);
        OnPointerDragEvent?.Invoke(xMovement);
    }

    private void EndPointer()
    {
        isPointerDown = false;
        activeFingerId = -1;
        OnPointerUpEvent?.Invoke();
    }

    private void DisableLegacySlider()
    {
        Slider slider = GetComponent<Slider>();

        if (slider != null)
            slider.enabled = false;

        Graphic[] graphics = GetComponentsInChildren<Graphic>();

        for (int i = 0; i < graphics.Length; i++)
            graphics[i].enabled = false;
    }
}
