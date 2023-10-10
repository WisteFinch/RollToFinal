using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RollToFinal
{
    public class Transfer : MonoBehaviour
    {

        /// <summary>
        /// 获取单例
        /// </summary>
        public static Transfer Instance { get; private set; }

        public bool EnableAI = false;

        public bool UseJoyStick = false;

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
        }
    }

}
