using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PiMenuButton : MonoBehaviour
{
    private PiMenu pimenu;
    private Button button;
    private Image glow;

    private EventTrigger trigger;
    private EventTrigger.Entry enter;
    private EventTrigger.Entry exit;
    private EventTrigger.Entry click;


    private void Awake()
    {
        if(!transform.parent.transform.TryGetComponent<PiMenu>(out pimenu))
        {
            Debug.Log("PiMenuButton - Awake - PiMenu");
        }
        if (!transform.transform.TryGetComponent<Button>(out button))
        {
            Debug.Log("PiMenuButton - Awake - Button");
        }
        if (!TryGetComponent<EventTrigger>(out trigger))
        {
            Debug.Log("PiMenu - Awake -  EventTrigger");
        }
        else
        {
            enter = new EventTrigger.Entry();
            enter.eventID = EventTriggerType.PointerEnter;
            enter.callback.AddListener((data) => { OnPointerEnter((PointerEventData)data); });
            trigger.triggers.Add(enter);

            exit = new EventTrigger.Entry();
            exit.eventID = EventTriggerType.PointerExit;
            exit.callback.AddListener((data) => { OnPointerExit((PointerEventData)data); });
            trigger.triggers.Add(exit);

            click = new EventTrigger.Entry();
            click.eventID = EventTriggerType.PointerClick;
            click.callback.AddListener((data) => { OnClick((PointerEventData)data); });
            trigger.triggers.Add(click);
        }
        if (!transform.GetChild(1).TryGetComponent<Image>(out glow))
        {
            Debug.Log("PiMenuButton - Awake - Image");
        }

    }

    private void OnPointerExit(PointerEventData data)
    {
        StopAllCoroutines();
        glow.color = new Color(255, 255, 255, 0);
    }

    private void OnPointerEnter(PointerEventData data)
    {
        StartCoroutine(Glow());
    }

    private void OnClick(PointerEventData data)
    {
        StopAllCoroutines();
        pimenu.Disable();
    }

    private IEnumerator Glow()
    {
        float a = 0;
        while (true)
        {
            for(a = 0; a < 0.5f; a += 0.001f)
            {
                glow.color = new Color(255, 255, 255, a);
                yield return null;
            }
            for(a = 0.5f; a > 0; a -= 0.001f)
            {
                glow.color = new Color(255, 255, 255, a);
                yield return null;
            }
        }
    }

    public void Disable()
    {
        button.enabled = false;
        trigger.enabled = false;
        glow.color = new Color(0, 0, 0, 0.7f);
    }

    public virtual void Enable()
    {
        button.enabled = true;
        trigger.enabled = true;
        glow.color = new Color(255, 255, 255, 0);
    }


}
