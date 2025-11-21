using UnityEngine;
using TMPro;

[ExecuteInEditMode] // ВАЖНО: работает в редакторе
public class name : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 1.8f, 0);
    [SerializeField] private float scale = 0.08f;

    private Camera cam;
    private Canvas canvas;

    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        cam = Camera.main;
        canvas = GetComponent<Canvas>();

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (canvas != null) canvas.enabled = false;
            return;
        }
#endif

        if (canvas != null) canvas.enabled = true;
        transform.localScale = Vector3.one * scale;
    }

    private void LateUpdate()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif

        if (target != null && cam != null)
        {
            transform.position = target.position + offset;
            transform.rotation = cam.transform.rotation;
        }
    }
}
