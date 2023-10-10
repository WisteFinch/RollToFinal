using RollToFinal;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RollToFinal
{
    public class EffectJudgeSuccess : MonoBehaviour, IEffectBase
    {
        public string Name { get => "判定成功"; }
        public string Description { get => $"玩家{((IEffectBase)this).Target}的下一次判定必定成功"; }

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
            var effect = GameLogic.Instance.Effects.GetComponentsInChildren<EffectJudgeFail5>();
            if(effect.Count() > 0)
            {
                Destroy(this.gameObject);
                return;
            }
            var effect1 = GameLogic.Instance.Effects.GetComponentsInChildren<EffectJudgeSuccess>();
            foreach (var e in effect1)
            {
                if (((IEffectBase)e).Target == ((IEffectBase)this).Target && e.UUID != this.UUID)
                {
                    ((IEffectBase)e).OnLapsed();
                }
            }
            var effect2 = GameLogic.Instance.Effects.GetComponentsInChildren<EffectJudgeFail>();
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
                DataSystem.Instance.SetData("Player1JudgeBalance", DataSystem.Instance.GetData("Player1JudgeBalance") + 1);
            }
            else
            {
                DataSystem.Instance.SetData("Player2JudgeBalance", DataSystem.Instance.GetData("Player2JudgeBalance") + 1);
            }
        }

        void IEffectBase.Register()
        {
            GameLogic.Instance.LifeCycleCallBack += OnLifeCycleCallBack;
            return;
        }

        private void OnLifeCycleCallBack()
        {
            LifeCycle++;
            if (LifeCycle >= 4)
            {
                GameLogic.Instance.LifeCycleCallBack -= OnLifeCycleCallBack;
                ((IEffectBase)this).OnLapsed();
            }
        }

        void IEffectBase.OnLapsed()
        {
            if (((IEffectBase)this).Target == 1)
            {
                DataSystem.Instance.SetData("Player1JudgeBalance", DataSystem.Instance.GetData("Player1JudgeBalance") - 1);
            }
            else
            {
                DataSystem.Instance.SetData("Player2JudgeBalance", DataSystem.Instance.GetData("Player2JudgeBalance") - 1);
            }
            Destroy(this.gameObject);
        }
    }
}

