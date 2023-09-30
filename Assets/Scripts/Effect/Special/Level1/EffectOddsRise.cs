using UnityEngine;

namespace RollToFinal
{
    public class EffectOddsRise : MonoBehaviour, IEffectBase
    {
        public string Name { get => "点数分布提高"; }
        public string Description { get => ""; }

        public int LifeCycle = 0;

        public int TargetPlayer = 0;

        void IEffectBase.OnInstantiated(GameObject player, object[] data)
        {
            int rollResult = (int)data[0];
            // 确认目标
            TargetPlayer = rollResult > 3 ? (GameLogic.Instance.CurrentPlayer == 1 ? 1 : 2) : (GameLogic.Instance.CurrentPlayer == 1 ? 2 : 1);
            // 产生效果
            if (TargetPlayer == 1)
            {
                var delta = GameLogic.Instance.Player1OddsDelta;
                for (int i = 0; i < 3; i++)
                    delta[i] -= 5f;
                for (int i = 3; i < delta.Count; i++)
                    delta[i] += 5f;
            }
            else
            {
                var delta = GameLogic.Instance.Player1OddsDelta;
                for (int i = 0; i < 3; i++)
                    delta[i] -= 5f;
                for (int i = 3; i < delta.Count; i++)
                    delta[i] += 5f;
            }
            // 使自身失效
            ((IEffectBase)this).OnLapsed();
        }

        void IEffectBase.Register(IEffectBase.TurnStartCallBack start, IEffectBase.TurnEndCallBack end, IEffectBase.LifeCycleCallBack lc)
        {
            return;
        }

        void IEffectBase.OnLapsed()
        {
            Destroy(this.gameObject);
        }
    }
}

