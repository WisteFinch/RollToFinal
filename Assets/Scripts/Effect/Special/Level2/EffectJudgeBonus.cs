using UnityEngine;

namespace RollToFinal
{
    public class EffectJudgeBonus : MonoBehaviour, IEffectBase
    {
        public string Name { get => "判定奖励"; }
        public string Description { get => $"玩家{((IEffectBase)this).Target}的判定点数+1，持续2回合"; }

        IEffectBase.EffectType IEffectBase.Type { get => IEffectBase.EffectType.Gain; }

        int IEffectBase.Target { get; set; }

        public int LifeCycle = 0;

        public string UUID;



        void IEffectBase.OnInstantiated(object[] data)
        {
            UUID = System.Guid.NewGuid().ToString();
            int rollResult = (int)data[0];
            // 确定目标
            ((IEffectBase)this).Target = rollResult > 3 ? (GameLogic.Instance.CurrentPlayer == 1 ? 1 : 2) : (GameLogic.Instance.CurrentPlayer == 1 ? 2 : 1);
        }

        void IEffectBase.OnAssert()
        {
            // 使冲突效果失效
            var effect1 = GameLogic.Instance.Effects.GetComponentsInChildren<EffectJudgeBonus>();
            foreach (var e in effect1)
            {
                if (((IEffectBase)e).Target == ((IEffectBase)this).Target && e.UUID != this.UUID)
                {
                    ((IEffectBase)e).OnLapsed();
                }
            }
            // 产生效果
            if (((IEffectBase)this).Target == 1)
            {
                DataSystem.Instance.SetData("Player1JudgeBonus", 1);
            }
            else
            {
                DataSystem.Instance.SetData("Player2JudgeBonus", 1);
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
            if(LifeCycle >= 4)
            {
                GameLogic.Instance.LifeCycleCallBack -= OnLifeCycleCallBack;
                ((IEffectBase)this).OnLapsed();
            }
                
        }

        void IEffectBase.OnLapsed()
        {
            if (((IEffectBase)this).Target == 1)
            {
                DataSystem.Instance.SetData("Player1JudgeBonus", 0);
            }
            else
            {
                DataSystem.Instance.SetData("Player2JudgeBonus", 0);
            }
            Destroy(this.gameObject);
        }
    }
}
