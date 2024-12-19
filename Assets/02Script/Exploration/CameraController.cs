using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    private Camera renderCamera;

    private EventTrigger trigger;
    private EventTrigger.Entry drag;
    private EventTrigger.Entry dragEnd;

    private Vector2 dragStart;
    private bool draging;
    [SerializeField]
    private float speed = 0.05f;

    private void Awake()
    {
        GameObject cam = GameObject.Find("MapCamera");
        if (cam == null || !cam.transform.TryGetComponent<Camera>(out renderCamera))
        {
            Debug.Log("DragMap - Awake -  Camera");
        }
        if (!TryGetComponent<EventTrigger>(out trigger))
        {
            Debug.Log("DragMap - Awake -  EventTrigger");
        }

        drag = new EventTrigger.Entry();
        drag.eventID = EventTriggerType.Drag;
        drag.callback.AddListener((data) => { OnDrag((PointerEventData)data); });
        trigger.triggers.Add(drag);
        dragEnd = new EventTrigger.Entry();
        dragEnd.eventID = EventTriggerType.EndDrag;
        dragEnd.callback.AddListener((data) => { OnEndDrag((PointerEventData)data); });
        trigger.triggers.Add(dragEnd);

        dragStart = Vector3.zero;
        draging = false;
    }
    private void Update()
    {
        float scroll = Input.mouseScrollDelta.y;

        if (renderCamera.orthographicSize <= 5 && scroll > 0)
        {
            renderCamera.orthographicSize = 5;
        }
        else if (renderCamera.orthographicSize >= 40 && scroll < 0)
        {
            renderCamera.orthographicSize = 40;
        }
        else
        {
            renderCamera.orthographicSize -= scroll;
        }
    }

    public void SetPosition(Vector3 position)
    {
        renderCamera.transform.position = position;
    }

    private void OnDrag(PointerEventData data)
    {
        if (!draging)
        {
            dragStart = data.position;
            draging = true;
        }
        else
        {
            Vector2 delta = data.position - dragStart;
            renderCamera.transform.position -= new Vector3(delta.x, delta.y, 0) * speed;
            dragStart = data.position;
        }
    }
    private void OnEndDrag(PointerEventData data)
    {
        draging = false;
    }

    public void OnEnable()
    {
        if (trigger != null)
            trigger.enabled = true;
    }

    public void OnDisable()
    {
        if (trigger != null)
            trigger.enabled = false;
    }
}
