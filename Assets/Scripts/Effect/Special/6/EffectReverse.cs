using RollToFinal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RollToFinal
{
    public class EffectReverse : MonoBehaviour, IEffectBase
    {
        public string Name { get => "方向反转"; }
        public string Description { get => ""; }

        public int LifeCycle = 0;

        public int TargetPlayer = 0;

        public void OnInstantiated(GameObject player, object data)
        {
            int rollResult = (int)data;
            if(rollResult >3)
            {
                TargetPlayer = GameLogic.Instance.CurrentPlayer == 1 ? 2 : 1;
            }
            else
            {
                TargetPlayer = GameLogic.Instance.CurrentPlayer == 1 ? 1 : 2;
            }
            if(TargetPlayer == 1)
            {
                DataSystem.Instance.SetData("Player1Reverse", (int)DataSystem.Instance.GetData("Player1Reverse") + 1);
            }
            else
            {
                DataSystem.Instance.SetData("Player2Reverse", (int)DataSystem.Instance.GetData("Player2Reverse") + 1);
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
            if(LifeCycle >= 4)
            {
                OnLapsed();
            }
                
        }

        public void OnLapsed()
        {
            if (TargetPlayer == 1)
            {
                DataSystem.Instance.SetData("Player1Reverse", (int)DataSystem.Instance.GetData("Player1Reverse") - 1);
            }
            else
            {
                DataSystem.Instance.SetData("Player2Reverse", (int)DataSystem.Instance.GetData("Player2Reverse") - 1);
            }
            Destroy(this);
        }
    }
}
