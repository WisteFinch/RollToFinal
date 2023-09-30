using UnityEngine;

namespace RollToFinal
{
    public class EffectOddsLevelReduce : MonoBehaviour, IEffectBase
    {
        public string Name { get => "骰子降级"; }
        public string Description { get => ""; }

        public int LifeCycle = 0;

        public int TargetPlayer = 0;

        public GameObject EffectMove;

        void IEffectBase.OnInstantiated(GameObject player, object[] data)
        {
            int rollResult = (int)data[0];
            // 确认目标
            TargetPlayer = rollResult > 3 ? (GameLogic.Instance.CurrentPlayer == 1 ? 2 : 1) : (GameLogic.Instance.CurrentPlayer == 1 ? 1 : 2);
            // 产生效果
            if (TargetPlayer == 1)
            {
                if(GameLogic.Instance.Player1OddsLevel <= 0)
                {
                    var obj = Instantiate(EffectMove, GameLogic.Instance.Effects.transform.position, Quaternion.identity, GameLogic.Instance.transform);
                    obj.GetComponent<IEffectBase>().OnInstantiated(GameLogic.Instance.Player2, new object[] { 5, 2 });
                }
                else
                    GameLogic.Instance.Player1OddsLevel--;
            }
            else
            {
                if (GameLogic.Instance.Player2OddsLevel <= 0)
                {
                    var obj = Instantiate(EffectMove, GameLogic.Instance.Effects.transform.position, Quaternion.identity, GameLogic.Instance.transform);
                    obj.GetComponent<IEffectBase>().OnInstantiated(GameLogic.Instance.Player1, new object[] { 5, 1 });
                }
                else
                    GameLogic.Instance.Player2OddsLevel--;
            }
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

