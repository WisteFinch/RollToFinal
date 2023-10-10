using UnityEngine;

namespace RollToFinal
{
    public class EffectMove : MonoBehaviour, IEffectBase
    {
        public string Name { get => "移动"; }
        public string Description { get => $"玩家{((IEffectBase)this).Target}获得{Movement}点行动力"; }

        IEffectBase.EffectType IEffectBase.Type { get => IEffectBase.EffectType.Gain; }
        int IEffectBase.Target { get; set; }

        public int Movement = 0;

        void IEffectBase.OnInstantiated(object[] data)
        {
            Movement = (int)data[0];
            ((IEffectBase)this).Target = (int)data[1];
        }

        void IEffectBase.OnAssert()
        {
            var instance = GameLogic.Instance;
            var player = ((IEffectBase)this).Target == 1 ? instance.Player1 : instance.Player2;
            //Vector3 pos = player.transform.position;
            int stepSize = (((IEffectBase)this).Target == 1 ? DataSystem.Instance.GetData("Player1Reverse") : DataSystem.Instance.GetData("Player2Reverse")) == 1 ? -1 : 1;
            int progress = ((IEffectBase)this).Target == 1 ? instance.Player1Progress : instance.Player2Progress;
            //var platform = instance.PlatformBlocks;

            if ((((IEffectBase)this).Target == 1 ? DataSystem.Instance.GetData("Player1ForzenMove") : DataSystem.Instance.GetData("Player2ForzenMove")) > 0)
                Movement = 0;

            if (progress == 0 && stepSize == -1)
                Movement = 0;

            //// 判断方块
            //while (Movement > 0)
            //{
            //    if (progress + stepSize < 0)
            //        break;
            //    Block.BlockType type = platform[progress + stepSize].GetComponent<Block>().Type;
            //    if (type == Block.BlockType.Barrier && platform[progress].GetComponent<Block>().Type != Block.BlockType.Barrier)
            //    {
            //        if (Movement >= 3)
            //        {
            //            progress += stepSize;
            //        }
            //        Movement -= 3;
            //    }
            //    else if (type == Block.BlockType.EndLine)
            //    {
            //        progress += stepSize;
            //        instance.Win();
            //        break;
            //    }
            //    else if (type == Block.BlockType.Start)
            //    {
            //        progress += stepSize;
            //        break;
            //    }
            //    else
            //    {
            //        Movement--;
            //        progress += stepSize;
            //    }
            //}
            //// 判断落脚
            //if (platform[progress].GetComponent<Block>().Type == Block.BlockType.Barrier)
            //    pos.y = 3;
            //else
            //    pos.y = 0;
            //if (platform[progress].GetComponent<Block>().Type == Block.BlockType.Empty)
            //{
            //    progress = ((IEffectBase)this).Target == 1 ? instance.Player1Progress : instance.Player2Progress;
            //}

            //pos.z = progress;
            //if (((IEffectBase)this).Target == 1)
            //    instance.Player1Progress = progress;
            //else
            //    instance.Player2Progress = progress;
            //player.transform.position = pos;
            //instance.UpdateProgress();

            player.GetComponent<PlayerController>().Jump(Movement);

            ((IEffectBase)this).OnLapsed();
        }

        void IEffectBase.Register()
        {
            return;
        }

        void IEffectBase.OnLapsed()
        {
            Destroy(this.gameObject);
        }
    }
}