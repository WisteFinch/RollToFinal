using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RollToFinal
{
    public class PieChartController : MonoBehaviour
    {
        /// <summary>
        /// 饼图引用
        /// </summary>
        public DrawPieChart PieChart;

        /// <summary>
        /// 变化率
        /// </summary>
        public float ChangeRate = 0f;

        /// <summary>
        /// 划分
        /// </summary>
        public List<DrawPieChart.DivideItem> Divides = new();

        /// <summary>
        /// 当前划分
        /// </summary>
        private List<DrawPieChart.DivideItem> CurrentDivides = new();

        /// <summary>
        /// 变化完成
        /// </summary>
        private bool ChangeFinished;

        /// <summary>
        /// 划分概率和
        /// </summary>
        private float DividesRatioCount;

        /// <summary>
        /// 当前划分概率和
        /// </summary>
        private float CurrentDividesRatioCount;

        private void Start()
        {
            CurrentDivides = Divides;
            ChangeFinished = true;
        }

        private void Awake()
        {
            CurrentDivides = Divides;
            ChangeFinished = true;
        }

        private void Update()
        {
            UpdateValue();
        }

        /// <summary>
        /// 更新数值
        /// </summary>
        private void UpdateValue()
        {
            if (ChangeFinished)
                return;
            if(ChangeRate == 0f)
            {
                CurrentDivides = Divides;
                PieChart.Divides = CurrentDivides;
            }
            else
            {

            }

            PieChart.ReDraw();
        }

        private void OnValidate()
        {
            ChangeFinished = false;
            CalcRateCount(ref Divides, ref DividesRatioCount);
            CalcRateCount(ref CurrentDivides, ref CurrentDividesRatioCount);
        }

        /// <summary>
        /// 计算概率和
        /// </summary>
        /// <param name="divides">划分引用</param>
        /// <param name="count">和引用</param>
        private void CalcRateCount(ref List<DrawPieChart.DivideItem> divides, ref float count)
        {
            count = 0f;
            foreach(var i in divides)
            {
                count += i.ratio;
            }
        }
    }
}