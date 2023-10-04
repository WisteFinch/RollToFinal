using RollToFinal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RollToFinal
{
    public class EffectRollResultBonus : MonoBehaviour, IEffectBase
    {
        public string Name { get => "点数奖励"; }
        public string Description { get => ""; }

        public int LifeCycle = 0;

        public int TargetPlayer = 0;

        public string UUID;

        void IEffectBase.OnInstantiated(GameObject player, object[] data)
        {
            UUID = System.Guid.NewGuid().ToString();
            int rollResult = (int)data[0];
            // 确认目标
            TargetPlayer = rollResult > 3 ? (GameLogic.Instance.CurrentPlayer == 1 ? 1 : 2) : (GameLogic.Instance.CurrentPlayer == 1 ? 2 : 1);
            // 使冲突效果失效
            var effect1 = GameLogic.Instance.Effects.GetComponentsInChildren<EffectRollResultBonus>();
            foreach (var e in effect1)
            {
                if (e.TargetPlayer == TargetPlayer && e.UUID != this.UUID)
                {
                    ((IEffectBase)e).OnLapsed();
                }
            }
            // 产生效果
            if (TargetPlayer == 1)
            {
                DataSystem.Instance.SetData("Player1RollResultDelta", (int)DataSystem.Instance.GetData("Player1RollResultDelta") + 1);
            }
            else
            {
                DataSystem.Instance.SetData("Player2RollResultDelta", (int)DataSystem.Instance.GetData("Player2RollResultDelta") + 1);
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
            if (LifeCycle >= 6)
            {
                ((IEffectBase)this).OnLapsed();
            }
        }

        void IEffectBase.OnLapsed()
        {
            if (TargetPlayer == 1)
            {
                DataSystem.Instance.SetData("Player1RollResultDelta", (int)DataSystem.Instance.GetData("Player1RollResultDelta") - 1);
            }
            else
            {
                DataSystem.Instance.SetData("Player2RollResultDelta", (int)DataSystem.Instance.GetData("Player2RollResultDelta") - 1);
            }
            Destroy(this.gameObject);
        }
    }
}

