using RollToFinal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RollToFinal
{
    public class EffectLimitOddsMaxTo4 : MonoBehaviour, IEffectBase
    {
        public string Name { get => "降低骰子上限至4"; }
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
            var effects1 = GameLogic.Instance.Effects.GetComponentsInChildren<EffectLimitOddsMaxTo4>();
            foreach(var e in effects1)
            {
                if(e.TargetPlayer == TargetPlayer)
                {
                    e.OnLapsed();
                }
            }
            var effects2 = GameLogic.Instance.Effects.GetComponentsInChildren<EffectLockOddsTo6>();
            foreach (var e in effects2)
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
                for (int i = 4; i < delta.Count; i++)
                    delta[i] -= 100f;
                GameLogic.Instance.Player1OddsDelta = delta;
            }
            else
            {
                var delta = GameLogic.Instance.Player2OddsDelta;
                for (int i = 4; i < delta.Count; i++)
                    delta[i] -= 100f;
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
            if (LifeCycle >= 4)
            {
                OnLapsed();
            }
        }

        public void OnLapsed()
        {
            if (TargetPlayer == 1)
            {
                var delta = GameLogic.Instance.Player1OddsDelta;
                for (int i = 4; i < delta.Count; i++)
                    delta[i] += 100f;
                GameLogic.Instance.Player1OddsDelta = delta;
            }
            else
            {
                var delta = GameLogic.Instance.Player2OddsDelta;
                for (int i = 4; i < delta.Count; i++)
                    delta[i] += 100f;
                GameLogic.Instance.Player2OddsDelta = delta;
            }
            Destroy(this);
        }
    }
}

