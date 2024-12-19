using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private float reconnaissancePersent;
    private PlayerController player;
    private MenuManager menuManager;
    private Camera camera;

    private MapPainter mapPainter;

    private Vector2Int playerPosition;
    private MapData data;


    private void Awake()
    {
        GameObject p = GameObject.Find("Player");
        if(p == null || !p.transform.TryGetComponent<PlayerController>(out player))
        {
            Debug.Log("MapManager - Awake - PlayerController");
        }

        GameObject cam = GameObject.Find("MapCamera");
        if (cam == null || !cam.transform.TryGetComponent<Camera>(out camera))
        {
            Debug.Log("DragMap - Awake -  Camera");
        }
        if (!TryGetComponent<MapPainter>(out mapPainter))
        {
            Debug.Log("MapManager - Awake - mapPainter");
        }

        reconnaissancePersent = 30;
    }

    private void Start()
    {
        StartCoroutine(CheckFinishMap());
    }

    private IEnumerator CheckFinishMap()
    {
        while(data == null)
        {
            yield return null;
            data = GameManager.Inst.MapData;
        }
        while (!data.Access)
        {
            yield return null;
        }

        playerPosition = data.GetPosition();
        player.transform.position = new Vector3(playerPosition.x * 12 + 6, playerPosition.y * 12 + 6, 0);
        camera.transform.position = new Vector3(playerPosition.x * 12 + 6, playerPosition.y * 12 + 6, -1);
        Reconnaissance();
        mapPainter.DrawMap();
        player.Control = true;
    }

    private void Reconnaissance()
    {
        if(Random.Range(0,100) < reconnaissancePersent)
        {
            data.UpdateKnownMap(); 
            mapPainter.DrawMap();
        }
    }

    public void Search()
    {
        player.Control = false;
        data.Search();
        mapPainter.DrawMap();
        player.Control= true;
    }

    private void Notify()
    {
        if (!data.GetGone(playerPosition.x, playerPosition.y))
        {
            switch (data.GetMap(playerPosition.x, playerPosition.y))
            {
                case 5:
                    Debug.Log("Exit");
                    break;
                case 6:
                    Debug.Log("Boss");
                    break;
                case 7:
                    Debug.Log("Monster");
                    break;
                case 8:
                    Debug.Log("Chest");
                    break;
                case 9:
                    Debug.Log("Positive");
                    break;
                case 10:
                    Debug.Log("Negative");
                    break;
            }
        }
    }


    public void MovePlayer(Direction direction)
    {
        if (data.MovePlayer(direction))
        {
            player.Control = false;
            playerPosition = data.GetPosition();
            Notify();
            data.UpdateGoneMap();
            mapPainter.DrawMap();
            if((direction == Direction.Left || direction == Direction.Right) && direction != player.Look)
            {
                player.Look = direction;
            }
            StartCoroutine(RunPlayer());
        }
    }

    private IEnumerator RunPlayer()
    {
        camera.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -1);
        camera.orthographicSize = 15;
        player.PlayAnimation(CharacterStateDC.Run);
        player.transform.LeanMove(new Vector3(playerPosition.x * 12 + 6, playerPosition.y * 12 + 6, 0), 1);
        camera.transform.LeanMove(new Vector3(playerPosition.x * 12 + 6, playerPosition.y * 12 + 6, -1), 1);
        yield return YieldInstructionCache.WaitForSeconds(1);
        player.PlayAnimation(CharacterStateDC.Idle);
        player.Control = true;
        Reconnaissance();
    }
    
}
