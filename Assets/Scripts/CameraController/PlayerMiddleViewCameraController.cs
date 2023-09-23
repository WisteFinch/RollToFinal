using Cinemachine;
using UnityEngine;

namespace RollToFinal
{
    public class PlayerMiddleViewCameraController : MonoBehaviour
    {
        /// <summary>
        /// 基准点
        /// </summary>
        private GameObject RefPoint;

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
            RefPoint = new GameObject("PlayerMiddleViewCameraRefPoint");
            camera.m_Follow = RefPoint.transform;
            camera.m_LookAt = RefPoint.transform;
        }

        private void Update()
        {
            RefPoint.transform.position = (Player1.position + Player2.position) / 2;
            CameraAttribute.m_Heading.m_Bias = Vector3.Angle(Player2.position - Player1.position, Vector3.forward) - 90;
            CameraAttribute.m_FollowOffset.y = Vector3.Distance(Player1.position, Player2.position) * 0.87f + 5f;
        }
    }
}