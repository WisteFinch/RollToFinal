using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
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
            Win,
            /// <summary>
            /// 判定
            /// </summary>
            Judge
        }

        public delegate void ChangeStateCallBack();

        public delegate void BlockChangeCallBack();

        [Header("游戏参数")]
        public int Length = 200;

        public int SpecialRollCoolDown = 2;

        [Header("对象引用")]

        /// <summary>
        /// 玩家平台
        /// </summary>
        public GameObject Platform;

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
        /// 机遇方块选项列表
        /// </summary>
        public List<OptionItem> RaffleOptionsList;

        /// <summary>
        /// 机遇方块效果
        /// </summary>
        public GameObject EffectRaffle;

        /// <summary>
        /// 陷阱方块效果
        /// </summary>
        public GameObject EffectTrap;

        /// <summary>
        /// 存放效果
        /// </summary>
        public GameObject Effects;

        /// <summary>
        /// 临时存放效果实例
        /// </summary>
        private GameObject TempEffectInstance;

        /// <summary>
        /// 粒子系统
        /// </summary>
        public particlesC Particles;

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

        /// <summary>
        /// GUI动画控制器
        /// </summary>
        public Animator GUIAnimator;

        /// <summary>
        /// GUI控制器
        /// </summary>
        public GUIController GUICTL;

        [Header("运行数据")]

        /// <summary>
        /// 当前游戏状态
        /// </summary>
        public GameState CurrentGameState = GameState.GameStart;

        /// <summary>
        /// 游戏状态暂存
        /// </summary>
        public GameState TempGameState;

        /// <summary>
        /// 当前玩家
        /// </summary>
        public int CurrentPlayer = 1;

        /// <summary>
        /// 玩家平台方块
        /// </summary>
        [HideInInspector]
        public List<GameObject> PlatformBlocks = new();

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
        /// 玩家1幸运抵消
        /// </summary>
        public int Player1LuckyBalance = 0;

        /// <summary>
        /// 玩家1厄运抵消
        /// </summary>
        public int Player1CalamityBalance = 0;

        /// <summary>
        /// 玩家1进度
        /// </summary>
        public int Player1Progress;

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
        /// 玩家12幸运抵消
        /// </summary>
        public int Player2LuckyBalance = 0;

        /// <summary>
        /// 玩家2厄运抵消
        /// </summary>
        public int Player2CalamityBalance = 0;

        /// <summary>
        /// 玩家2进度
        /// </summary>
        public int Player2Progress;

        /// <summary>
        /// 玩家概率等级列表
        /// </summary>
        private readonly List<List<float>> PlayerOddsLevelList = new() { 
            new List<float>() { 100f, -1000f, -1000f, -1000f, -1000f, -1000f, -1000f, -1000f},  // 0级，全1
            new List<float>() { 100f, 100f, 100f, 100f, -1000f, -1000f, -1000f, -1000f },       // 1级，4面
            new List<float>() { 100f, 100f, 100f, 100f, 100f, 100f, -1000f, -1000f},            // 2级，6面（默认）
            new List<float>() { 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f },               // 3级，8面
            new List<float>() { -1000f, -1000f, -1000f, -1000f, -1000f, 100f, -1000f, -1000f},  // 4级，全1
        };

        /// <summary>
        /// 特殊骰子概率列表
        /// </summary>
        public List<float> Specialodds = new() { 16f, 16f, 16f, 16f, 16f, 16f, 2f, 2f };

        /// <summary>
        /// 回合开始委托
        /// </summary>
        public TurnStartCallBack TurnStartCallBack = null;

        /// <summary>
        /// 回合结束委托
        /// </summary>
        public TurnEndCallBack TurnEndCallBack = null;

        /// <summary>
        /// 清除失效效果委托
        /// </summary>
        public LifeCycleCallBack LifeCycleCallBack = null;

        /// <summary>
        /// 游戏状态改变委托
        /// </summary>
        public ChangeStateCallBack CSCallBack = null;

        /// <summary>
        /// 方块改变委托
        /// </summary>
        public ChangeStateCallBack BCCallBack = null;

        /// <summary>
        /// 启用状态检查
        /// </summary>
        private bool EnableStateCheck = false;

        /// <summary>
        /// 状态占用
        /// </summary>
        public int StateBlock = 0;

        /// <summary>
        /// 判断结果
        /// </summary>
        public int JudgeResult = 0;

        /// <summary>
        /// GUI状态
        /// </summary>
        public int GUIState = 0;

        public int BlockEffectCheck = 0;

        private void Start()
        {
            GeneratePlatform();
            OnChangeState();
            ResetOdds();
        }

        private void OnDestroy()
        {
            // 销毁原有平台
            PlatformBlocks.Clear();
            foreach (var item in PlatformBlocks)
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
            UpdateProgress();
        }

        #region 平台
        /// <summary>
        /// 生成平台
        /// </summary>
        public void GeneratePlatform()
        {
            float delay = 0f;
            // 销毁原有平台
            PlatformBlocks.Clear();
            foreach (var item in PlatformBlocks) {
                Destroy(item);
            }
            // 计算概率和
            SumPlatformBlocksOdds = 0;
            foreach (var item in PlatformGenerateTable) {
                SumPlatformBlocksOdds += item.Odds;
            }
            // 创建起点
            AddBlock(BeginBlock, delay);
            // 创建平台
            for(int i = 1; i <= Length; i++)
            {
                delay += Time.fixedDeltaTime;
                AddBlock(GenerateBlock(), delay);
            }
            // 创建终点
            AddBlock(EndBlock, delay + Time.fixedDeltaTime);
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
        private void AddBlock(GameObject perfab, float delay = -1)
        {
            GameObject obj = Instantiate(perfab, new Vector3(Platform.transform.position.x, Platform.transform.position.y, PlatformBlocks.Count + Platform.transform.position.z), Quaternion.identity, Platform.transform);
            obj.GetComponent<Block>().Index = PlatformBlocks.Count;
            PlatformBlocks.Add(obj);
            if (delay != -1)
                obj.GetComponent<Block>().Entry(delay);
        }

        /// <summary>
        /// 替换方块
        /// </summary>
        /// <param name="target">目标方块</param>
        /// <param name="perfab">预制体</param>
        /// /// <param name="boom">爆炸效果</param>
        public void ReplaceBlock(GameObject target, GameObject perfab, bool boom = false)
        {
            GameObject obj;
            int index = target.GetComponent<Block>().Index;
            obj = Instantiate(perfab, target.transform.position, Quaternion.identity, Platform.transform);
            obj.GetComponent<Block>().Index = index;
            if (boom)
                PlatformBlocks[index].GetComponent<Block>().Boom();
            else
                PlatformBlocks[index].GetComponent<Block>().Escape();
            PlatformBlocks[index] = obj;
            PlatformBlocks[index].GetComponent<Block>().Entry();
            BCCallBack?.Invoke();
        }

        #endregion

        #region 时间轴与状态
        /// <summary>
        /// 时间轴动画结束后调用状态改变函数
        /// </summary>
        /// <param name="director">时间轴</param>
        private void InvokeAfterTimelineFinish(PlayableDirector director)
        {
            Invoke(nameof(OnChangeState), (float)director.duration + 0.05f);
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
            CSCallBack?.Invoke();
            switch (CurrentGameState)
            {
                case GameState.GameStart:
                    DataSystem.Instance.ClearData();
                    CurrentGameState = GameState.Opening;
                    Player1Progress = 0;
                    Player2Progress = 0;
                    Player1SpecialRollCoolDown = Player2SpecialRollCoolDown = SpecialRollCoolDown;
                    DrawCoolDown();
                    //DataSystem.Instance.SetData("Player1Reverse", 0);
                    //DataSystem.Instance.SetData("Player2Reverse", 0);
                    //DataSystem.Instance.SetData("Player1JudgeBonus", 0);
                    //DataSystem.Instance.SetData("Player2JudgeBonus", 0);
                    //DataSystem.Instance.SetData("Player1JudgeBalance", 0);
                    //DataSystem.Instance.SetData("Player2JudgeBalance", 0);
                    PlayAndInvoke(OpeningDirector);
                    break;
                // 开场运镜 -> 玩家开始
                case GameState.Opening:
                    SwitchPlayer(1);
                    CurrentGameState = GameState.PlayerStart;
                    EnableStateCheck = true;
                    break;
                // 玩家开始 -> 委托：回合开始
                case GameState.PlayerStart:
                    CurrentGameState = GameState.DelegateStart;
                    TurnStartCallBack?.Invoke();
                    BlockEffectCheck = 0;
                    EnableStateCheck = true;
                    // 计算特殊骰子冷却
                    if(CurrentPlayer == 1)
                    {
                        if(DataSystem.Instance.GetData("Player1SpecialRollCount") == 0)
                        {
                            if (Player1SpecialRollCoolDown > 0)
                            {
                                Player1SpecialRollCoolDown--;
                            }
                            if (Player1SpecialRollCoolDown <= 0)
                            {
                                Player1SpecialRollCoolDown = SpecialRollCoolDown;
                                DataSystem.Instance.SetData("Player1SpecialRollCount", DataSystem.Instance.GetData("Player1SpecialRollCount") + 1);
                            }
                        }
                    }
                    else
                    {
                        if (DataSystem.Instance.GetData("Player2SpecialRollCount") == 0)
                        {
                            if (Player2SpecialRollCoolDown > 0)
                            {
                                Player2SpecialRollCoolDown--;
                            }
                            if (Player2SpecialRollCoolDown <= 0)
                            {
                                Player2SpecialRollCoolDown = SpecialRollCoolDown;
                                DataSystem.Instance.SetData("Player2SpecialRollCount", DataSystem.Instance.GetData("Player2SpecialRollCount") + 1);
                            }
                        }
                    }
                    DrawCoolDown();
                    break;
                // 委托：回合开始 -> 方块效果
                case GameState.DelegateStart:
                    CurrentGameState = GameState.Moving;
                    EnableStateCheck = true;
                    break;
                // 掷骰子 -> 移动
                case GameState.Rolling:
                    // 应用效果
                    CurrentGameState = GameState.Moving;
                    TempEffectInstance.GetComponent<IEffectBase>().Register();
                    TempEffectInstance.GetComponent<IEffectBase>().OnAssert();
                    EnableStateCheck = true;
                    break;
                // 移动 -> 委托：回合结束 | 方块效果
                case GameState.Moving:
                    if (PlatformBlocks[CurrentPlayer == 1 ? Player1Progress : Player2Progress].GetComponent<Block>().Type == Block.BlockType.Raffle)
                    {
                        EnableStateCheck = false;
                        CurrentGameState = GameState.BlockEffect;
                        var obj = Instantiate(EffectRaffle, Effects.transform.position, Quaternion.identity, Effects.transform);
                        obj.GetComponent<IEffectBase>().OnInstantiated();
                        TempEffectInstance = obj;
                        UITitle.text = obj.GetComponent<IEffectBase>().Name;
                        UIDescription.text = obj.GetComponent<IEffectBase>().Description;
                        GUIState = 21;
                        obj.GetComponent<IEffectBase>().Register();
                        TempEffectInstance.GetComponent<IEffectBase>().OnAssert();
                        OnGUIStateChange();
                        BlockEffectCheck++;
                    }
                    else if (PlatformBlocks[CurrentPlayer == 1 ? Player1Progress : Player2Progress].GetComponent<Block>().Type == Block.BlockType.Trap)
                    {
                        EnableStateCheck = false;
                        CurrentGameState = GameState.BlockEffect;
                        var obj = Instantiate(EffectTrap, Effects.transform.position, Quaternion.identity, Effects.transform);
                        obj.GetComponent<IEffectBase>().OnInstantiated();
                        TempEffectInstance = obj;
                        UITitle.text = obj.GetComponent<IEffectBase>().Name;
                        UIDescription.text = "判定值大于3：概率升级\n判定值小于等于3：概率降级";
                        GUIState = 31;
                        obj.GetComponent<IEffectBase>().Register();
                        TempEffectInstance.GetComponent<IEffectBase>().OnAssert();
                        OnGUIStateChange();
                        BlockEffectCheck++;
                    }
                    else
                    {
                        if (BlockEffectCheck == 0)
                        {
                            BlockEffectCheck++;
                            CurrentGameState = GameState.PlayerIdle;
                        }
                        else
                        {
                            BlockEffectCheck++;
                            CurrentGameState = GameState.DelegateEnd;
                            EnableStateCheck = true;
                        }
                    }
                    break;
                // 方块效果 --> 委托：回合结束
                case GameState.BlockEffect:
                    if (BlockEffectCheck == 1)
                    {
                        CurrentGameState = GameState.PlayerIdle;
                    }
                    else
                    {
                        CurrentGameState = GameState.DelegateEnd;
                        EnableStateCheck = true;
                    }
                    break;
                // 委托：回合结束 -> 切换玩家
                case GameState.DelegateEnd:
                    TurnEndCallBack?.Invoke();
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
                    CurrentGameState = GameState.SpecialEffect;
                    // 应用效果
                    if (TempEffectInstance != null)
                    {
                        TempEffectInstance.GetComponent<IEffectBase>().Register();
                        TempEffectInstance.GetComponent<IEffectBase>().OnAssert();
                    }
                    EnableStateCheck = true;
                    break;
                // 特殊骰子效果 -> 玩家闲置:
                case GameState.SpecialEffect:
                    CurrentGameState = GameState.PlayerIdle;
                    EnableStateCheck = true;
                    break;
                // 判定 -> 返回上一状态
                case GameState.Judge:
                    CurrentGameState = TempGameState;
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
            UITitle.text = $"玩家{CurrentPlayer}胜利！";
            UIDescription.text = "";
            EnableStateCheck = false;
            StateBlock += 1000;
            for(int i = 0; i < PlatformBlocks.Count - 1; i++)
            {
                PlatformBlocks[i].GetComponent<Block>().Boom(i * Time.fixedDeltaTime);
            }
            int count = UnityEngine.Random.Range(3, 7);
            float r = 360f / count;
            for(int i = 0; i < count; i++)
            {
                Vector3 pos = new(MathF.Sin(Mathf.Deg2Rad * i * r) * 30, 0, MathF.Cos(Mathf.Deg2Rad * i * r) * 30 + Length);
                Particles.getFirework(pos);
            }
            GUIAnimator.SetTrigger("TitleEntry");
            if (CurrentPlayer == 1)
            {
                Player2.GetComponent<PlayerController>().enabled = false;
                Player2.AddComponent<Rigidbody>();
            }
            else
            {
                Player1.GetComponent<PlayerController>().enabled = false;
                Player1.AddComponent<Rigidbody>();
            }
                
            Invoke(nameof(BackToMenu), 10f);
        }

        public void BackToMenu()
        {
            SceneManager.LoadScene(0);
        }

        public void OnGUIStateChange()
        {
            switch(GUIState)
            {
                // 1 ：移动开始，标题进入
                case 1:
                    StateBlock++;
                    GUIAnimator.SetTrigger("TitleEntry");
                    GUIState = 2;
                    break;
                // 2 : 转盘进入
                case 2:
                    GUICTL.SetDivides(CurrentPlayer == 1 ? Player1PieChart.PieChart.Divides : Player2PieChart.PieChart.Divides);
                    GUIAnimator.SetTrigger("CycleEntry");
                    GUIState = 3;
                    break;
                // 3 : 开转
                case 3:
                    GUIState = 4;
                    GUICTL.Roll(DataSystem.Instance.GetData("RollResult") - 1);
                    break;
                // 4 : 转盘消失
                case 4:
                    UITitle.text = $"[{DataSystem.Instance.GetData("RollResult")}]移动";
                    UIDescription.text = TempEffectInstance.GetComponent<IEffectBase>().Description;
                    GUIAnimator.SetTrigger("CycleEscape");
                    GUIState = 5;
                    break;
                // 5 : 显示提示，等待
                case 5:
                    Invoke(nameof(OnGUIStateChange), 2f);
                    GUIState = 6;
                    break;
                // 6 : 标题离开
                case 6:
                    GUIAnimator.SetTrigger("TitleEscape");
                    GUIState = 7;
                    break;
                // 7 : 结束
                case 7:
                    StateBlock--;
                    OnChangeState();
                    GUIState = 0;
                    break;

                // 11 ：特殊开始，标题进入
                case 11:
                    StateBlock++;
                    GUIAnimator.SetTrigger("TitleEntry");
                    GUIState = 12;
                    break;
                // 12 : 转盘进入
                case 12:
                    List<DrawPieChart.DivideItem> sdivs = new();
                    var sPie = CurrentPlayer == 1 ? Player1PieChart.PieChart.Divides : Player2PieChart.PieChart.Divides;
                    for (int i = 0; i < Specialodds.Count; i++)
                    {
                        DrawPieChart.DivideItem item = sPie[i];
                        item.title = SpecialOptionsList[i].Title;
                        item.ratio = Specialodds[i];
                        sdivs.Add(item);
                    }
                    GUICTL.SetDivides(sdivs);
                    GUIAnimator.SetTrigger("CycleEntry");
                    GUIState = 13;
                    break;
                // 13 : 开转
                case 13:
                    GUIState = 14;
                    GUICTL.Roll(DataSystem.Instance.GetData("RollResult"));
                    break;
                // 14 : 转盘消失
                case 14:
                    var effect = TempEffectInstance.GetComponent<IEffectBase>();
                    int res = DataSystem.Instance.GetData("RollResult");
                    if (CalcBalance(effect.Target, effect.Type))
                    {
                        UITitle.text = $"[{SpecialOptionsList[res].Title}]抽奖 : {effect.Name}";
                        UIDescription.text = effect.Description;
                    }
                    else
                    {
                        UITitle.text = $"[{SpecialOptionsList[res].Title}]抽奖 : {effect.Name}";
                        UIDescription.text = "该效果已被抵消";
                        Destroy(TempEffectInstance);
                    }
                    GUIAnimator.SetTrigger("CycleEscape");
                    GUIState = 15;
                    break;
                // 15 : 显示提示，等待
                case 15:
                    Invoke(nameof(OnGUIStateChange), 2f);
                    GUIState = 16;
                    break;
                // 16 : 标题离开
                case 16:
                    GUIAnimator.SetTrigger("TitleEscape");
                    GUIState = 17;
                    break;
                // 17 : 结束
                case 17:
                    StateBlock--;
                    OnChangeState();
                    GUIState = 0;
                    break;
                // 21 ：机遇开始，标题进入
                case 21:
                    StateBlock++;
                    GUIAnimator.SetTrigger("TitleEntry");
                    GUIState = 28;
                    break;
                // 28 ：等待
                case 28:
                    Invoke(nameof(OnGUIStateChange), 2f);
                    GUIState = 29;
                    break;
                // 29 ：检查状态
                case 29:
                    GUIState = 22;
                    OnGUIStateChange();
                    break;
                // 22 : 转盘进入
                case 22:
                    TempEffectInstance.GetComponent<EffectRaffle>().CheckJudgeResult();
                    List<DrawPieChart.DivideItem> rdivs = new();
                    var rPie = CurrentPlayer == 1 ? Player1PieChart.PieChart.Divides : Player2PieChart.PieChart.Divides;
                    for (int i = 0; i < 6; i++)
                    {
                        DrawPieChart.DivideItem item = rPie[i];
                        item.title = (i + 1).ToString();
                        item.ratio = 1f;
                        rdivs.Add(item);
                    }
                    GUICTL.SetDivides(rdivs);
                    UIDescription.text = "";
                    GUIAnimator.SetTrigger("CycleEntry");
                    GUIState = 23;
                    break;
                // 23 : 开转
                case 23:
                    GUIState = 24;
                    GUICTL.Roll(Math.Clamp(TempEffectInstance.GetComponent<EffectRaffle>().JudgeResult - 1, 0, 5));
                    break;
                // 24 : 转盘消失
                case 24:
                    UITitle.text = $"[{TempEffectInstance.GetComponent<EffectRaffle>().JudgeResult}]{TempEffectInstance.GetComponent<IEffectBase>().Name}";
                    GUIAnimator.SetTrigger("CycleEscape");
                    GUIState = 25;
                    break;
                // 25 : 显示提示，等待
                case 25:
                    if(TempEffectInstance.GetComponent<EffectRaffle>().JudgeResult > 3)
                    {
                        UIDescription.text = TempEffectInstance.GetComponent<IEffectBase>().Description + "\n判定成功";
                    }
                    else
                    {
                        UIDescription.text = TempEffectInstance.GetComponent<IEffectBase>().Description + "\n判定失败";
                    }
                    Invoke(nameof(OnGUIStateChange), 2f);
                    GUIState = 26;
                    break;
                // 26 : 标题离开
                case 26:
                    TempEffectInstance.GetComponent<EffectRaffle>().Execute();
                    GUIAnimator.SetTrigger("TitleEscape");
                    GUIState = 27;
                    break;
                // 27 : 结束
                case 27:
                    StateBlock--;
                    OnChangeState();
                    GUIState = 0;
                    break;
                // 31 ：陷阱开始，标题进入
                case 31:
                    StateBlock++;
                    GUIAnimator.SetTrigger("TitleEntry");
                    GUIState = 38;
                    break;
                // 38 ：等待
                case 38:
                    Invoke(nameof(OnGUIStateChange), 2f);
                    GUIState = 39;
                    break;
                // 39 ：检查状态
                case 39:
                    GUIState = 32;
                    OnGUIStateChange();
                    break;
                // 32 : 转盘进入
                case 32:
                    TempEffectInstance.GetComponent<EffectTrap>().CheckJudgeResult();
                    List<DrawPieChart.DivideItem> tdivs = new();
                    var tPie = CurrentPlayer == 1 ? Player1PieChart.PieChart.Divides : Player2PieChart.PieChart.Divides;
                    for (int i = 0; i < 6; i++)
                    {
                        DrawPieChart.DivideItem item = tPie[i];
                        item.title = (i + 1).ToString();
                        item.ratio = 1f;
                        tdivs.Add(item);
                    }
                    GUICTL.SetDivides(tdivs);
                    UIDescription.text = "";
                    GUIAnimator.SetTrigger("CycleEntry");
                    GUIState = 33;
                    break;
                // 33 : 开转
                case 33:
                    GUIState = 34;
                    GUICTL.Roll(Math.Clamp(TempEffectInstance.GetComponent<EffectTrap>().JudgeResult - 1, 0, 5));
                    break;
                // 34 : 转盘消失
                case 34:
                    UITitle.text = $"[{TempEffectInstance.GetComponent<EffectTrap>().JudgeResult}]{TempEffectInstance.GetComponent<IEffectBase>().Name}";
                    GUIAnimator.SetTrigger("CycleEscape");
                    GUIState = 35;
                    break;
                // 35 : 显示提示，等待
                case 35:
                    if (TempEffectInstance.GetComponent<EffectTrap>().JudgeResult > 3)
                    {
                        UIDescription.text = "判定值大于3：概率升级";
                    }
                    else
                    {
                        UIDescription.text = "判定值小于等于3：概率降级";
                    }
                    Invoke(nameof(OnGUIStateChange), 2f);
                    GUIState = 36;
                    break;
                // 36 : 标题离开
                case 36:
                    TempEffectInstance.GetComponent<EffectTrap>().Execute();
                    GUIAnimator.SetTrigger("TitleEscape");
                    GUIState = 37;
                    break;
                // 37 : 结束
                case 37:
                    StateBlock--;
                    OnChangeState();
                    GUIState = 0;
                    break;

                default:
                    GUIState = 0;
                    break;
            }
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
                if(Transfer.Instance.EnableAI)
                {
                    if (Transfer.Instance.UseJoyStick)
                    {
                        Input.SwitchCurrentActionMap("Player2Gamepad");
                    }
                    else
                    {
                        Input.SwitchCurrentActionMap("Player1");
                    }
                }
                else
                    Input.SwitchCurrentActionMap("Player1");
            }
            else if(player == 2)
            {
                CurrentPlayer = 2;
                Player1Camera.SetActive(false);
                Player2Camera.SetActive(true);
                if(Transfer.Instance.UseJoyStick)
                {
                    Input.SwitchCurrentActionMap("Player2Gamepad");
                }
                else
                {
                    Input.SwitchCurrentActionMap("Player2Keyboard");
                }

            }
        }

        public void UpdateProgress()
        {
            UIPlayer1Progess.text = "进度 : " + Player1Progress.ToString();
            UIPlayer2Progess.text = "进度 : " + Player2Progress.ToString();
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

        /// <summary>
        /// 获取骰子结果
        /// </summary>
        /// <param name="odds">概率分布</param>
        /// <returns>骰子结果</returns>
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
            if (CurrentGameState != GameState.PlayerIdle)
                return;
            if (player == CurrentPlayer &&  CurrentGameState == GameState.PlayerIdle)
            {
                CurrentGameState = GameState.Rolling;
                EnableStateCheck = false;
                // 获取点数
                int res = Math.Clamp(player == 1 ? GetRollResult(Player1Odds) + DataSystem.Instance.GetData("Player1RollResultDelta"): GetRollResult(Player2Odds) + DataSystem.Instance.GetData("Player2RollResultDelta"), 0, 100);
                DataSystem.Instance.SetData("RollResult", res);
                // 创建效果
                var perfab = RollOptionsList[res - 1].Effects[0];
                var step = DataSystem.Instance.GetData("RollResult");
                TempEffectInstance = Instantiate(perfab, Effects.transform.position, Quaternion.identity, Effects.transform);
                TempEffectInstance.GetComponent<IEffectBase>().OnInstantiated(new object[] { step, CurrentPlayer });
                // 设置UI
                UITitle.text = "移动";
                UIDescription.text = "";

                GUIState = 1;
                OnGUIStateChange();
            }
        }

        /// <summary>
        /// 玩家掷特殊骰子
        /// </summary>
        public void PlayerSpecialRoll(int player)
        {
            if (CurrentGameState != GameState.PlayerIdle)
                return;
            if(CurrentPlayer == 1)
            {
                if (DataSystem.Instance.GetData("Player1SpecialRollCount") <= 0)
                    return;
                DataSystem.Instance.SetData("Player1SpecialRollCount", DataSystem.Instance.GetData("Player1SpecialRollCount") - 1);
            }
            else
            {
                if (DataSystem.Instance.GetData("Player2SpecialRollCount") <= 0)
                    return;
                DataSystem.Instance.SetData("Player2SpecialRollCount", DataSystem.Instance.GetData("Player2SpecialRollCount") - 1);
            }
            DrawCoolDown();
            if (player == CurrentPlayer && CurrentGameState == GameState.PlayerIdle)
            {
                EnableStateCheck = false;
                CurrentGameState = GameState.SpecialRolling;
                // 获取点数&序号
                int balance = player == 1 ? DataSystem.Instance.GetData("Player1SpecialRollBalance") : DataSystem.Instance.GetData("Player2SpecialRollBalance");
                int res;
                if (balance == 0)
                {
                    res = GetRollResult(Specialodds);
                }else if(balance > 0)
                {
                    res = 8;
                    DataSystem.Instance.SetData(player == 1 ? "Player1SpecialRollBalance" : "Player2SpecialRollBalance", res - 1);
                }
                else
                {
                    res = 1;
                    DataSystem.Instance.SetData(player == 1 ? "Player1SpecialRollBalance" : "Player2SpecialRollBalance", res + 1);
                }
                var index = UnityEngine.Random.Range(0, SpecialOptionsList[res - 1].Effects.Count);
                DataSystem.Instance.SetData("RollResult", res - 1);
                DataSystem.Instance.SetData("EffectIndex", index);
                // 创建效果&设置UI
                var perfab = SpecialOptionsList[res - 1].Effects[index];
                TempEffectInstance = Instantiate(perfab, Effects.transform.position, Quaternion.identity, Effects.transform);
                var effect = TempEffectInstance.GetComponent<IEffectBase>();
                effect.OnInstantiated(new object[] { res - 1 });

                UITitle.text = "抽奖";
                UIDescription.text = "";

                GUIState = 11;
                OnGUIStateChange();
            }
        }

        /// <summary>
        /// 玩家判定
        /// </summary>
        /// <param name="player">玩家编号</param>
        public void PlayerJudge(int player)
        {
            EnableStateCheck = false;
            TempGameState = CurrentGameState;
            CurrentGameState = GameState.Judge;
            // 获取点数
            int res;
            int balance = player == 1 ? DataSystem.Instance.GetData("Player1JudgeBalance") : DataSystem.Instance.GetData("Player2JudgeBalance");
            int protection = player == 1 ? DataSystem.Instance.GetData("Player1JudgeProtection") : DataSystem.Instance.GetData("Player2JudgeProtection");
            if (protection > 0)
            {
                res = 6;
                DataSystem.Instance.SetData(player == 1 ? "Player1JudgeProtection" : "Player2JudgeProtection", protection - 1);
            }
            else
            {
                if (balance == 0)
                    res = UnityEngine.Random.Range(0, 6) + 1;
                else if (balance > 0)
                    res = 6;
                else
                    res = 1;
            }
            JudgeResult = res;
        }



        /// <summary>
        /// 计算效果抵消
        /// </summary>
        /// <param name="target">目标玩家序号</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        private bool CalcBalance(int target, IEffectBase.EffectType type)
        {
            if(target == 1)
            {
                if(type == IEffectBase.EffectType.Gain)
                {
                    if(Player1LuckyBalance > 0)
                    {
                        Player1LuckyBalance--;
                        return false;
                    }
                }
                else if (type == IEffectBase.EffectType.Reduce)
                {
                    if (Player1CalamityBalance > 0)
                    {
                        Player1CalamityBalance--;
                        return false;
                    }
                }
                else if(type == IEffectBase.EffectType.Calamity)
                {
                    if (DataSystem.Instance.GetData("Player1CalamityBalance") > 0)
                    {
                        DataSystem.Instance.SetData("Player1CalamityBalance", DataSystem.Instance.GetData("Player1CalamityBalance") - 1);
                        return false;
                    }
                }
                else if (type == IEffectBase.EffectType.Lucky)
                {
                    if (DataSystem.Instance.GetData("Player1LuckyBalance") > 0)
                    {
                        DataSystem.Instance.SetData("Player1LuckyBalance", DataSystem.Instance.GetData("Player1LuckyBalance") - 1);
                        return false;
                    }
                }
            }
            else
            {
                if (type == IEffectBase.EffectType.Gain)
                {
                    if (Player2LuckyBalance > 0)
                    {
                        Player2LuckyBalance--;
                        return false;
                    }
                }
                else if (type == IEffectBase.EffectType.Reduce)
                {
                    if (Player2CalamityBalance > 0)
                    {
                        Player2CalamityBalance--;
                        return false;
                    }
                }
                else if (type == IEffectBase.EffectType.Calamity)
                {
                    if (DataSystem.Instance.GetData("Player2CalamityBalance") > 0)
                    {
                        DataSystem.Instance.SetData("Player2CalamityBalance", DataSystem.Instance.GetData("Player2CalamityBalance") - 1);
                        return false;
                    }
                }
                else if (type == IEffectBase.EffectType.Lucky)
                {
                    if (DataSystem.Instance.GetData("Player1LuckyBalance") > 0)
                    {
                        DataSystem.Instance.SetData("Player2LuckyBalance", DataSystem.Instance.GetData("Player2LuckyBalance") - 1);
                        return false;
                    }
                }
            }
            return true;
        }

        public void DrawCoolDown()
        {
            if (DataSystem.Instance.GetData("Player1SpecialRollCount") > 0)
            {
                UIPlayer1CoolDown.text = $"抽奖就绪({DataSystem.Instance.GetData("Player1SpecialRollCount")})";
            }
            else
            {
                UIPlayer1CoolDown.text = "抽奖冷却 : " + Player1SpecialRollCoolDown.ToString();
            }

            if (DataSystem.Instance.GetData("Player2SpecialRollCount") > 0)
            {
                UIPlayer2CoolDown.text = $"抽奖就绪({DataSystem.Instance.GetData("Player2SpecialRollCount")})";
            }
            else
            {
                UIPlayer2CoolDown.text = "抽奖冷却 : " + Player2SpecialRollCoolDown.ToString();
            }
        }

        #endregion
    }
}