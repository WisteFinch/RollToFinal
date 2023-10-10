using System;
using UnityEngine;

namespace RollToFinal
{
    public class PlayerController : MonoBehaviour
    {
        /// <summary>
        /// 移动能力
        /// </summary>
        public int Movement;

        /// <summary>
        /// 玩家序号
        /// </summary>
        public int PlayerIndex = 1;

        /// <summary>
        /// 跳跃速度
        /// </summary>
        public float JumpSpeed = 1;

        /// <summary>
        /// 移动速度
        /// </summary>
        public float MoveSpeed = 2;

        /// <summary>
        /// 跳跃间隔
        /// </summary>
        public float Delay = 0.25f;

        /// <summary>
        /// 跳跃进度
        /// </summary>
        public float JumpProgress = 0f;

        /// <summary>
        /// 时间进度
        /// </summary>
        public float TimeProgress = 0f;

        /// <summary>
        /// 允许跳跃
        /// </summary>
        public bool EnableJump = false;

        /// <summary>
        /// 允许移动
        /// </summary>
        public bool EnableMove = true;

        /// <summary>
        /// 位置
        /// </summary>
        public Vector3 Pos = new();

        /// <summary>
        /// 当前行动力变化
        /// </summary>
        public int CurrentMovementDelta = 0;

        /// <summary>
        /// 可越过
        /// </summary>
        public bool Capable = true;

        void Start()
        {
            GameLogic.Instance.BCCallBack += OnBlockChange;
        }

        void Update()
        {
            CalcMove();
            if (!EnableJump)
                return;
            CalcJump();
        }

        void CalcJump()
        {
            // 计算进度
            int playerProgress = PlayerIndex == 1 ? GameLogic.Instance.Player1Progress : GameLogic.Instance.Player2Progress;
            // 计算停止
            if (Movement <= 0)
            {
                EnableJump = false;
                JumpProgress = 0f;
                TimeProgress = Delay;
                GameLogic.Instance.StateBlock--;
                OnBlockChange();
                return;
            }
            // 计算方向
            int dir = (PlayerIndex == 1 ? DataSystem.Instance.GetData("Player1Reverse") : DataSystem.Instance.GetData("Player2Reverse")) == 1 ? -1 : 1;
            // 计算是否跳完一步
            if (JumpProgress >= 1)
            {
                JumpProgress = 0f;
                TimeProgress = Delay;
                if (Capable)
                {
                    if (PlayerIndex == 1)
                    {
                        GameLogic.Instance.Player1Progress += dir;
                    }
                    else 
                    {
                        GameLogic.Instance.Player2Progress += dir;
                    }
                }
                Movement -= CurrentMovementDelta;
                // 边缘判断
                playerProgress = PlayerIndex == 1 ? GameLogic.Instance.Player1Progress : GameLogic.Instance.Player2Progress;
                if (playerProgress == 0)
                {
                    Movement = 0;
                    EnableJump = false;
                    JumpProgress = 0f;
                    TimeProgress = Delay;
                    GameLogic.Instance.StateBlock--;
                }
                else if (playerProgress == GameLogic.Instance.Length + 1)
                {
                    Movement = 0;
                    EnableJump = false;
                    EnableMove = false;
                    JumpProgress = 0f;
                    TimeProgress = Delay;
                    GameLogic.Instance.StateBlock--;
                    GameLogic.Instance.Win();
                }
            }
            else
            {
                // 计算跳跃进度
                if (TimeProgress > 0)
                {
                    TimeProgress -= Time.deltaTime;
                    return;
                }
                else
                {
                    JumpProgress += Time.deltaTime * JumpSpeed;
                }
                // 计算跳跃类型
                var currentBlock = GameLogic.Instance.PlatformBlocks[playerProgress].GetComponent<Block>().Type;
                var nextBlock = GameLogic.Instance.PlatformBlocks[playerProgress + dir].GetComponent<Block>().Type;
                if(currentBlock != Block.BlockType.Barrier && nextBlock != Block.BlockType.Barrier)
                {
                    CurrentMovementDelta = 1;
                    Capable = true;
                    Pos = new(Pos.x, Jump0To0(JumpProgress), playerProgress + JumpProgress * dir);
                } 
                else if (currentBlock == Block.BlockType.Barrier && nextBlock == Block.BlockType.Barrier)
                {
                    CurrentMovementDelta = 1;
                    Capable = true;
                    Pos = new(Pos.x, Jump0To0(JumpProgress) + 3, playerProgress + JumpProgress * dir);
                }
                else if(currentBlock == Block.BlockType.Barrier && nextBlock != Block.BlockType.Barrier)
                {
                    CurrentMovementDelta = 1;
                    Capable = true;
                    Pos = new(Pos.x, Jump3To0(JumpProgress), playerProgress + JumpProgress * dir);
                }
                else
                {
                    if(Movement < 3)
                    {
                        CurrentMovementDelta = Movement;
                        Capable = false;
                        Pos = new(Pos.x, JumpInPlace(JumpProgress, Movement), playerProgress);
                    }
                    else
                    {
                        CurrentMovementDelta = 3;
                        Capable = true;
                        Pos = new(Pos.x, Jump0To3(JumpProgress), playerProgress + JumpProgress * dir);
                    }
                }
            }
        }

        void OnBlockChange()
        {
            if (Movement > 0)
                return;
            int playerProgress = PlayerIndex == 1 ? GameLogic.Instance.Player1Progress : GameLogic.Instance.Player2Progress;
            // 计算脚底方块
            var block = GameLogic.Instance.PlatformBlocks[playerProgress].GetComponent<Block>().Type;
            if (block == Block.BlockType.Barrier)
            {
                Pos = new(Pos.x, 3, playerProgress);
            }
            else if (block == Block.BlockType.Empty)
            {
                if (PlayerIndex == 1)
                {
                    GameLogic.Instance.Player1Progress--;
                }
                else
                {
                    GameLogic.Instance.Player2Progress--;
                }
                OnBlockChange();
            }
            else
            {
                Pos = new(Pos.x, 0, playerProgress);
            }
        }

        void CalcMove()
        {
            if(!EnableMove) 
                return;
            int playerProgress = PlayerIndex == 1 ? GameLogic.Instance.Player1Progress : GameLogic.Instance.Player2Progress;
            this.transform.position = Vector3.MoveTowards(this.transform.position, Pos, Time.deltaTime * MoveSpeed);
        }

        /// <summary>
        /// 开跳
        /// </summary>
        /// <param name="target">移动力</param>
        public void Jump(int movement)
        {
            Movement = movement;
            EnableJump = true;
            JumpProgress = 0;
            TimeProgress = 0;
            GameLogic.Instance.StateBlock++;
        }

        /// <summary>
        /// 0高度跳向0高度
        /// </summary>
        /// <param name="x">x轴</param>
        /// <returns>Y轴</returns>
        float Jump0To0(float x)
        {
            return -2 * x * x + 2 * x;
        }

        /// <summary>
        /// 0高度跳向3高度
        /// </summary>
        /// <param name="x">x轴</param>
        /// <returns>Y轴</returns>
        float Jump0To3(float x)
        {
            return -7 * x * x + 10 * x;
        }

        /// <summary>
        /// 3高度跳向0高度
        /// </summary>
        /// <param name="x">x轴</param>
        /// <returns>Y轴</returns>
        float Jump3To0(float x)
        {
            return -7 * x * x + 4 * x + 3;
        }

        /// <summary>
        /// 原地跳
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="time">高度</param>
        /// <returns>Y轴</returns>
        float JumpInPlace(float time, int h)
        {
            return -(h * 4 + 2) * time * time + (h * 4 + 2) * time;
        }
    }
}
