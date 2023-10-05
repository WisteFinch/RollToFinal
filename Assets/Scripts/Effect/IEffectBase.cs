using System;
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
        /// 效果类型
        /// </summary>
        [Serializable]
        public enum EffectType
        {
            /// <summary>
            /// 幸运
            /// </summary>
            Lucky,
            /// <summary>
            /// 厄运
            /// </summary>
            Calamity
        }

        /// <summary>
        /// 目标效果
        /// </summary>
        public int Target { get; set; }

        /// <summary>
        /// 注册委托
        /// </summary>
        public void Register(TurnStartCallBack start, TurnEndCallBack end, LifeCycleCallBack lc);

        /// <summary>
        /// 实例化时调用
        /// </summary>
        /// <param name="data">数据</param>
        public void OnInstantiated(object[] data = null);

        /// <summary>
        /// 生效时调用
        /// </summary>
        public void OnAssert();

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

        /// <summary>
        /// 效果类型
        /// </summary>
        public EffectType Type { get; }
    }
}