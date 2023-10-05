using UnityEngine;

namespace RollToFinal
{
    public class EffectOddsLevelRise : MonoBehaviour, IEffectBase
    {
        public string Name { get => "骰子升级"; }
        public string Description { get => ""; }

        IEffectBase.EffectType IEffectBase.Type { get => IEffectBase.EffectType.Lucky; }

        int IEffectBase.Target { get; set; }

        public int LifeCycle = 0;

        public GameObject EffectMove;

        void IEffectBase.OnInstantiated(object[] data)
        {
            int rollResult = (int)data[0];
            // 确认目标
            ((IEffectBase)this).Target = rollResult > 3 ? (GameLogic.Instance.CurrentPlayer == 1 ? 1 : 2) : (GameLogic.Instance.CurrentPlayer == 1 ? 2 : 1);
            
        }

        void IEffectBase.OnAssert()
        {
            // 产生效果
            if (((IEffectBase)this).Target == 1)
            {
                if (GameLogic.Instance.Player1OddsLevel >= 4)
                {
                    var obj = Instantiate(EffectMove, GameLogic.Instance.Effects.transform.position, Quaternion.identity, GameLogic.Instance.transform);
                    obj.GetComponent<IEffectBase>().OnInstantiated(new object[] { 5, 1 });
                }
                else
                    GameLogic.Instance.Player1OddsLevel++;
            }
            else
            {
                if (GameLogic.Instance.Player2OddsLevel >= 4)
                {
                    var obj = Instantiate(EffectMove, GameLogic.Instance.Effects.transform.position, Quaternion.identity, GameLogic.Instance.transform);
                    obj.GetComponent<IEffectBase>().OnInstantiated(new object[] { 5, 2 });
                }
                else
                    GameLogic.Instance.Player2OddsLevel++;
            }
            GameLogic.Instance.CalcOdds();
            // 使自身失效
            ((IEffectBase)this).OnLapsed();
        }

        void IEffectBase.Register(IEffectBase.TurnStartCallBack start, IEffectBase.TurnEndCallBack end, IEffectBase.LifeCycleCallBack lc)
        {
            return;
        }

        void IEffectBase.OnLapsed()
        {
            Destroy(this.gameObject);
        }
    }
}

