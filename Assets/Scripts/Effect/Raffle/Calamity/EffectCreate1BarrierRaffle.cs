using UnityEngine;

namespace RollToFinal
{
    public class EffectCreate1BarrierRaffle : MonoBehaviour, IEffectBase
    {
        public string Name { get => "命运多舛"; }
        public string Description { get => "面前刷新一个障碍物"; }

        IEffectBase.EffectType IEffectBase.Type { get => IEffectBase.EffectType.Calamity; }

        int IEffectBase.Target { get; set; }

        public GameObject BarrierPerfab;

        void IEffectBase.OnInstantiated(object[] data)
        {
            // 确认目标
            ((IEffectBase)this).Target = GameLogic.Instance.CurrentPlayer == 1 ? 1 : 2;

        }

        void IEffectBase.OnAssert()
        {
            // 产生效果
            int progress = ((IEffectBase)this).Target == 1 ? GameLogic.Instance.Player1Progress : GameLogic.Instance.Player2Progress;
            int index = progress + 1;
            if (index < GameLogic.Instance.Length + 1)
            {
                GameLogic.Instance.ReplaceBlock(GameLogic.Instance.PlatformBlocks[index], BarrierPerfab);
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


