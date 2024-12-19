using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharacterController
{
    private MapManager mapManager;

    private bool isControl;
    public bool Control
    {
        set => isControl = value;
        get => isControl;
    }

    protected override void Awake()
    {
        base.Awake();
        isControl = false;

        GameObject m = GameObject.Find("ExplorationManager");
        if(m == null || !m.TryGetComponent<MapManager>(out mapManager))
        {
            Debug.Log("PlayerController - Awake - MapManager");
        }
    }

    private void Update()
    {
        if (isControl && GameManager.Inst.MapData.Access)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                mapManager.MovePlayer(Direction.Left);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                mapManager.MovePlayer(Direction.Right);
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                mapManager.MovePlayer(Direction.Up);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                mapManager.MovePlayer(Direction.Down);
            }
        }
    }
}
