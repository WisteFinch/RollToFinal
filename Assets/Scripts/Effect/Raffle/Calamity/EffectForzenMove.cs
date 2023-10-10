using UnityEngine;

namespace RollToFinal
{
    public class EffectForzenMove : MonoBehaviour, IEffectBase
    {
        public string Name { get => "诅咒II"; }
        public string Description { get => "下回合无法移动"; }

        public int LifeCycle = 0;

        IEffectBase.EffectType IEffectBase.Type { get => IEffectBase.EffectType.Calamity; }

        int IEffectBase.Target { get; set; }



        void IEffectBase.OnInstantiated(object[] data)
        {
            // 确认目标
            ((IEffectBase)this).Target = GameLogic.Instance.CurrentPlayer == 1 ? 1 : 2;
        }

        void IEffectBase.OnAssert()
        {
            // 产生效果
            DataSystem.Instance.SetData(((IEffectBase)this).Target == 1 ? "Player1ForzenMove" : "Player2ForzenMove", 1);
        }

        void IEffectBase.Register()
        {
            GameLogic.Instance.LifeCycleCallBack += OnLifeCycleCallBack;
            return;
        }

        void OnLifeCycleCallBack()
        {
            LifeCycle++;
            if (LifeCycle >= 2)
            {
                GameLogic.Instance.LifeCycleCallBack -= OnLifeCycleCallBack;
                ((IEffectBase)this).OnLapsed();
            }
        }

        void IEffectBase.OnLapsed()
        {
            DataSystem.Instance.SetData(((IEffectBase)this).Target == 1 ? "Player1ForzenMove" : "Player2ForzenMove", 0);
            Destroy(this.gameObject);
        }
    }
}

