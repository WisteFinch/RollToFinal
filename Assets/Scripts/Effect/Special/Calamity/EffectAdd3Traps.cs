using System.Linq;
using UnityEngine;

namespace RollToFinal
{
    public class EffectAdd3Traps : MonoBehaviour, IEffectBase
    {
        public string Name { get => "前方生成三个陷阱"; }
        public string Description { get => $"在玩家{((IEffectBase)this).Target}面前生成3个陷阱"; }

        IEffectBase.EffectType IEffectBase.Type { get => IEffectBase.EffectType.Calamity; }

        int IEffectBase.Target { get; set; }

        public GameObject Trap;

        void IEffectBase.OnInstantiated(object[] data)
        {
            // 确认目标
            ((IEffectBase)this).Target = GameLogic.Instance.CurrentPlayer == 1 ? 1 : 2;
        }

        void IEffectBase.OnAssert()
        {
            int count = 3;
            // 产生效果
            int progress = ((IEffectBase)this).Target == 1 ? GameLogic.Instance.Player1Progress : GameLogic.Instance.Player2Progress;
            int index = progress + 1;
            while (count > 0 && index <= GameLogic.Instance.Length && index - progress <= 10)
            {
                if (GameLogic.Instance.PlatformBlocks[index].GetComponent<Block>().Type == Block.BlockType.Normal)
                {
                    count--;
                    GameLogic.Instance.ReplaceBlock(GameLogic.Instance.PlatformBlocks[index], Trap);
                }
                index++;
            }

            // 使自身失效
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
