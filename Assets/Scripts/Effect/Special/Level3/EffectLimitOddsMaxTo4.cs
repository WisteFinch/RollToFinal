using UnityEngine;

namespace RollToFinal
{
    public class EffectLimitOddsMaxTo4 : MonoBehaviour, IEffectBase
    {
        public string Name { get => "降低点数上限至4"; }
        public string Description { get => $"玩家{((IEffectBase)this).Target}的点数上限降至4，持续2回合"; }

        IEffectBase.EffectType IEffectBase.Type { get => IEffectBase.EffectType.Reduce; }

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
            var effects1 = GameLogic.Instance.Effects.GetComponentsInChildren<EffectLimitOddsMaxTo4>();
            foreach (var e in effects1)
            {
                if (((IEffectBase)e).Target == ((IEffectBase)this).Target && e.UUID != this.UUID)
                {
                    ((IEffectBase)e).OnLapsed();
                }
            }
            var effects2 = GameLogic.Instance.Effects.GetComponentsInChildren<EffectRiseOddsLevelTo6>();
            foreach (var e in effects2)
            {
                if (((IEffectBase)e).Target == ((IEffectBase)this).Target)
                {
                    ((IEffectBase)e).OnLapsed();
                }
            }
            // 产生效果
            if (((IEffectBase)this).Target == 1)
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

