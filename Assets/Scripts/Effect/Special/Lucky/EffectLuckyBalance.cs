using UnityEngine;

namespace RollToFinal
{
    public class EffectLuckyBalance : MonoBehaviour, IEffectBase
    {
        public string Name { get => "抵消下一次增益"; }
        public string Description { get => $"抵消玩家{((IEffectBase)this).Target}的下一次增益效果"; }

        IEffectBase.EffectType IEffectBase.Type { get => IEffectBase.EffectType.Calamity; }

        int IEffectBase.Target { get; set; }

        void IEffectBase.OnInstantiated(object[] data)
        {
            // 确认目标
            ((IEffectBase)this).Target = GameLogic.Instance.CurrentPlayer == 1 ? 2 : 1;
        }

        void IEffectBase.OnAssert()
        {
            // 产生效果
            if (((IEffectBase)this).Target == 1)
            {
                GameLogic.Instance.Player1LuckyBalance += 1;
            }
            else
            {
                GameLogic.Instance.Player2LuckyBalance += 1;
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
