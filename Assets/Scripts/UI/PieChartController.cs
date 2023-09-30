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
        public float ChangeRate = 0.01f;

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
            CurrentDivides.Clear();
            Divides.ForEach(divide => CurrentDivides.Add(divide));
            CleanRubbishItem(ref CurrentDivides);
            PieChart.Divides = CurrentDivides;
            ChangeFinished = true;
        }

        private void Awake()
        {
            CurrentDivides.Clear();
            Divides.ForEach(divide => CurrentDivides.Add(divide));
            CleanRubbishItem( ref CurrentDivides);
            PieChart.Divides = CurrentDivides;
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
            if (ChangeRate == 0f)
            {
                CurrentDivides.Clear();
                Divides.ForEach(divide => CurrentDivides.Add(divide));
                ChangeFinished = true;
            }
            else
            {
                CalcRateCount(ref CurrentDivides, ref CurrentDividesRatioCount);
                bool flag = true;
                int min_count = Divides.Count > CurrentDivides.Count ? CurrentDivides.Count : Divides.Count;
                float ratio_average = (CurrentDividesRatioCount + DividesRatioCount) / 2;
                for (int i = 0; i < min_count; i++)
                {
                    if (!Divides[i].Equals(CurrentDivides[i]))
                    {
                        var item = Divides[i];
                        item.ratio = Mathf.MoveTowards(CurrentDivides[i].ratio, item.ratio, ratio_average * ChangeRate);
                        CurrentDivides[i] = item;
                        flag = false;
                    }
                }
                if (Divides.Count > CurrentDivides.Count)
                {
                    flag = false;
                    for (int i = CurrentDivides.Count; i < Divides.Count; i++)
                    {
                        var item = Divides[i];
                        item.ratio = Mathf.MoveTowards(0f, item.ratio, ratio_average * ChangeRate);
                        CurrentDivides.Add(item);
                    }
                }
                if (Divides.Count < CurrentDivides.Count)
                {
                    flag = false;
                    for (int i = CurrentDivides.Count - 1; i >= Divides.Count; i--)
                    {
                        var item = CurrentDivides[i];
                        item.ratio = Mathf.MoveTowards(item.ratio, 0, ratio_average * ChangeRate);
                        CurrentDivides[i] = item;
                        if (CurrentDivides[i].ratio == 0f)
                            CurrentDivides.RemoveAt(i);
                    }
                }
                if (flag)
                {
                    ChangeFinished = true;
                    CleanRubbishItem(ref CurrentDivides);
                }
            }
            PieChart.ReDraw();
        }


        private void OnValidate()
        {
            OnValueChanged();
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

        /// <summary>
        /// 值改变
        /// </summary>
        public void OnValueChanged()
        {
            ChangeFinished = false;
            CalcRateCount(ref Divides, ref DividesRatioCount);
        }

        /// <summary>
        /// 清除垃圾项
        /// </summary>
        /// <param name="divides">划分</param>
        public void CleanRubbishItem(ref List<DrawPieChart.DivideItem> divides)
        {
            //for(int i = divides.Count - 1; i >= 0; i--)
            //{
            //    if (divides[i].ratio == 0f)
            //        divides.RemoveAt(i);
            //}
        }
    }
}