using UnityEngine;

namespace RollToFinal
{
    public class EffectOddsReduce : MonoBehaviour, IEffectBase
    {
        public string Name { get => "点数分布降低"; }
        public string Description { get => $"玩家{((IEffectBase)this).Target}的概率分布降低"; }

        IEffectBase.EffectType IEffectBase.Type { get => IEffectBase.EffectType.Reduce; }

        int IEffectBase.Target { get; set; }

        public int LifeCycle = 0;

        void IEffectBase.OnInstantiated(object[] data)
        {
            int rollResult = (int)data[0];
            // 确认目标
            ((IEffectBase)this).Target = rollResult > 3 ? (GameLogic.Instance.CurrentPlayer == 1 ? 2 : 1) : (GameLogic.Instance.CurrentPlayer == 1 ? 1 : 2);
        }

        void IEffectBase.OnAssert()
        {
            // 产生效果
            if (((IEffectBase)this).Target == 1)
            {
                var delta = GameLogic.Instance.Player1OddsDelta;
                for (int i = 0; i < 3; i++)
                    delta[i] += 10f;
                for (int i = 3; i < delta.Count; i++)
                    delta[i] -= 10f;
            }
            else
            {
                var delta = GameLogic.Instance.Player2OddsDelta;
                for (int i = 0; i < 3; i++)
                    delta[i] += 10f;
                for (int i = 3; i < delta.Count; i++)
                    delta[i] -= 10f;
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

