using RollToFinal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RollToFinal
{
    public class EffectLockOddsTo6 : MonoBehaviour, IEffectBase
    {
        public string Name { get => "锁定点数为6"; }
        public string Description { get => ""; }

        public int LifeCycle = 0;

        public int TargetPlayer = 0;

        public void OnInstantiated(GameObject player, object data)
        {
            int rollResult = (int)data;
            // 确认目标
            if (rollResult > 3)
            {
                TargetPlayer = GameLogic.Instance.CurrentPlayer == 1 ? 2 : 1;
            }
            else
            {
                TargetPlayer = GameLogic.Instance.CurrentPlayer == 1 ? 1 : 2;
            }
            // 使冲突效果失效
            var effect1 = GameLogic.Instance.Effects.GetComponentsInChildren<EffectLockOddsTo6>();
            foreach(var e in effect1)
            {
                if(e.TargetPlayer == TargetPlayer)
                {
                    e.OnLapsed();
                }
            }
            var effect2 = GameLogic.Instance.Effects.GetComponentsInChildren<EffectLimitOddsMaxTo4>();
            foreach (var e in effect2)
            {
                if (e.TargetPlayer == TargetPlayer)
                {
                    e.OnLapsed();
                }
            }
            // 产生效果
            if (TargetPlayer == 1)
            {
                var delta = GameLogic.Instance.Player1OddsDelta;
                for (int i = 0; i < 5; i++)
                    delta[i] -= 100f;
                for (int i = 6; i < delta.Count; i++)
                    delta[i] -= 100f;
                delta[5] += 100f;
                GameLogic.Instance.Player1OddsDelta = delta;
            }
            else
            {
                var delta = GameLogic.Instance.Player2OddsDelta;
                for (int i = 0; i < 5; i++)
                    delta[i] -= 100f;
                for (int i = 6; i < delta.Count; i++)
                    delta[i] -= 100f;
                delta[5] += 100f;
                GameLogic.Instance.Player2OddsDelta = delta;
            }
        }

        void IEffectBase.Register(IEffectBase.TurnStartCallBack start, IEffectBase.TurnEndCallBack end, IEffectBase.LifeCycleCallBack lc)
        {
            lc += OnLifeCycleCallBack;
            return;
        }

        private void OnLifeCycleCallBack()
        {
            LifeCycle++;
            if (LifeCycle >= 2)
            {
                OnLapsed();
            }
        }

        public void OnLapsed()
        {
            if (TargetPlayer == 1)
            {
                var delta = GameLogic.Instance.Player1OddsDelta;
                for (int i = 0; i < 5; i++)
                    delta[i] += 100f;
                for (int i = 6; i < delta.Count; i++)
                    delta[i] += 100f;
                delta[5] -= 100f;
                GameLogic.Instance.Player1OddsDelta = delta;
            }
            else
            {
                var delta = GameLogic.Instance.Player2OddsDelta;
                for (int i = 0; i < 5; i++)
                    delta[i] += 100f;
                for (int i = 6; i < delta.Count; i++)
                    delta[i] += 100f;
                delta[5] -= 100f;
                GameLogic.Instance.Player2OddsDelta = delta;
            }
            Destroy(this);
        }
    }
}

