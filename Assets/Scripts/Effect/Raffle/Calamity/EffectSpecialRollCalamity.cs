using UnityEngine;

namespace RollToFinal
{
    public class EffectSpecialRollCalamity : MonoBehaviour, IEffectBase
    {
        public string Name { get => "诅咒I"; }
        public string Description { get => "下一次特殊骰子必定为厄运"; }

        IEffectBase.EffectType IEffectBase.Type { get => IEffectBase.EffectType.Calamity; }

        int IEffectBase.Target { get; set; }

        void IEffectBase.OnInstantiated(object[] data)
        {
            // 确认目标
            ((IEffectBase)this).Target = GameLogic.Instance.CurrentPlayer == 1 ? 1 : 2;
        }

        void IEffectBase.OnAssert()
        {
            // 产生效果
            int count = DataSystem.Instance.GetData(((IEffectBase)this).Target == 1 ? "Player1SpecialRollBalance" : "Player2SpecialRollBalance");
            DataSystem.Instance.SetData(((IEffectBase)this).Target == 1 ? "Player1SpecialRollBalance" : "Player2SpecialRollBalance", count - 1);
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

