using UnityEngine;

namespace RollToFinal
{
    public class EffectOddsLevelReduce : MonoBehaviour, IEffectBase
    {
        public string Name { get => "概率降级"; }
        public string Description { get => $"玩家{((IEffectBase)this).Target}的概率降级"; }

        IEffectBase.EffectType IEffectBase.Type { get => IEffectBase.EffectType.Reduce; }

        int IEffectBase.Target { get; set; }

        public GameObject EffectMove;

        void IEffectBase.OnInstantiated(object[] data)
        {
            int rollResult = (int)data[0];
            // 确认目标
            ((IEffectBase)this).Target = rollResult > 3 ? (GameLogic.Instance.CurrentPlayer == 1 ? 2 : 1) : (GameLogic.Instance.CurrentPlayer == 1 ? 1 : 2);
        }

        void IEffectBase.OnAssert()
        {
            // 产生效果
            if (((IEffectBase)this).Target == 1)
            {
                if (GameLogic.Instance.Player1OddsLevel <= 0)
                {
                    var obj = Instantiate(EffectMove, GameLogic.Instance.Effects.transform.position, Quaternion.identity, GameLogic.Instance.transform);
                    obj.GetComponent<IEffectBase>().OnInstantiated(new object[] { 5, 2 });
                    obj.GetComponent<IEffectBase>().OnAssert();
                }
                else
                    GameLogic.Instance.Player1OddsLevel--;
            }
            else
            {
                if (GameLogic.Instance.Player2OddsLevel <= 0)
                {
                    var obj = Instantiate(EffectMove, GameLogic.Instance.Effects.transform.position, Quaternion.identity, GameLogic.Instance.transform);
                    obj.GetComponent<IEffectBase>().OnInstantiated(new object[] { 5, 1 });
                    obj.GetComponent<IEffectBase>().OnAssert();
                }
                else
                    GameLogic.Instance.Player2OddsLevel--;
            }
            GameLogic.Instance.CalcOdds();
            // 使自身失效
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

