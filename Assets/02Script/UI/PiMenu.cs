using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PiMenu : MonoBehaviour
{
    private RectTransform rect;

    private List<PiMenuButton> buttons;
    private List<float> angleCut;

    private void Awake()
    {

        if (!TryGetComponent<RectTransform>(out rect))
        {
            Debug.Log("PiMenu - Awake -  RectTransfrom");
        }

        buttons = new List<PiMenuButton>();

        for(int i = 0; i < transform.childCount; i++)
        {
            PiMenuButton btn;
            if(transform.GetChild(i).TryGetComponent<PiMenuButton>(out btn))
            {
                buttons.Add(btn);
            }
            else
            {
                Debug.Log("PiMenu - Awake - PiMenuButton: " + i + " Child");
            }
        }

        SetPosition();
    }

    private void SetPosition()
    {
        float halfradian = Mathf.PI / buttons.Count;
        float radian = halfradian * 2; // 360 / count * pi / 180

        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].transform.position = new Vector3(150 * Mathf.Sin(radian * i), 150 * Mathf.Cos(radian * i)) + transform.position;
        }

        angleCut = new List<float>();
        for (int i = 0; i < buttons.Count; i++)
        {
            angleCut.Add(radian * i + halfradian);
        }
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void Enable()
    {
        foreach (PiMenuButton button in buttons)
        {
            button.Enable();
        }
    }

}
