using UnityEngine;

namespace RollToFinal
{
    public class EffectCalamityBalance : MonoBehaviour, IEffectBase
    {
        public string Name { get => "抵消下一次损耗"; }
        public string Description { get => ""; }

        IEffectBase.EffectType IEffectBase.Type { get => IEffectBase.EffectType.Lucky; }

        int IEffectBase.Target { get; set; }

        void IEffectBase.OnInstantiated(object[] data)
        {
            // 确认目标
            ((IEffectBase)this).Target = GameLogic.Instance.CurrentPlayer == 1 ? 1 : 2;
        }

        void IEffectBase.OnAssert()
        {
            // 产生效果
            if (((IEffectBase)this).Target == 1)
            {
                GameLogic.Instance.Player1CalamityBalance += 2;
            }
            else
            {
                GameLogic.Instance.Player2CalamityBalance += 2;
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
