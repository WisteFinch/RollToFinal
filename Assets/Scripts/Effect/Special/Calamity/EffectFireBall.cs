using System.Linq;
using UnityEngine;

namespace RollToFinal
{
    public class EffectFireBall : MonoBehaviour, IEffectBase
    {
        public string Name { get => "召唤火球"; }
        public string Description { get => ""; }

        IEffectBase.EffectType IEffectBase.Type { get => IEffectBase.EffectType.Calamity; }

        int IEffectBase.Target { get; set; }

        public GameObject Empty;

        public int TargetIndex;

        void IEffectBase.OnInstantiated(object[] data)
        {
            // 确认目标
            ((IEffectBase)this).Target = GameLogic.Instance.CurrentPlayer == 1 ? 1 : 2;
        }

        void IEffectBase.OnAssert()
        {
            int count = 3;
            // 产生效果
            TargetIndex = (((IEffectBase)this).Target == 1 ? GameLogic.Instance.Player1Progress : GameLogic.Instance.Player2Progress) + Random.Range(1, 7);
            int index = TargetIndex;
            while (count <= 0 && index <= GameLogic.Instance.Length)
            {
                GameLogic.Instance.ReplaceBlock(GameLogic.Instance.PlatformBlocks[index], Empty);
                count--;
                index++;
            }

            // 使自身失效
            ((IEffectBase)this).OnLapsed();
        }

        void IEffectBase.Register(ref IEffectBase.TurnStartCallBack start, ref IEffectBase.TurnEndCallBack end, ref IEffectBase.LifeCycleCallBack lc)
        {
            return;
        }

        void IEffectBase.OnLapsed()
        {
            Destroy(this.gameObject);
        }
    }
}
