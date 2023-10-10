using System.Collections.Generic;
using UnityEngine;

namespace RollToFinal
{
    public class EffectRaffle : MonoBehaviour, IEffectBase
    {
        public string Name { get => _name; }
        public string Description { get => _desc; }

        IEffectBase.EffectType IEffectBase.Type { get => IEffectBase.EffectType.Calamity; }

        int IEffectBase.Target { get; set; }

        public GameObject EffectObj;

        public GameObject BlockObj;

        public GameObject NormalPerfab;

        public int Threshold = 3;

        public int JudgeResult = 0;

        public bool EnableCheck = false;

        public int EffectType;

        private string _name;

        private string _desc;

        void IEffectBase.OnInstantiated(object[] data)
        {
            // 确认目标
            ((IEffectBase)this).Target = GameLogic.Instance.CurrentPlayer == 1 ? 1 : 2;
            // 获取方块对象
            BlockObj = GameLogic.Instance.PlatformBlocks[((IEffectBase)this).Target == 1 ? GameLogic.Instance.Player1Progress : GameLogic.Instance.Player2Progress];
            // 获取效果对象并初始化
            bool flag;
            int index;
            IEffectBase effect;
            do
            {
                flag = false;
                EffectType = Random.Range(0, GameLogic.Instance.RaffleOptionsList.Count);
                index = Random.Range(0, GameLogic.Instance.RaffleOptionsList[EffectType].Effects.Count);
                var EffectPerfab = GameLogic.Instance.RaffleOptionsList[EffectType].Effects[index];
                EffectObj = Instantiate(EffectPerfab, GameLogic.Instance.Effects.transform.position, Quaternion.identity, GameLogic.Instance.Effects.transform);
                effect = EffectObj.GetComponent<IEffectBase>();
                effect.OnInstantiated();

                EffectReverseRaffle e1;
                if (EffectObj.TryGetComponent(out e1))
                {
                    if (!e1.Enable)
                    {
                        flag = true;
                        Destroy(EffectObj);
                    }
                }
            } while (flag);
            
            // 设置标题
            switch (EffectType)
            {
                case 0:
                    _name = $"祝福 : {effect.Name}";
                    _desc = $"{effect.Description} \n判定值大于 {Threshold} 即可获得";
                    break;
                case 1:
                    _name = $"诅咒 : {effect.Name}";
                    _desc = $"{effect.Description} \n判定值大于 {Threshold} 即可避免";
                    break;
                case 2:
                    _name = $"抉择 : {effect.Name}";
                    _desc = $"{effect.Description}";
                    break;
            }
        }

        void IEffectBase.OnAssert()
        {
            GameLogic.Instance.StateBlock++;
            GameLogic.Instance.PlayerJudge(((IEffectBase)this).Target);
            EnableCheck = true;
        }

        public void CheckJudgeResult()
        {
            if (!EnableCheck)
                return;
            // 判断是否产生判定结果
            JudgeResult = GameLogic.Instance.JudgeResult + (((IEffectBase)this).Target == 1 ? DataSystem.Instance.GetData("Player1JudgeBonus") : DataSystem.Instance.GetData("Player2JudgeBonus"));
            GameLogic.Instance.JudgeResult = -1;
            EnableCheck = false;
            if (JudgeResult == -1)
                return;
        }

        public void Execute()
        {
            // 应用效果
            GameLogic.Instance.ReplaceBlock(BlockObj, NormalPerfab);
            var effect = EffectObj.GetComponent<IEffectBase>();
            if (JudgeResult > 3)
            {
                if (EffectType == 0)
                {
                    effect.Register();
                    effect.OnAssert();
                }
                else if (EffectType == 1)
                    Destroy(EffectObj);
            }
            else
            {
                if (EffectType == 0)
                    Destroy(EffectObj);
                else if (EffectType == 1)
                {
                    effect.Register();
                    effect.OnAssert();
                }
            }
            GameLogic.Instance.CSCallBack -= CheckJudgeResult;
            GameLogic.Instance.StateBlock--;
            ((IEffectBase)this).OnLapsed();
        }

        void IEffectBase.Register()
        {
        }

        void IEffectBase.OnLapsed()
        {
            Destroy(this.gameObject);
        }
    }
}
