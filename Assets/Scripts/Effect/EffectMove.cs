using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RollToFinal
{
    public class EffectMove : MonoBehaviour, IEffectBase
    {
        public string Name { get => "移动"; }
        public string Description { get => ""; }

        public void OnInstantiated(GameObject player, object data)
        {
            Vector3 pos = player.transform.position;
            int step = (int)data;
            var instance = GameLogic.Instance;
            int currentPlayer = instance.CurrentPlayer;
            int stepSize = currentPlayer == 1 ? (int)DataSystem.Instance.GetData("Player1Step") : (int)DataSystem.Instance.GetData("Player2Step");
            int progress = currentPlayer == 1 ? instance.Player1Progress : instance.Player2Progress;
            var platform = currentPlayer == 1 ? instance.PlatformBlocks1 : instance.PlatformBlocks2;

            // 判断方块
            while (step > 0)
            {
                Block.BlockType type = platform[progress + stepSize].GetComponent<Block>().Type;
                if (type == Block.BlockType.Barrier && platform[progress].GetComponent<Block>().Type != Block.BlockType.Barrier)
                {
                    if (step >= 3)
                    {
                        progress += stepSize;
                    }
                    step -= 3;
                }
                else if (type == Block.BlockType.EndLine)
                {
                    progress += stepSize;
                    instance.Win();
                    break;
                }
                else if (type == Block.BlockType.Start)
                {
                    progress += stepSize;
                    break;
                }
                else
                {
                    step--;
                    progress += stepSize;
                }
            }
            // 判断落脚
            if (platform[progress].GetComponent<Block>().Type == Block.BlockType.Barrier)
                pos.y = 3;
            else
                pos.y = 0;
            if (platform[progress].GetComponent<Block>().Type == Block.BlockType.Empty)
            {
                progress = currentPlayer == 1 ? instance.Player1Progress : instance.Player2Progress;
            }

            pos.z = progress;
            if(currentPlayer == 1)
                instance.Player1Progress = progress;
            else
                instance.Player2Progress = progress;
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