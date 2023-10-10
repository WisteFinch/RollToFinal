using System.Linq;
using UnityEngine;

namespace RollToFinal
{
    public class EffectRandom2Calamities : MonoBehaviour, IEffectBase
    {
        public string Name { get => "随机获得两个损耗效果"; }
        public string Description { get => Desc; }

        IEffectBase.EffectType IEffectBase.Type { get => IEffectBase.EffectType.Calamity; }

        int IEffectBase.Target { get; set; }

        public GameObject[] Calamities;

        public string Desc;

        public int[] index = new int[2];

        void IEffectBase.OnInstantiated(object[] data)
        {
            // 确认目标
            ((IEffectBase)this).Target = GameLogic.Instance.CurrentPlayer == 1 ? 1 : 2;
            // 获取效果编号
            index[0] = Random.Range(0, Calamities.Count());
            do
            {
                index[1] = Random.Range(0, Calamities.Count());
            } while (index[0] == index[1]);
            // 设置介绍
            Desc = $"{Calamities[index[0]].GetComponent<IEffectBase>().Name}\n{Calamities[index[1]].GetComponent<IEffectBase>().Name}";
        }

        void IEffectBase.OnAssert()
        {
            // 产生效果
            var perfab = Calamities[index[0]];
            var obj = Instantiate(perfab, GameLogic.Instance.Effects.transform.position, Quaternion.identity, GameLogic.Instance.Effects.transform);
            var Effect = obj.GetComponent<IEffectBase>();
            Effect.Register();
            Effect.OnInstantiated(new object[] { 0 });
            Effect.OnAssert();
            perfab = Calamities[index[1]];
            obj = Instantiate(perfab, GameLogic.Instance.Effects.transform.position, Quaternion.identity, GameLogic.Instance.Effects.transform);
            Effect = obj.GetComponent<IEffectBase>();
            Effect.Register();
            Effect.OnInstantiated(new object[] { 0 });
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
