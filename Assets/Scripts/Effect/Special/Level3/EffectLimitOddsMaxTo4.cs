using UnityEngine;

namespace RollToFinal
{
    public class EffectLimitOddsMaxTo4 : MonoBehaviour, IEffectBase
    {
        public string Name { get => "降低骰子上限至4"; }
        public string Description { get => ""; }

        public int LifeCycle = 0;

        public int TargetPlayer = 0;

        public string UUID;

        void IEffectBase.OnInstantiated(GameObject player, object[] data)
        {
            UUID = System.Guid.NewGuid().ToString();
            int rollResult = (int)data[0];
            // 确认目标
            TargetPlayer = rollResult > 3 ? (GameLogic.Instance.CurrentPlayer == 1 ? 2 : 1) : (GameLogic.Instance.CurrentPlayer == 1 ? 1 : 2);
            // 使冲突效果失效
            var effects1 = GameLogic.Instance.Effects.GetComponentsInChildren<EffectLimitOddsMaxTo4>();
            foreach(var e in effects1)
            {
                if(e.TargetPlayer == TargetPlayer && e.UUID != this.UUID)
                {
                    ((IEffectBase)e).OnLapsed();
                }
            }
            var effects2 = GameLogic.Instance.Effects.GetComponentsInChildren<EffectRiseOddsLevelTo6>();
            foreach (var e in effects2)
            {
                if (e.TargetPlayer == TargetPlayer)
                {
                    ((IEffectBase)e).OnLapsed();
                }
            }
            // 产生效果
            if (TargetPlayer == 1)
            {
                var delta = GameLogic.Instance.Player1OddsDelta;
                for (int i = 4; i < delta.Count; i++)
                    delta[i] -= 1000f;
                GameLogic.Instance.Player1OddsDelta = delta;
            }
            else
            {
                var delta = GameLogic.Instance.Player2OddsDelta;
                for (int i = 4; i < delta.Count; i++)
                    delta[i] -= 1000f;
                GameLogic.Instance.Player2OddsDelta = delta;
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
            if (LifeCycle >= 4)
            {
                ((IEffectBase)this).OnLapsed();
            }
        }

        void IEffectBase.OnLapsed()
        {
            if (TargetPlayer == 1)
            {
                var delta = GameLogic.Instance.Player1OddsDelta;
                for (int i = 4; i < delta.Count; i++)
                    delta[i] += 1000f;
                GameLogic.Instance.Player1OddsDelta = delta;
            }
            else
            {
                var delta = GameLogic.Instance.Player2OddsDelta;
                for (int i = 4; i < delta.Count; i++)
                    delta[i] += 1000f;
                GameLogic.Instance.Player2OddsDelta = delta;
            }
            GameLogic.Instance.CalcOdds();
            Destroy(this.gameObject);
        }
    }
}

