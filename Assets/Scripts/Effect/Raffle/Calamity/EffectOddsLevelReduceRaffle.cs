using UnityEngine;

namespace RollToFinal
{
    public class EffectOddsLevelReduceRaffle : MonoBehaviour, IEffectBase
    {
        public string Name { get => "诅咒II"; }
        public string Description { get => "使骰子等级下降一级"; }

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
            if (((IEffectBase)this).Target == 1)
            {
                if (GameLogic.Instance.Player1OddsLevel > 0)
                    GameLogic.Instance.Player1OddsLevel--;
            }
            else
            {
                if (GameLogic.Instance.Player2OddsLevel > 0)
                    GameLogic.Instance.Player2OddsLevel--;
            }
            GameLogic.Instance.CalcOdds();
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

