using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RollToFinal
{
    public class InputManager : MonoBehaviour
    {
        /// <summary>
        /// 玩家输入
        /// </summary>
        public struct PlayerInput
        {
            /// <summary>
            /// 视角轴
            /// </summary>
            public Vector2 LookAxis;
            /// <summary>
            /// 按下幸运按钮
            /// </summary>
            public bool LuckyKeyDown;
            /// <summary>
            /// 按下厄运按钮
            /// </summary>
            public bool CalamityKeyDown;
            private int v1;
            private int v2;
            private int v3;
        }

        /// <summary>
        /// 玩家1输入
        /// </summary>
        public PlayerInput player1 = new();

        /// <summary>
        /// 玩家2输入
        /// </summary>
        public PlayerInput player2 = new();

        /// <summary>
        /// 鼠标位置
        /// </summary>
        public Vector2 MousePosition = Vector2.zero;

        private void Update()
        {
            // 记录鼠标位置
            this.MousePosition = Mouse.current.position.ReadValue();
        }

        private void LateUpdate()
        {
            // 重置按键状态
            this.player1 = new();
            this.player2 = new();
        }

        ///// <summary>
        ///// 激活跳跃键
        ///// </summary>
        ///// <param name="context">事件信息</param>
        //public void OnPlayer1(InputAction.CallbackContext context)
        //{
        //    if (context.phase == InputActionPhase.Started)
        //        this.JumpKeyDown = true;
        //    if (context.phase == InputActionPhase.Canceled)
        //        this.JumpKeyUp = true;
        //    this.JumpKeyPressed = context.phase == InputActionPhase.Performed;
        //}

        ///// <summary>
        ///// 激活移动键
        ///// </summary>
        ///// <param name="context">事件信息</param>
        //public void OnMove(InputAction.CallbackContext context)
        //{
        //    this.MoveAxis = context.ReadValue<Vector2>();
        //}

        ///// <summary>
        ///// 激活视角键
        ///// </summary>
        ///// <param name="context">事件信息</param>
        //public void OnLook(InputAction.CallbackContext context)
        //{
        //    this.LookAxis = context.ReadValue<Vector2>();
        //}

        ///// <summary>
        ///// 激活交互键
        ///// </summary>
        ///// <param name="context">事件信息</param>
        //public void OnInteract(InputAction.CallbackContext context)
        //{
        //    if (context.phase == InputActionPhase.Started)
        //        this.InteractKeyDown = true;
        //}

        ///// <summary>
        ///// 激活冲刺键
        ///// </summary>
        ///// <param name="context">事件信息</param>
        //public void OnDash(InputAction.CallbackContext context)
        //{
        //    if (context.phase == InputActionPhase.Started)
        //        this.DashKeyDown = true;
        //}

        ///// <summary>
        ///// 激活开火键
        ///// </summary>
        ///// <param name="context">事件信息</param>
        //public void OnFire(InputAction.CallbackContext context)
        //{
        //    if (context.phase == InputActionPhase.Started)
        //        this.FireKeyDown = true;
        //}

        ///// <summary>
        ///// 激活返回键
        ///// </summary>
        ///// <param name="context">事件信息</param>
        //public void OnEscape(InputAction.CallbackContext context)
        //{
        //    if (context.phase == InputActionPhase.Started)
        //        this.EscapeKeyDown = true;
        //}
    }
}

