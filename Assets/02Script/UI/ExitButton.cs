using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButton : PiMenuButton
{
    public override void Enable()
    {
        if(GameManager.Inst.MapData.GetCurrentMap() == (int)TileType.Exit)
        {
            base.Enable();
        }
        else
        {
            Disable();
        }
    }

}
