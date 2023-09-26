using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RollToFinal
{
    public class GameLogic : MonoBehaviour
    {
        /// <summary>
        /// 平台生成项
        /// </summary>
        [Serializable]
        public struct PlatformGenerateItem
        {
            public GameObject Block;
            public float Odds;
        }

        /// <summary>
        /// 游戏状态
        /// </summary>
        public enum GameState
        {
            /// <summary>
            /// 游戏开始
            /// </summary>
            Start,
            /// <summary>
            /// 开场
            /// </summary>
            Opening,
            /// <summary>
            /// 玩家闲置
            /// </summary>
            PlayerIdle,
            /// <summary>
            /// 掷骰子
            /// </summary>
            Rolling,
            /// <summary>
            /// 移动
            /// </summary>
            Moving,
            /// <summary>
            /// 事件
            /// </summary>
            Event,
            /// <summary>
            /// 事件效果
            /// </summary>
            EventEffect,
            /// <summary>
            /// 切换玩家
            /// </summary>
            SwitchPlayer,
        }

        [Header("游戏参数")]
        public int Length = 200;

        [Header("对象引用")]

        /// <summary>
        /// 玩家1平台
        /// </summary>
        public GameObject Platform1;

        /// <summary>
        /// 玩家2平台
        /// </summary>
        public GameObject Platform2;

        /// <summary>
        /// 起始方块
        /// </summary>
        public GameObject BeginBlock;

        /// <summary>
        /// 终点方块
        /// </summary>
        public GameObject EndBlock;

        /// <summary>
        /// 玩家1摄像头
        /// </summary>
        public GameObject Player1Camera;

        /// <summary>
        /// 玩家2摄像头
        /// </summary>
        public GameObject Player2Camera;

        /// <summary>
        /// 全局视角摄像头
        /// </summary>
        public GameObject OverallCamera;

        /// <summary>
        /// 玩家中部视角摄像头
        /// </summary>
        public GameObject PlayerMiddleViewCamera;


        /// <summary>
        /// 平台生成表
        /// </summary>
        public List<PlatformGenerateItem> PlatformGenerateTable;

        
        [Header("时间轴")]
        /// <summary>
        /// 开场时间轴
        /// </summary>
        public PlayableDirector OpeningDirector;

        [Header("运行数据")]

        /// <summary>
        /// 当前游戏状态
        /// </summary>
        public GameState CurrentGameState = GameState.Start;

        /// <summary>
        /// 当前玩家
        /// </summary>
        public int CurrentPlayer = 1;

        /// <summary>
        /// 玩家1平台方块
        /// </summary>
        [HideInInspector]
        public List<GameObject> PlatformBlocks1 = new();

        /// <summary>
        /// 玩家2平台方块
        /// </summary>
        [HideInInspector]
        public List<GameObject> PlatformBlocks2 = new();

        private float SumOdds = 0;


        /// <summary>
        /// 生成平台
        /// </summary>
        public void GeneratePlatform()
        {
            // 销毁原有平台
            PlatformBlocks1.Clear();
            foreach (var item in PlatformBlocks1) {
                Destroy(item);
            }
            PlatformBlocks2.Clear();
            foreach (var item in PlatformBlocks2)
            {
                Destroy(item);
            }
            // 计算概率和
            SumOdds = 0;
            foreach (var item in PlatformGenerateTable) {
                SumOdds += item.Odds;
            }
            // 创建起点
            AddBlock(BeginBlock);
            // 创建平台
            for(int i = 1; i <= Length; i++)
            {
                AddBlock(GenerateBlock());
            }
            // 创建终点
            AddBlock(EndBlock);
        }

        /// <summary>
        /// 根据概率从生成表中选取预制体
        /// </summary>
        /// <returns>选取的预制体</returns>
        private GameObject GenerateBlock()
        {
            float rand = UnityEngine.Random.Range(0f, SumOdds);
            foreach(var item in PlatformGenerateTable)
            {
                rand -= item.Odds;
                if (rand <= 0f)
                {
                    return item.Block;
                }
            }
            return PlatformGenerateTable[0].Block;
        }

        /// <summary>
        /// 平台添加方块
        /// </summary>
        /// <param name="perfab">方块预制体</param>
        /// <param name="target">目标平台 0为全部</param>
        private void AddBlock(GameObject perfab, int target = 0)
        {
            if (target == 0 || target == 1)
            {
                GameObject obj1 = Instantiate(perfab, new Vector3(Platform1.transform.position.x, Platform1.transform.position.y, PlatformBlocks1.Count + Platform1.transform.position.z), Quaternion.identity, Platform1.transform);
                PlatformBlocks1.Add(obj1);
            }
            if (target == 0 || target == 2)
            {
                GameObject obj2 = Instantiate(perfab, new Vector3(Platform2.transform.position.x, Platform2.transform.position.y, PlatformBlocks2.Count + Platform2.transform.position.z), Quaternion.identity, Platform2.transform);
                PlatformBlocks2.Add(obj2);
            }
        }

        private void Start()
        {
            GeneratePlatform();
            OnChangeState();
        }

        private void OnDestroy()
        {
            // 销毁原有平台
            PlatformBlocks1.Clear();
            foreach (var item in PlatformBlocks1)
            {
                Destroy(item);
            }
            PlatformBlocks2.Clear();
            foreach (var item in PlatformBlocks2)
            {
                Destroy(item);
            }
        }

        /// <summary>
        /// 时间轴动画结束后调用状态改变函数
        /// </summary>
        /// <param name="director">时间轴</param>
        private void InvokeAfterTimelineFinish(PlayableDirector director)
        {
            Invoke(nameof(OnChangeState), (float)director.duration);
        }

        /// <summary>
        /// 播放时间轴并在结束后调用状态改变函数
        /// </summary>
        /// <param name="director">时间轴</param>
        private void PlayAndInvoke(PlayableDirector director)
        {
            director.Play();
            InvokeAfterTimelineFinish(director);
        }

        public void OnChangeState()
        {
            switch (CurrentGameState)
            {
                case GameState.Start:
                    CurrentGameState = GameState.Opening;
                    PlayAndInvoke(OpeningDirector);
                    break;
                // 开场运镜
                case GameState.Opening:
                    CurrentPlayer = 1;
                    CurrentGameState = GameState.PlayerIdle;
                break;
            }
        }
    }
}