using UnityEngine;

namespace RollToFinal
{
    public class EffectReverse : MonoBehaviour, IEffectBase
    {
        public string Name { get => "方向反转"; }
        public string Description { get => ""; }

        public int LifeCycle = 0;

        public int TargetPlayer = 0;

        public string UUID;

        void IEffectBase.OnInstantiated(GameObject player, object[] data)
        {
            UUID = System.Guid.NewGuid().ToString();
            int rollResult = (int)data[0];
            // 确定目标
            TargetPlayer = rollResult > 3 ? (GameLogic.Instance.CurrentPlayer == 1 ? 2 : 1) : (GameLogic.Instance.CurrentPlayer == 1 ? 1 : 2);
            // 使冲突效果失效
            var effect1 = GameLogic.Instance.Effects.GetComponentsInChildren<EffectReverse>();
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
                DataSystem.Instance.SetData("Player1Reverse", 1);
            }
            else
            {
                DataSystem.Instance.SetData("Player2Reverse", 1);
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
                ((IEffectBase)this).OnLapsed();
            }
                
        }

        void IEffectBase.OnLapsed()
        {
            if (TargetPlayer == 1)
            {
                DataSystem.Instance.SetData("Player1Reverse", 0);
            }
            else
            {
                DataSystem.Instance.SetData("Player2Reverse", 0);
            }
            Destroy(this.gameObject);
        }
    }
}
