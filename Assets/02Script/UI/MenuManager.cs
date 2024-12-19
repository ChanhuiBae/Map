using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    private PiMenu pimenu;
    private PlayerController player;
    private CameraController camController;
    private MapManager mapManager;

    private Image latern;

    private void Awake()
    {
        GameObject menu = GameObject.Find("PiMenu");
        if(menu == null || !menu.TryGetComponent<PiMenu>(out pimenu))
        {
            Debug.Log("MenuManager - Awake - PiMenu");
        }
        GameObject p = GameObject.Find("Player");
        if (p == null || !p.transform.TryGetComponent<PlayerController>(out player))
        {
            Debug.Log("MenuManager - Awake - PlayerController");
        }
        GameObject c = GameObject.Find("CameraController");
        if(c == null || !c.transform.TryGetComponent<CameraController>(out camController))
        {
            Debug.Log("MenuManager - Awake - CameraController");
        }
        GameObject mm = GameObject.Find("ExplorationManager");
        if(mm == null || !mm.transform.TryGetComponent<MapManager>(out mapManager))
        {
            Debug.Log("MenuManager - Awake - MapManager");
        }
    }

    private void Start()
    {
        if(pimenu != null)
        {
            pimenu.Disable();
        }
    }

    private void Update()
    {
        if (GameManager.Inst.MapData.Access
            && player.State == CharacterStateDC.Idle 
            && player.Control
            && Input.GetKeyDown(KeyCode.Tab))
        {
            player.Control = false;
            camController.enabled = false;
            pimenu.gameObject.SetActive(true);
            pimenu.Enable();
        }

        if (GameManager.Inst.MapData.Access
        && player.State == CharacterStateDC.Idle
        && !player.Control
        && Input.GetKeyUp(KeyCode.Tab))
        {
            camController.enabled = true;
            player.Control = true;
            pimenu.Disable();
        }
    }

    public void Exit()
    {
        camController.enabled = true;
        player.Control = true;
    }

    public void ShowBag()
    {
        camController.enabled = true;
        player.Control = true;
    }

    public void Campfire()
    {
        camController.enabled = true;
        player.Control = true;
    }

    public void ShowCharactersInfo()
    {
        camController.enabled = true;
        player.Control = true;
    }

    public void Search()
    {
        mapManager.Search();
        camController.enabled = true;
        player.Control = true;
    }

}
