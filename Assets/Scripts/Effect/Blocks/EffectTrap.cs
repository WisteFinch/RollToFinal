using UnityEngine;

namespace RollToFinal
{
    public class EffectTrap : MonoBehaviour, IEffectBase
    {
        public string Name { get => "陷阱"; }
        public string Description { get => ""; }

        IEffectBase.EffectType IEffectBase.Type { get => IEffectBase.EffectType.Calamity; }

        int IEffectBase.Target { get; set; }

        public GameObject Trap;

        public GameObject NormalPerfab;

        public int Threshold = 3;

        public int LifeCycle = 0;

        public int JudgeResult = 0;

        public bool EnableCheck = false;

        void IEffectBase.OnInstantiated(object[] data)
        {
            // 确认目标
            ((IEffectBase)this).Target = GameLogic.Instance.CurrentPlayer == 1 ? 1 : 2;
            // 设定阈值
            Trap = GameLogic.Instance.PlatformBlocks[((IEffectBase)this).Target == 1 ? GameLogic.Instance.Player1Progress : GameLogic.Instance.Player2Progress];
            var obj = Trap.GetComponent<Block>();
            if (obj.Data.Count != 0)
                Threshold = (int)obj.Data[0];
        }

        void IEffectBase.OnAssert()
        {
            GameLogic.Instance.ReplaceBlock(Trap, NormalPerfab);
            GameLogic.Instance.StateBlock++;
            GameLogic.Instance.PlayerJudge(((IEffectBase)this).Target);
            EnableCheck = true;
        }

        void CheckJudgeResult()
        {
            if (!EnableCheck)
                return;
            // 判断是否产生判定结果
            JudgeResult = (int)DataSystem.Instance.GetData("JudgeResult");
            DataSystem.Instance.SetData("JudgeResult", -1);
            if (JudgeResult == -1)
                return;
            // 应用效果
            GameLogic.Instance.CSCallBack -= CheckJudgeResult;
            if (JudgeResult > Threshold)
            {
                if(((IEffectBase)this).Target == 1)
                    GameLogic.Instance.Player1OddsLevelDelta++;
                else
                    GameLogic.Instance.Player2OddsLevelDelta++;
            }
            else
            {
                if(((IEffectBase)this).Target == 1)
                    GameLogic.Instance.Player1OddsLevelDelta--;
                else
                    GameLogic.Instance.Player2OddsLevelDelta--;
            }
            GameLogic.Instance.CalcOdds();
            GameLogic.Instance.StateBlock--;
        }

        private void OnLifeCycleCallBack()
        {
            LifeCycle++;
            if (LifeCycle >= 4)
            {
                ((IEffectBase)this).OnLapsed();
            }
        }

        void IEffectBase.Register(IEffectBase.TurnStartCallBack start, IEffectBase.TurnEndCallBack end, IEffectBase.LifeCycleCallBack lc)
        {
            GameLogic.Instance.CSCallBack += CheckJudgeResult;
            lc += OnLifeCycleCallBack;
        }

        void IEffectBase.OnLapsed()
        {
            // 使效果失效
            if (JudgeResult > Threshold)
            {
                if (((IEffectBase)this).Target == 1)
                    GameLogic.Instance.Player1OddsLevelDelta--;
                else
                    GameLogic.Instance.Player2OddsLevelDelta--;
            }
            else
            {
                if (((IEffectBase)this).Target == 1)
                    GameLogic.Instance.Player1OddsLevelDelta++;
                else
                    GameLogic.Instance.Player2OddsLevelDelta++;
            }
            GameLogic.Instance.CalcOdds();
            Destroy(this.gameObject);
        }
    }
}
