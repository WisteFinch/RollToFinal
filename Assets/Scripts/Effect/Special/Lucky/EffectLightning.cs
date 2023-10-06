using UnityEngine;

namespace RollToFinal
{

    public class Effectlightning : MonoBehaviour, IEffectBase
    {
        public string Name { get => "闪电"; }
        public string Description { get => ""; }

        IEffectBase.EffectType IEffectBase.Type { get => IEffectBase.EffectType.Calamity; }

        int IEffectBase.Target { get; set; }

        public GameObject Trap;

        void IEffectBase.OnInstantiated(object[] data)
        {
            int rollResult = (int)data[0];
            // 确定目标
            ((IEffectBase)this).Target = GameLogic.Instance.CurrentPlayer == 1 ? 2 : 1;
        }

        void IEffectBase.OnAssert()
        { 
            // 产生效果
            int progress = ((IEffectBase)this).Target == 1 ? GameLogic.Instance.Player1Progress : GameLogic.Instance.Player2Progress;
            int offset = Random.Range(1, 7);
            int index = Mathf.Clamp(progress + offset, 1, GameLogic.Instance.Length + 1);
            GameLogic.Instance.ReplaceBlock(GameLogic.Instance.PlatformBlocks[index], Trap);
            GameLogic.Instance.PlatformBlocks[index].GetComponent<Block>().Data.Add(6);
            // 使自身失效
            ((IEffectBase)this).OnLapsed();
        }

        void IEffectBase.Register(ref IEffectBase.TurnStartCallBack start, ref IEffectBase.TurnEndCallBack end, ref IEffectBase.LifeCycleCallBack lc)
        {
            return;
        }

        void IEffectBase.OnLapsed()
        {
            Destroy(this.gameObject);
        }
    }
}