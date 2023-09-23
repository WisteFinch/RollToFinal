using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RollToFinal
{
    /// <summary>
    /// 游戏核心（单例）
    /// </summary>
    public class GameCore : MonoBehaviour
    {
        /// <summary>
        /// 获取单例
        /// </summary>
        public static GameCore Instance { get; private set; }

        /// <summary>
        /// 输入管理
        /// </summary>
        public InputManager Input;

        private void Awake()
        {
            // 创建单例
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
                return;
            }

            // 设置输入管理
            this.Input = this.gameObject.GetComponent<InputManager>();

        }
    }
}

