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

        void IEffectBase.OnInstantiated(GameObject player, object[] data)
        {
            int rollResult = (int)data[0];
            // 确认目标
            if (rollResult > 3)
            {
                TargetPlayer = GameLogic.Instance.CurrentPlayer == 1 ? 1 : 2;
            }
            else
            {
                TargetPlayer = GameLogic.Instance.CurrentPlayer == 1 ? 2 : 1;
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

