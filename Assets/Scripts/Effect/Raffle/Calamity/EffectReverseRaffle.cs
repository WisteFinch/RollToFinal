using UnityEngine;

namespace RollToFinal
{
    public class EffectReverseRaffle : MonoBehaviour, IEffectBase
    {
        public string Name { get => "迷失"; }
        public string Description { get => "两回合内，反转前进方向"; }

        IEffectBase.EffectType IEffectBase.Type { get => IEffectBase.EffectType.Reduce; }

        int IEffectBase.Target { get; set; }

        public int Threshold = 20;

        public int LifeCycle = 0;

        public string UUID;

        public bool Enable = false;



        void IEffectBase.OnInstantiated(object[] data)
        {
            // 确定目标
            ((IEffectBase)this).Target = GameLogic.Instance.CurrentPlayer == 1 ? 1 : 2;
            if(((IEffectBase)this).Target == 1)
            {
                if (GameLogic.Instance.Player1Progress - GameLogic.Instance.Player2Progress >= Threshold)
                    Enable = true;
            }
            else
            {
                if (GameLogic.Instance.Player2Progress - GameLogic.Instance.Player1Progress >= Threshold)
                    Enable = true;
            }
        }

        void IEffectBase.OnAssert()
        {
            // 使冲突效果失效
            var effect1 = GameLogic.Instance.Effects.GetComponentsInChildren<EffectReverseRaffle>();
            foreach (var e in effect1)
            {
                if (((IEffectBase)e).Target == ((IEffectBase)this).Target && e.UUID != this.UUID)
                {
                    ((IEffectBase)e).OnLapsed();
                }
            }
            var effect2 = GameLogic.Instance.Effects.GetComponentsInChildren<EffectReverse>();
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
                DataSystem.Instance.SetData("Player1Reverse", 1);
            }
            else
            {
                DataSystem.Instance.SetData("Player2Reverse", 1);
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
