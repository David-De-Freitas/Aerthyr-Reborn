using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCursorManager : MonoBehaviour
{
    private static CustomCursorManager singleton;
    public static CustomCursorManager Singleton { get { return singleton; } }

    [Header("REFERENCES")]
    [SerializeField] Transform cursorTransform;
    [SerializeField] Transform weelTransform;
    [Header("STATISTICS")]
    [SerializeField] float spinSpeed;

    CursorState state;

    CanvasGroup canvasGroup;

    public enum CursorState
    {
        Hidden,
        Normal,
        WeelSpin
    }

    private void Awake()
    {
        if (singleton != null)
        {
            Destroy(transform.parent.gameObject);
        }
        else
        {
            singleton = this;
            DontDestroyOnLoad(transform.parent.gameObject);
        }
    }

    // Use this for initialization
    void Start()
    {
        Cursor.visible = false;
        canvasGroup = GetComponent<CanvasGroup>();
        SetState(CursorState.Hidden);
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();

    }

    void UpdatePosition()
    {
        if (state != CursorState.Hidden)
        {
            transform.position = Input.mousePosition;
        }
    }

    public void SetState(CursorState newState)
    {
        state = newState;
        switch (state)
        {
            case CursorState.Hidden:
                canvasGroup.alpha = 0;
                Cursor.lockState = CursorLockMode.Locked;
                break;
            case CursorState.Normal:
                canvasGroup.alpha = 1;
                Cursor.lockState = CursorLockMode.None;
                break;
            case CursorState.WeelSpin:
                canvasGroup.alpha = 1;
                Cursor.lockState = CursorLockMode.None;
                break;
            default:
                break;
        }
    }
}
