using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

namespace RollToFinal
{
    /// <summary>
    /// 数据系统（单例）
    /// </summary>
    public class DataSystem : MonoBehaviour
    {
        /// <summary>
        /// 获取单例
        /// </summary>
        public static DataSystem Instance { get; private set; }

        /// <summary>
        /// 输入管理
        /// </summary>
        public InputManager Input;

        /// <summary>
        /// 数据
        /// </summary>
        public Dictionary<string, int> Data = new();

        public bool UseJoyStick = false;

        public bool EnableAI = false;

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

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public int GetData(string key)
        {
            if(Data.ContainsKey(key))
                return Data[key];
            else
                return 0;
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void SetData(string key, int value)
        {
            Data[key] = value;
        }
    }
}

