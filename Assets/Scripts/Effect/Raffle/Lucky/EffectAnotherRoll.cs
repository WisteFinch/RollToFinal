using UnityEngine;

namespace RollToFinal
{
    public class EffectAnotherRoll : MonoBehaviour, IEffectBase
    {
        public string Name { get => "恩赐I"; }
        public string Description { get => "获得额外一个普通骰子"; }

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
            GameLogic.Instance.CurrentGameState = GameLogic.GameState.PlayerIdle;
            GameLogic.Instance.TempGameState = GameLogic.GameState.PlayerIdle;
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

