using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace RollToFinal
{
    public class GameLogic : MonoBehaviour
    {
        #region 单例
        /// <summary>
        /// 获取单例
        /// </summary>
        public static GameLogic Instance { get; private set; }

        private void Awake()
        {
            // 创建单例
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this.gameObject);
                return;
            }
        }
        #endregion

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
        /// 选项
        /// </summary>
        [Serializable]
        public struct OptionItem
        {
            public string Title;
            public string Description;
            public List<GameObject> Effects;
        }

        /// <summary>
        /// 游戏状态
        /// </summary>
        public enum GameState
        {
            /// <summary>
            /// 游戏开始
            /// </summary>
            GameStart,
            /// <summary>
            /// 开场
            /// </summary>
            Opening,
            /// <summary>
            /// 玩家开始
            /// </summary>
            PlayerStart,
            /// <summary>
            /// 委托：回合开始
            /// </summary>
            DelegateStart,
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
            /// 掷特殊骰子
            /// </summary>
            SpecialRolling,
            /// <summary>
            /// 特殊骰子效果
            /// </summary>
            SpecialEffect,
            /// <summary>
            /// 方块效果
            /// </summary>
            BlockEffect,
            /// <summary>
            /// 委托：回合结束
            /// </summary>
            DelegateEnd,
            /// <summary>
            /// 切换玩家
            /// </summary>
            SwitchPlayer,
            /// <summary>
            /// 委托：清除失效效果
            /// </summary>
            DelegateLC,
            /// <summary>
            /// 胜利
            /// </summary>
            Win
        }

        [Header("游戏参数")]
        public int Length = 200;

        public int SpecialRollCoolDown = 2;

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
        /// 玩家1
        /// </summary>
        public GameObject Player1;

        /// <summary>
        /// 玩家2
        /// </summary>
        public GameObject Player2;

        /// <summary>
        /// 玩家1饼图
        /// </summary>
        public PieChartController Player1PieChart;

        /// <summary>
        /// 玩家2饼图
        /// </summary>
        public PieChartController Player2PieChart;

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
        /// 输入
        /// </summary>
        public PlayerInput Input;

        /// <summary>
        /// 平台生成表
        /// </summary>
        public List<PlatformGenerateItem> PlatformGenerateTable;

        /// <summary>
        /// 掷骰子选项列表
        /// </summary>
        public List<OptionItem> RollOptionsList;

        /// <summary>
        /// 掷特殊骰子选项列表
        /// </summary>
        public List<OptionItem> SpecialOptionsList;

        /// <summary>
        /// 事件方块选项列表
        /// </summary>
        public List<OptionItem> EventOptionsList;

        /// <summary>
        /// 陷阱方块选项列表
        /// </summary>
        public List <OptionItem> TrapOptionsList;

        /// <summary>
        /// 存放效果
        /// </summary>
        public GameObject Effects;


        [Header("时间轴")]
        /// <summary>
        /// 开场时间轴
        /// </summary>
        public PlayableDirector OpeningDirector;

        /// <summary>
        /// 掷骰子时间轴
        /// </summary>
        public PlayableDirector RollingDirector;

        [Header("GUI")]
        /// <summary>
        /// UI:标题
        /// </summary>
        public Text UITitle;

        /// <summary>
        /// UI:介绍
        /// </summary>
        public Text UIDescription;

        /// <summary>
        /// UI:玩家1进度
        /// </summary>
        public Text UIPlayer1Progess;

        /// <summary>
        /// UI:玩家2进度
        /// </summary>
        public Text UIPlayer2Progess;

        /// <summary>
        /// UI:玩家1特殊骰子冷却
        /// </summary>
        public Text UIPlayer1CoolDown;

        /// <summary>
        /// UI:玩家2特殊骰子冷却
        /// </summary>
        public Text UIPlayer2CoolDown; 

        [Header("运行数据")]

        /// <summary>
        /// 当前游戏状态
        /// </summary>
        public GameState CurrentGameState = GameState.GameStart;

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

        /// <summary>
        /// 平台方块总概率
        /// </summary>
        private float SumPlatformBlocksOdds = 0;

        /// <summary>
        /// 玩家1概率列表
        /// </summary>
        public List<float> Player1Odds;

        /// <summary>
        /// 玩家1概率变化列表
        /// </summary>
        public List<float> Player1OddsDelta;

        /// <summary>
        /// 玩家1概率等级
        /// </summary>
        [Range(0, 4)]
        public int Player1OddsLevel;

        /// <summary>
        /// 玩家1概率等级变化
        /// </summary>
        public int Player1OddsLevelDelta;

        /// <summary>
        /// 玩家1特殊骰子冷却
        /// </summary>
        public int Player1SpecialRollCoolDown = 0;

        /// <summary>
        /// 玩家2概率列表
        /// </summary>
        public List<float> Player2Odds;

        /// <summary>
        /// 玩家2概率变化列表
        /// </summary>
        public List<float> Player2OddsDelta;

        /// <summary>
        /// 玩家2概率等级
        /// </summary>
        [Range(0, 4)]
        public int Player2OddsLevel;

        /// <summary>
        /// 玩家2概率等级变化
        /// </summary>
        public int Player2OddsLevelDelta;

        /// <summary>
        /// 玩家2特殊骰子冷却
        /// </summary>
        public int Player2SpecialRollCoolDown = 0;

        /// <summary>
        /// 玩家概率等级列表
        /// </summary>
        private List<List<float>> PlayerOddsLevelList = new() { 
            new List<float>() { 100f, -1000f, -1000f, -1000f, -1000f, -1000f, -1000f, -1000f},  // 0级，全1
            new List<float>() { 100f, 100f, 100f, 100f, -1000f, -1000f, -1000f, -1000f },       // 1级，4面
            new List<float>() { 100f, 100f, 100f, 100f, 100f, 100f, -1000f, -1000f},            // 2级，6面（默认）
            new List<float>() { 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f },               // 3级，8面
            new List<float>() { -1000f, -1000f, -1000f, -1000f, -1000f, 100f, -1000f, -1000f},  // 4级，全1
        };

        /// <summary>
        /// 特殊骰子概率列表
        /// </summary>
        private List<float> Specialodds = new() { 16f, 16f, 16f, 16f, 16f, 16f, 2f, 2f };

        /// <summary>
        /// 回合开始委托
        /// </summary>
        private IEffectBase.TurnStartCallBack TurnStartCallBack = null;

        /// <summary>
        /// 回合结束委托
        /// </summary>
        private IEffectBase.TurnEndCallBack TurnEndCallBack = null;

        /// <summary>
        /// 清除失效效果委托
        /// </summary>
        private IEffectBase.LifeCycleCallBack LifeCycleCallBack = null;

        /// <summary>
        /// 启用状态检查
        /// </summary>
        private bool EnableStateCheck = false;

        /// <summary>
        /// 状态占用
        /// </summary>
        private int StateBlock = 0;

        /// <summary>
        /// 玩家1进度
        /// </summary>
        public int Player1Progress;

        /// <summary>
        /// 玩家2进度
        /// </summary>
        public int Player2Progress;

        private void Start()
        {
            GeneratePlatform();
            OnChangeState();
            ResetOdds();
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

        private void OnValidate()
        {
            //CalcOdds();
        }

        private void Update()
        {
            StateCheck();
        }

        #region 平台
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
            SumPlatformBlocksOdds = 0;
            foreach (var item in PlatformGenerateTable) {
                SumPlatformBlocksOdds += item.Odds;
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
            float rand = UnityEngine.Random.Range(0f, SumPlatformBlocksOdds);
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

        #endregion

        #region 时间轴与状态
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
                case GameState.GameStart:
                    CurrentGameState = GameState.Opening;
                    Player1Progress = 0;
                    Player2Progress = 0;
                    UpdateProgress();
                    DataSystem.Instance.SetData("Player1Reverse", 0);
                    DataSystem.Instance.SetData("Player2Reverse", 0);
                    PlayAndInvoke(OpeningDirector);
                    Player1SpecialRollCoolDown = Player2SpecialRollCoolDown = SpecialRollCoolDown;
                    UIPlayer1CoolDown.text = Player1SpecialRollCoolDown.ToString();
                    UIPlayer2CoolDown.text = Player2SpecialRollCoolDown.ToString();
                    break;
                // 开场运镜 -> 玩家开始
                case GameState.Opening:
                    CurrentGameState = GameState.PlayerStart;
                    EnableStateCheck = true;
                    break;
                // 玩家开始 -> 委托：回合开始
                case GameState.PlayerStart:
                    CurrentGameState = GameState.DelegateStart;
                    TurnStartCallBack?.Invoke();
                    EnableStateCheck = true;
                    if(CurrentPlayer == 1 && Player1SpecialRollCoolDown > 0)
                    {
                        Player1SpecialRollCoolDown--;
                        UIPlayer1CoolDown.text = Player1SpecialRollCoolDown.ToString();
                    }else if (CurrentPlayer == 2 && Player2SpecialRollCoolDown > 0)
                    {
                        Player2SpecialRollCoolDown--;
                        UIPlayer2CoolDown.text = Player2SpecialRollCoolDown.ToString();
                    }
                    break;
                // 委托：回合开始 -> 玩家闲置
                case GameState.DelegateStart:
                    CurrentGameState = GameState.PlayerIdle;
                    break;
                // 掷骰子 -> 移动
                case GameState.Rolling:
                    var rollPerfab = RollOptionsList[(int)DataSystem.Instance.GetData("RollResult")].Effects[0];
                    var rollStep = (int)DataSystem.Instance.GetData("RollResult");
                    if (CurrentPlayer == 1)
                    {
                        var obj = Instantiate(rollPerfab, Effects.transform.position, Quaternion.identity, Effects.transform);
                        obj.GetComponent<IEffectBase>().OnInstantiated(Player1, new object[] { rollStep, CurrentPlayer });
                    }
                    else
                    {
                        var obj = Instantiate(rollPerfab, Effects.transform.position, Quaternion.identity, Effects.transform);
                        obj.GetComponent<IEffectBase>().OnInstantiated(Player2, new object[] { rollStep, CurrentPlayer });
                    }
                    CurrentGameState = GameState.Moving;
                    EnableStateCheck = true;
                    break;
                // 移动 -> 委托：回合结束 | 方块效果
                case GameState.Moving:
                    if((CurrentPlayer == 1 ? PlatformBlocks1[Player1Progress].GetComponent<Block>().Type : PlatformBlocks2[Player2Progress].GetComponent<Block>().Type) == Block.BlockType.Raffle)
                    {
                        CurrentGameState = GameState.BlockEffect;
                        var rand = UnityEngine.Random.Range(0, EventOptionsList.Count);
                        var effects = EventOptionsList[rand].Effects;
                        var index = UnityEngine.Random.Range(0, effects.Count);
                        var perfab = effects[index];
                        var obj = Instantiate(perfab, CurrentPlayer == 1 ? Effects.transform.position : Player2.transform.position, Quaternion.identity, Effects.transform);
                        obj.GetComponent<IEffectBase>().Register(TurnStartCallBack, TurnEndCallBack, LifeCycleCallBack);
                        obj.GetComponent<IEffectBase>().OnInstantiated(Player1);
                        UITitle.text = $"{EventOptionsList[rand].Title} : {effects[index].GetComponent<IEffectBase>().Name}";
                        UIDescription.text = effects[index].GetComponent<IEffectBase>().Description;
                        PlayAndInvoke(RollingDirector);
                    }
                    else if ((CurrentPlayer == 1 ? PlatformBlocks1[Player1Progress].GetComponent<Block>().Type : PlatformBlocks2[Player2Progress].GetComponent<Block>().Type) == Block.BlockType.Trap)
                    {
                        CurrentGameState = GameState.BlockEffect;
                        var rand = UnityEngine.Random.Range(0, TrapOptionsList.Count);
                        var effects = EventOptionsList[rand].Effects;
                        var index = UnityEngine.Random.Range(0, effects.Count);
                        var perfab = effects[index];
                        var obj = Instantiate(perfab, CurrentPlayer == 1 ? Effects.transform.position : Player2.transform.position, Quaternion.identity, Effects.transform);
                        obj.GetComponent<IEffectBase>().Register(TurnStartCallBack, TurnEndCallBack, LifeCycleCallBack);
                        obj.GetComponent<IEffectBase>().OnInstantiated(Player1);
                        UITitle.text = $"{EventOptionsList[rand].Title} : {effects[index].GetComponent<IEffectBase>().Name}";
                        UIDescription.text = effects[index].GetComponent<IEffectBase>().Description;
                        PlayAndInvoke(RollingDirector);
                    }
                    else
                    {
                        CurrentGameState = GameState.DelegateEnd;
                        TurnEndCallBack?.Invoke();
                        EnableStateCheck = true;
                    }
                    break;
                // 方块效果 --> 委托：回合结束
                case GameState.BlockEffect:
                    CurrentGameState = GameState.DelegateEnd;
                    TurnEndCallBack?.Invoke();
                    EnableStateCheck = true;
                    break;
                // 委托：回合结束 -> 切换玩家
                case GameState.DelegateEnd:
                    CurrentGameState = GameState.SwitchPlayer;
                    SwitchPlayer(CurrentPlayer == 1 ? 2 : 1);
                    EnableStateCheck = true;
                    break;
                // 切换玩家 -> 委托：清除失效效果
                case GameState.SwitchPlayer:
                    CurrentGameState = GameState.DelegateLC;
                    LifeCycleCallBack?.Invoke();
                    EnableStateCheck = true;
                    break;
                // 委托：清除失效效果 -> 玩家开始
                case GameState.DelegateLC:
                    CurrentGameState = GameState.PlayerStart;
                    EnableStateCheck = true;
                    break;
                // 特殊骰子 -> 特殊骰子效果
                case GameState.SpecialRolling:
                    var specialPerfab = SpecialOptionsList[(int)DataSystem.Instance.GetData("RollResult") - 1].Effects[(int)DataSystem.Instance.GetData("EffectIndex")];
                    var specialData = (int)DataSystem.Instance.GetData("RollResult");
                    if (CurrentPlayer == 1)
                    {
                        var obj = Instantiate(specialPerfab, Effects.transform.position, Quaternion.identity, Effects.transform);
                        obj.GetComponent<IEffectBase>().Register(TurnStartCallBack, TurnEndCallBack, LifeCycleCallBack);
                        obj.GetComponent<IEffectBase>().OnInstantiated(Player1, new object[] { specialData });
                    }
                    else
                    {
                        var obj = Instantiate(specialPerfab, Effects.transform.position, Quaternion.identity, Effects.transform);
                        obj.GetComponent<IEffectBase>().Register(TurnStartCallBack, TurnEndCallBack, LifeCycleCallBack);
                        obj.GetComponent<IEffectBase>().OnInstantiated(Player2, new object[] { specialData });
                    }
                    CurrentGameState = GameState.SpecialEffect;
                    EnableStateCheck = true;
                    break;
                // 特殊骰子效果 -> 玩家闲置:
                case GameState.SpecialEffect:
                    CurrentGameState = GameState.PlayerIdle;
                    EnableStateCheck = true;
                    break;
            }
        }

        private void StateCheck()
        {
            if (!EnableStateCheck)
                return;
            if(StateBlock <= 0)
            {
                StateBlock = 0;
                EnableStateCheck = false;
                OnChangeState();
            }
        }

        public void Win()
        {
            CurrentGameState = GameState.Win;
            UITitle.text = $"Player {CurrentPlayer} Win!";
            PlayAndInvoke(RollingDirector);
        }
        #endregion

        #region 玩家

        public void SwitchPlayer(int player)
        {
            if(player == 1)
            {
                CurrentPlayer = 1;
                Player1Camera.SetActive(true);
                Player2Camera.SetActive(false);
                Input.SwitchCurrentActionMap("Player1");
            }
            else if(player == 2)
            {
                CurrentPlayer = 2;
                Player1Camera.SetActive(false);
                Player2Camera.SetActive(true);
                Input.SwitchCurrentActionMap("Player2Keyboard");
            }
        }

        public void UpdateProgress()
        {
            UIPlayer1Progess.text = Player1Progress.ToString();
            UIPlayer2Progess.text = Player2Progress.ToString();
        }

        #endregion

        #region 掷骰子

        /// <summary>
        /// 重置概率
        /// </summary>
        /// <param name="player">玩家编号</param>
        private void ResetOdds(int player = 0)
        {
            if(player == 0 || player == 1)
            {
                Player1OddsDelta = new() { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f};
                Player1OddsLevel = 2;
                Player1OddsLevelDelta = 0;
                CalcOdds(1);
            } 
            if(player == 0 || player == 2)
            {
                Player2OddsDelta = new() { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
                Player2OddsLevel = 2;
                Player2OddsLevelDelta = 0;
                CalcOdds(2);
            }
        }

        /// <summary>
        /// 计算概率列表
        /// </summary>
        /// <param name="player">玩家序号</param>
        public void CalcOdds(int player = 0)
        {
            if (player == 0 || player == 1)
            {
                Player1Odds = new();
                for (int i = 0; i < 8; i++)
                {
                    Player1Odds.Add(Mathf.Clamp(Player1OddsDelta[i] + PlayerOddsLevelList[Math.Clamp(Player1OddsLevel + Player1OddsLevelDelta, 0, 4)][i], 0f, 1000f));
                }
                SyncPieChart(1);
            }
            if (player == 0 || player == 2)
            {
                Player2Odds = new();
                for (int i = 0; i < 8; i++)
                {
                    Player2Odds.Add(Mathf.Clamp(Player2OddsDelta[i] + PlayerOddsLevelList[Math.Clamp(Player2OddsLevel + Player2OddsLevelDelta, 0, 4)][i], 0f, 1000f));
                }
                SyncPieChart(2);
            }
        }

        /// <summary>
        /// 同步饼图
        /// </summary>
        /// <param name="player">玩家序号</param>
        private void SyncPieChart(int player = 0)
        {
            if(player == 0 || player == 1)
            {

                for(int i = 0; i < Player1Odds.Count; i++)
                {
                    var div = Player1PieChart.Divides[i];
                    div.ratio = Player1Odds[i];
                    Player1PieChart.Divides[i] = div;
                }
                Player1PieChart.OnValueChanged();
            } 
            if (player == 0 || player == 2)
            {

                for (int i = 0; i < Player1Odds.Count; i++)
                {
                    var div = Player2PieChart.Divides[i];
                    div.ratio = Player2Odds[i];
                    Player2PieChart.Divides[i] = div;
                }
                Player2PieChart.OnValueChanged();
            }
        }

        private int GetRollResult(List<float> odds)
        {
            float sum = 0f;
            foreach (var i in odds)
                sum += i;
            float r = UnityEngine.Random.Range(0,sum);
            for(int i = 0; i < odds.Count; i++)
            {
                if (r <= odds[i])
                    return i + 1;
                r -= odds[i];
            }
            return 1;
        }

        /// <summary>
        /// 玩家掷骰子
        /// </summary>
        /// <param name="Player">玩家序号</param>
        public void PlayerRoll(int player)
        {
            if(player == CurrentPlayer &&  CurrentGameState == GameState.PlayerIdle)
            {
                CurrentGameState = GameState.Rolling;
                int res = Math.Clamp(player == 1 ? GetRollResult(Player1Odds) + (int)DataSystem.Instance.GetData("Player1RollResultDelta"): GetRollResult(Player2Odds) + (int)DataSystem.Instance.GetData("Player2RollResultDelta"), 0, 100);
                UITitle.text = RollOptionsList[res - 1].Title;
                UIDescription.text = RollOptionsList[res - 1].Description;
                DataSystem.Instance.SetData("RollResult", res);
                PlayAndInvoke(RollingDirector);
            }
        }

        /// <summary>
        /// 玩家掷特殊骰子
        /// </summary>
        public void PlayerSpecialRoll(int player)
        {
            if(CurrentPlayer == 1)
            {
                if (Player1SpecialRollCoolDown > 0)
                    return;
                Player1SpecialRollCoolDown = SpecialRollCoolDown;
                UIPlayer1CoolDown.text = Player1SpecialRollCoolDown.ToString();
            }
            else
            {
                if (Player2SpecialRollCoolDown > 0)
                    return;
                Player2SpecialRollCoolDown = SpecialRollCoolDown;
                UIPlayer2CoolDown.text = Player2SpecialRollCoolDown.ToString();
            }
            if (player == CurrentPlayer && CurrentGameState == GameState.PlayerIdle)
            {
                CurrentGameState = GameState.SpecialRolling;
                int res = GetRollResult(Specialodds);
                var index = UnityEngine.Random.Range(0, SpecialOptionsList[res - 1].Effects.Count);
                UITitle.text = $"{SpecialOptionsList[res - 1].Title} : {SpecialOptionsList[res - 1].Effects[index].GetComponent<IEffectBase>().Name}";
                UIDescription.text = SpecialOptionsList[res - 1].Effects[index].GetComponent<IEffectBase>().Description;
                DataSystem.Instance.SetData("RollResult", res);
                DataSystem.Instance.SetData("EffectIndex", index);
                PlayAndInvoke(RollingDirector);
            }
        }

        #endregion
    }
}