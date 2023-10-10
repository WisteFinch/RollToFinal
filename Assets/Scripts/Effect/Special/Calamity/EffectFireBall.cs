using System.Linq;
using UnityEngine;

namespace RollToFinal
{
    public class EffectFireBall : MonoBehaviour, IEffectBase
    {
        public string Name { get => "火球"; }
        public string Description { get => $"在玩家{((IEffectBase)this).Target}面前生成一个火球，破坏3个方块\n三回合后方块将自动修复"; }

        IEffectBase.EffectType IEffectBase.Type { get => IEffectBase.EffectType.Calamity; }

        int IEffectBase.Target { get; set; }

        public GameObject Empty;

        public GameObject Normal;

        public int TargetIndex;

        public int LifeCycle = 0;



        void IEffectBase.OnInstantiated(object[] data)
        {
            // 确认目标
            ((IEffectBase)this).Target = GameLogic.Instance.CurrentPlayer == 1 ? 1 : 2;
        }

        void IEffectBase.OnAssert()
        {
            // 产生效果
            GameLogic.Instance.StateBlock++;
            TargetIndex = (((IEffectBase)this).Target == 1 ? GameLogic.Instance.Player1Progress : GameLogic.Instance.Player2Progress) + Random.Range(1, 7);
            GameLogic.Instance.Particles.getFireball(new(0, 10, TargetIndex + 1));
            Invoke(nameof(ReplaceBlock), 3);
        }

        void ReplaceBlock()
        {
            int count = 3;
            int index = TargetIndex;
            while (count > 0 && index <= GameLogic.Instance.Length)
            {
                GameLogic.Instance.ReplaceBlock(GameLogic.Instance.PlatformBlocks[index], Empty, true);
                count--;
                index++;
            }
            GameLogic.Instance.StateBlock--;
        }

        void IEffectBase.Register()
        {
            GameLogic.Instance.LifeCycleCallBack += OnLifeCycleCallBack;
        }

        void OnLifeCycleCallBack()
        {
            LifeCycle++;
            if (LifeCycle >= 6)
            {
                GameLogic.Instance.LifeCycleCallBack -= OnLifeCycleCallBack;
                ((IEffectBase)this).OnLapsed();
            }
        }

        void IEffectBase.OnLapsed()
        {
            int count = 3;
            int index = TargetIndex;
            while (count > 0 && index <= GameLogic.Instance.Length)
            {
                if (GameLogic.Instance.PlatformBlocks[index].GetComponent<Block>().Type == Block.BlockType.Empty)
                    GameLogic.Instance.ReplaceBlock(GameLogic.Instance.PlatformBlocks[index], Normal);
                count--;
                index++;
            }
            Destroy(this.gameObject);
        }
    }
}
