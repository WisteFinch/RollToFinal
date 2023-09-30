using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RollToFinal
{
    interface IEffectBase
    {
        /// <summary>
        /// 回合开始委托
        /// </summary>
        public delegate void TurnStartCallBack();

        /// <summary>
        /// 回合结束委托
        /// </summary>
        public delegate void TurnEndCallBack();

        /// <summary>
        /// 生命周期检测委托
        /// </summary>
        public delegate void LifeCycleCallBack();
        /// <summary>
        /// 注册委托
        /// </summary>
        public void Register(TurnStartCallBack start, TurnEndCallBack end, LifeCycleCallBack lc);

        /// <summary>
        /// 实例化时调用
        /// </summary>
        /// <param name="player">玩家实例</param>
        /// <param name="data">数据</param>
        public void OnInstantiated(GameObject player, object[] data = null);

        /// <summary>
        /// 失效时调用
        /// </summary>
        public void OnLapsed();

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 介绍
        /// </summary>
        public string Description { get; }
    }
}