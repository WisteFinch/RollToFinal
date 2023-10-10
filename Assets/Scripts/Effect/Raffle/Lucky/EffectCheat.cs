using UnityEngine;

namespace RollToFinal
{
    public class EffectCheatCode : MonoBehaviour, IEffectBase
    {
        public string Name { get => "作弊码"; }
        public string Description { get => "使下一次判定必定通过"; }

        IEffectBase.EffectType IEffectBase.Type { get => IEffectBase.EffectType.Lucky; }

        int IEffectBase.Target { get; set; }

        void IEffectBase.OnInstantiated(object[] data)
        {
            // 确认目标
            ((IEffectBase)this).Target = GameLogic.Instance.CurrentPlayer == 1 ? 1 : 2;
        }

        void IEffectBase.OnAssert()
        {
            // 产生效果
            int count = DataSystem.Instance.GetData(((IEffectBase)this).Target == 1 ? "Player1JudgeProtection" : "Player2JudgeProtection");
            DataSystem.Instance.SetData(((IEffectBase)this).Target == 1 ? "Player1JudgeProtection" : "Player2JudgeProtection", count + 1);
            ((IEffectBase)this).OnLapsed();
        }

        void IEffectBase.Register()
        {
            return;
        }

        void IEffectBase.OnLapsed()
        {
            Destroy(this.gameObject);
        }
    }
}

