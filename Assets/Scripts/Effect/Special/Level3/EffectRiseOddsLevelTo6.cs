using UnityEngine;

namespace RollToFinal
{
    public class EffectRiseOddsLevelTo6 : MonoBehaviour, IEffectBase
    {
        public string Name { get => "全6点数"; }
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
            var effect1 = GameLogic.Instance.Effects.GetComponentsInChildren<EffectRiseOddsLevelTo6>();
            foreach (var e in effect1)
            {
                if (((IEffectBase)e).Target == ((IEffectBase)this).Target && e.UUID != this.UUID)
                {
                    ((IEffectBase)e).OnLapsed();
                }
            }
            var effect2 = GameLogic.Instance.Effects.GetComponentsInChildren<EffectLimitOddsMaxTo4>();
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
                GameLogic.Instance.Player1OddsLevelDelta += 10;
            }
            else
            {
                GameLogic.Instance.Player2OddsLevelDelta += 10;
            }
            GameLogic.Instance.CalcOdds();
        }

        void IEffectBase.Register(ref IEffectBase.TurnStartCallBack start, ref IEffectBase.TurnEndCallBack end, ref IEffectBase.LifeCycleCallBack lc)
        {
            lc += OnLifeCycleCallBack;
            return;
        }

        private void OnLifeCycleCallBack()
        {
            LifeCycle++;
            if (LifeCycle >= 2)
            {
                ((IEffectBase)this).OnLapsed();
            }
        }

        void IEffectBase.OnLapsed()
        {
            if (((IEffectBase)this).Target == 1)
            {
                GameLogic.Instance.Player1OddsLevelDelta -= 10;
            }
            else
            {
                GameLogic.Instance.Player2OddsLevelDelta -= 10;
            }
            GameLogic.Instance.CalcOdds();
            Destroy(this.gameObject);
        }
    }
}

