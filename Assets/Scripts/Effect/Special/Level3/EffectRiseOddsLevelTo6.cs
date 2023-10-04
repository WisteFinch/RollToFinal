using UnityEngine;

namespace RollToFinal
{
    public class EffectRiseOddsLevelTo6 : MonoBehaviour, IEffectBase
    {
        public string Name { get => "全6点数"; }
        public string Description { get => ""; }

        public int LifeCycle = 0;

        public int TargetPlayer = 0;

        public string UUID = System.Guid.NewGuid().ToString();

        void IEffectBase.OnInstantiated(GameObject player, object[] data)
        {
            UUID = System.Guid.NewGuid().ToString();
            int rollResult = (int)data[0];
            // 确认目标
            TargetPlayer = rollResult > 3 ? (GameLogic.Instance.CurrentPlayer == 1 ? 1 : 2) : (GameLogic.Instance.CurrentPlayer == 1 ? 2 : 1);
            // 使冲突效果失效
            var effect1 = GameLogic.Instance.Effects.GetComponentsInChildren<EffectRiseOddsLevelTo6>();
            foreach(var e in effect1)
            {
                if(e.TargetPlayer == TargetPlayer && e.UUID != this.UUID)
                {
                    ((IEffectBase)e).OnLapsed();
                }
            }
            var effect2 = GameLogic.Instance.Effects.GetComponentsInChildren<EffectLimitOddsMaxTo4>();
            foreach (var e in effect2)
            {
                if (e.TargetPlayer == TargetPlayer)
                {
                    ((IEffectBase)e).OnLapsed();
                }
            }
            // 产生效果
            if (TargetPlayer == 1)
            {
                GameLogic.Instance.Player1OddsLevelDelta += 10;
            }
            else
            {
                GameLogic.Instance.Player2OddsLevelDelta += 10;
            }
            GameLogic.Instance.CalcOdds();
        }

        void IEffectBase.Register(IEffectBase.TurnStartCallBack start, IEffectBase.TurnEndCallBack end, IEffectBase.LifeCycleCallBack lc)
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
            if (TargetPlayer == 1)
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

