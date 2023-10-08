using RollToFinal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RollToFinal
{
    public class EffectJudgeFail5 : MonoBehaviour, IEffectBase
    {
        public string Name { get => "判定失败"; }
        public string Description { get => ""; }

        IEffectBase.EffectType IEffectBase.Type { get => IEffectBase.EffectType.Calamity; }

        int IEffectBase.Target { get; set; }

        public int LifeCycle = 0;

        public string UUID;

        void IEffectBase.OnInstantiated(object[] data)
        {
            UUID = System.Guid.NewGuid().ToString();
            int rollResult = (int)data[0];
            // 确认目标
            ((IEffectBase)this).Target = rollResult > 3 ? (GameLogic.Instance.CurrentPlayer == 1 ? 2 : 1) : (GameLogic.Instance.CurrentPlayer == 1 ? 1 : 2);
        }

        void IEffectBase.OnAssert()
        {
            // 使冲突效果失效
            var effect = GameLogic.Instance.Effects.GetComponentsInChildren<EffectJudgeFail5>();
            foreach (var e in effect)
            {
                if (((IEffectBase)e).Target == ((IEffectBase)this).Target && e.UUID != this.UUID)
                {
                    ((IEffectBase)e).OnLapsed();
                }
            }
            var effect1 = GameLogic.Instance.Effects.GetComponentsInChildren<EffectJudgeFail>();
            foreach (var e in effect1)
            {
                if (((IEffectBase)e).Target == ((IEffectBase)this).Target)
                {
                    ((IEffectBase)e).OnLapsed();
                }
            }
            var effect2 = GameLogic.Instance.Effects.GetComponentsInChildren<EffectJudgeSuccess>();
            foreach (var e in effect2)
            {
                if (((IEffectBase)e).Target == ((IEffectBase)this).Target)
                {
                    ((IEffectBase)e).OnLapsed();
                }
            }
            // 产生效果
            if (((IEffectBase)this).Target == 1)
            {
                DataSystem.Instance.SetData("Player1JudgeBalance", DataSystem.Instance.GetData("Player1JudgeBalance") - 1);
            }
            else
            {
                DataSystem.Instance.SetData("Player2JudgeBalance", DataSystem.Instance.GetData("Player2JudgeBalance") - 1);
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
            if (LifeCycle >= 10)
            {
                ((IEffectBase)this).OnLapsed();
            }
        }

        void IEffectBase.OnLapsed()
        {
            if (((IEffectBase)this).Target == 1)
            {
                DataSystem.Instance.SetData("Player1JudgeBalance", DataSystem.Instance.GetData("Player1JudgeBalance") + 1);
            }
            else
            {
                DataSystem.Instance.SetData("Player2JudgeBalance", DataSystem.Instance.GetData("Player2JudgeBalance") + 1);
            }
            Destroy(this.gameObject);
        }
    }
}

