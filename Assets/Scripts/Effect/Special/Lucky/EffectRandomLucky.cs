using System.Linq;
using UnityEngine;

namespace RollToFinal
{
    public class EffectRandomLucky : MonoBehaviour, IEffectBase
    {
        public string Name { get => "随机获得一个增益效果"; }
        public string Description { get => Desc; }

        IEffectBase.EffectType IEffectBase.Type { get => IEffectBase.EffectType.Lucky; }

        int IEffectBase.Target { get; set; }

        public GameObject[] Luckies;

        public string Desc;

        public int index;

        void IEffectBase.OnInstantiated(object[] data)
        {
            // 确认目标
            ((IEffectBase)this).Target = GameLogic.Instance.CurrentPlayer == 1 ? 1 : 2;
            // 获取效果编号
            index = Random.Range(0, Luckies.Count());
            // 设置介绍
            Desc = $"{Luckies[index].GetComponent<IEffectBase>().Name}";
        }

        void IEffectBase.OnAssert()
        {
            // 产生效果
            var perfab = Luckies[index];
            var obj = Instantiate(perfab, GameLogic.Instance.Effects.transform.position, Quaternion.identity, GameLogic.Instance.Effects.transform);
            var Effect = obj.GetComponent<IEffectBase>();
            Effect.Register();
            Effect.OnInstantiated(new object[] { 7 });
            Effect.OnAssert();
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
