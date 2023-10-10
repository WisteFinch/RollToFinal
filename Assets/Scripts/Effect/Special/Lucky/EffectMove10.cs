using UnityEngine;

namespace RollToFinal
{

    public class EffectMove10 : MonoBehaviour, IEffectBase
    {
        public string Name { get => "移动十步"; }
        public string Description { get => $"玩家{((IEffectBase)this).Target}获得10点移动力"; }

        IEffectBase.EffectType IEffectBase.Type { get => IEffectBase.EffectType.Lucky; }

        int IEffectBase.Target { get; set; }

        public GameObject Move;

        void IEffectBase.OnInstantiated(object[] data)
        {
            int rollResult = (int)data[0];
            // 确定目标
            ((IEffectBase)this).Target = rollResult > 3 ? (GameLogic.Instance.CurrentPlayer == 1 ? 1 : 2) : (GameLogic.Instance.CurrentPlayer == 1 ? 2 : 1);
        }

        void IEffectBase.OnAssert()
        {
            var obj = Instantiate(Move, GameLogic.Instance.Effects.transform.position, Quaternion.identity, GameLogic.Instance.Effects.transform);
            obj.GetComponent<IEffectBase>().OnInstantiated(new object[] { 10, ((IEffectBase)this).Target == 1 ? 1 : 2 });
            obj.GetComponent<IEffectBase>().OnAssert();
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