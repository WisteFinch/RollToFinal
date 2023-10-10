using UnityEngine;

namespace RollToFinal
{
    public class EffectOddsLevelRiseRaffle : MonoBehaviour, IEffectBase
    {
        public string Name { get => "恩赐III"; }
        public string Description { get => "使骰子等级上升一级"; }

        IEffectBase.EffectType IEffectBase.Type { get => IEffectBase.EffectType.Lucky; }

        int IEffectBase.Target { get; set; }

        public GameObject EffectMove;

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
                if (GameLogic.Instance.Player1OddsLevel < 4)
                    GameLogic.Instance.Player1OddsLevel++;
            }
            else
            {
                if (GameLogic.Instance.Player2OddsLevel < 4)
                    GameLogic.Instance.Player2OddsLevel++;
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

