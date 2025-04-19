using System.Collections.Generic;
using UnityEngine;

namespace Ardot.REPO.EnemyOverhaul;

public class PlayerAggro
{
    public PlayerAvatar Player;
    public float Aggro;
}

public class AggroHandler
{
    public List<PlayerAggro> AggroList = new ();

    public PlayerAggro GetHighestAggro()
    {
        PlayerAggro mostAggressive = null;

        for(int x = 0; x < AggroList.Count; x++)
        {
            PlayerAggro aggro = AggroList[x];

            if(mostAggressive == null || aggro.Aggro > mostAggressive.Aggro)
                mostAggressive = aggro;
        }

        return mostAggressive;
    }

    public void LoseAggro(float amount)
    {
        for(int x = 0; x < AggroList.Count; x++)
            AggroList[x].Aggro = Mathf.Max(AggroList[x].Aggro - amount, 0);
    }

    public PlayerAggro GetAggro(PlayerAvatar playerAvatar)
    {
        int index = -1;
        for(int x = 0; x < AggroList.Count; x++)
        {
            if(AggroList[x].Player == playerAvatar)
            {
                index = x;
                break;
            }
        }
                
        if(index == -1)
        {
            index = AggroList.Count;
            AggroList.Add(new PlayerAggro(){Player = playerAvatar, Aggro = 0});
        }

        return AggroList[index];
    }
}