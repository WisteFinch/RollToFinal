using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RollToFinal
{
    public class EffectMove : MonoBehaviour, IEffectBase
    {
        public void OnInstantiated(GameObject player, object data)
        {
            var pos = player.transform.position;
            int step = (int)data;
            var instance = GameLogic.Instance;
            var currentPlayer = instance.CurrentPlayer;
            // 判断胜利
            if((currentPlayer == 1 ? instance.Player1Progress + step : instance.Player2Progress + step) >= instance.Length + 1)
            {
                instance.Win();
                return;
            }
            // 判断障碍
            while (step > 0)
            {
                if(currentPlayer == 1)
                {
                    if(instance.PlatformBlocks1[instance.Player1Progress + 1].GetComponent<Block>().Type == Block.BlockType.Barrier)
                    {
                        if (step >= 3)
                        {
                            instance.Player1Progress++;
                        }
                        step -= 3;
                    }
                    else
                    {
                        step--;
                        instance.Player1Progress++;
                    }
                        
                }else
                {
                    if (instance.PlatformBlocks2[instance.Player2Progress + 1].GetComponent<Block>().Type == Block.BlockType.Barrier)
                    {
                        if (step >= 3)
                        {
                            instance.Player2Progress++;
                        }
                        step -= 3;
                    }
                    else
                    {
                        step--;
                        instance.Player2Progress++;
                    }

                }
            }
            // 判断落脚
            if ((currentPlayer == 1 ? instance.PlatformBlocks1[instance.Player1Progress].GetComponent<Block>().Type : instance.PlatformBlocks2[instance.Player2Progress].GetComponent<Block>().Type) == Block.BlockType.Barrier)
                pos.y = 3;
            else
                pos.y = 0;
            pos.z = currentPlayer == 1 ? instance.Player1Progress : instance.Player2Progress;

            player.transform.position = pos;
            instance.UpdateProgress();
            Destroy(this.gameObject);
        }

        void IEffectBase.Register(IEffectBase.TurnStartCallBack start, IEffectBase.TurnEndCallBack end, IEffectBase.LifeCycleCallBack lc)
        {
            return;
        }
    }
}