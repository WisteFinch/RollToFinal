using UnityEngine;

namespace RollToFinal
{
    public class EffectCalamityProtection : MonoBehaviour, IEffectBase
    {
        public string Name { get => "庇护"; }
        public string Description { get => "使下一次恶魔骰子对自己失效"; }

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
            int count = DataSystem.Instance.GetData(((IEffectBase)this).Target == 1 ? "Player1CalamityBalance" : "Player2CalamityBalance");
            DataSystem.Instance.SetData(((IEffectBase)this).Target == 1 ? "Player1CalamityBalance" : "Player2CalamityBalance", count + 1);
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

