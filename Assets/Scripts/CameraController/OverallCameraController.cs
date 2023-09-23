using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RollToFinal
{
    public class OverallCameraController : MonoBehaviour
    {
        /// <summary>
        /// 基准点
        /// </summary>
        private GameObject RefPoint;

        /// <summary>
        /// 游戏逻辑
        /// </summary>
        public GameLogic Logic;

        /// <summary>
        /// 玩家1位置
        /// </summary>
        public Transform Player1;

        /// <summary>
        /// 玩家2位置
        /// </summary>
        public Transform Player2;

        /// <summary>
        /// 摄像机环绕属性
        /// </summary>
        private CinemachineOrbitalTransposer CameraAttribute;

        private void Start()
        {
            CinemachineVirtualCamera camera = GetComponent<CinemachineVirtualCamera>();
            CameraAttribute = camera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
            RefPoint = new GameObject("OverallCameraRefPoint");
            RefPoint.transform.position = new Vector3((Player1.position.x + Player2.position.x) / 2, 0, Logic.Length / 2);
            camera.m_Follow = RefPoint.transform;
            camera.m_LookAt = RefPoint.transform;
            CameraAttribute.m_FollowOffset.y = Logic.Length * 0.87f + 5f;
        }
    }
}
