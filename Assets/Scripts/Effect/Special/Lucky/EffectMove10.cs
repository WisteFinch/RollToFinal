using UnityEngine;

namespace RollToFinal
{

    public class EffectMove10 : MonoBehaviour, IEffectBase
    {
        public string Name { get => "ÒÆ¶¯Ê®²½"; }
        public string Description { get => ""; }

        public int TargetPlayer = 0;

        public GameObject Move;

        void IEffectBase.OnInstantiated(GameObject player, object[] data)
        {
            int rollResult = (int)data[0];

            TargetPlayer = rollResult > 3 ? (GameLogic.Instance.CurrentPlayer == 1 ? 2 : 1) : (GameLogic.Instance.CurrentPlayer == 1 ? 1 : 2);
            var obj = Instantiate(Move, GameLogic.Instance.Effects.transform.position, Quaternion.identity, GameLogic.Instance.Effects.transform);
            obj.GetComponent<IEffectBase>().OnInstantiated(TargetPlayer == 1 ? GameLogic.Instance.Player1 : GameLogic.Instance.Player2 , new object[] { 10, TargetPlayer == 1 ? 1 : 2 });
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