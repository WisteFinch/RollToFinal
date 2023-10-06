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

        IEffectBase.EffectType IEffectBase.Type { get => IEffectBase.EffectType.Gain; }

        int IEffectBase.Target { get; set; }

        public int LifeCycle = 0;

        public string UUID;

        void IEffectBase.OnInstantiated(object[] data)
        {
            UUID = System.Guid.NewGuid().ToString();
            int rollResult = (int)data[0];
            // 确认目标
            ((IEffectBase)this).Target = rollResult > 3 ? (GameLogic.Instance.CurrentPlayer == 1 ? 1 : 2) : (GameLogic.Instance.CurrentPlayer == 1 ? 2 : 1);
        }

        void IEffectBase.OnAssert()
        {
            // 使冲突效果失效
            var effect1 = GameLogic.Instance.Effects.GetComponentsInChildren<EffectRollResultBonus>();
            foreach (var e in effect1)
            {
                if (((IEffectBase)e).Target == ((IEffectBase)this).Target && e.UUID != this.UUID)
                {
                    ((IEffectBase)e).OnLapsed();
                }
            }
            // 产生效果
            if (((IEffectBase)this).Target == 1)
            {
                DataSystem.Instance.SetData("Player1RollResultDelta", (int)DataSystem.Instance.GetData("Player1RollResultDelta") + 1);
            }
            else
            {
                DataSystem.Instance.SetData("Player2RollResultDelta", (int)DataSystem.Instance.GetData("Player2RollResultDelta") + 1);
            }
        }

        void IEffectBase.Register(ref IEffectBase.TurnStartCallBack start, ref IEffectBase.TurnEndCallBack end, ref IEffectBase.LifeCycleCallBack lc)
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
            if (((IEffectBase)this).Target == 1)
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

