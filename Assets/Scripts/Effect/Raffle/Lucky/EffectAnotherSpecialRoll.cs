using UnityEngine;

namespace RollToFinal
{
    public class EffectAnotherSpecialRoll : MonoBehaviour, IEffectBase
    {
        public string Name { get => "恩赐II"; }
        public string Description { get => "获得额外一个特殊骰子"; }

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
            int count = DataSystem.Instance.GetData(((IEffectBase)this).Target == 1 ? "Player1SpecialRollCount" : "Player2SpecialRollCount");
            DataSystem.Instance.SetData(((IEffectBase)this).Target == 1 ? "Player1SpecialRollCount" : "Player2SpecialRollCount", count + 1);
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

