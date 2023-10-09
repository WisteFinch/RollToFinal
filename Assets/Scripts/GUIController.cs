using System.Collections.Generic;
using UnityEngine;
using static RollToFinal.DrawPieChart;

namespace RollToFinal
{
    public class GUIController : MonoBehaviour
    {

        public DrawPieChart Pie;

        public float TargetRotation = 0;

        public float CurrentRotation = 0;

        public int RandomStart = 5;

        public int RandomEnd = 10;

        public bool EnableRoll = false;

        public void OnStateChange()
        {
            GameLogic.Instance.OnGUIStateChange();
        }

        public void SetDivides(List<DivideItem> Divides)
        {
            Pie.Divides = Divides;
            Pie.Rotate = 90;
        }

        public void Roll(int index)
        {
            float sum_ratio = 0f;
            foreach (var v in Pie.Divides)
                sum_ratio += v.ratio;
            float start = 0f, end = 0f;
            for(int i = 0; i < index; i++)
            {
                start += Pie.Divides[i].ratio;
            }
            end = start + Pie.Divides[index].ratio;

            TargetRotation = Random.Range(RandomStart, RandomEnd) * 360f + Random.Range(start / sum_ratio, end / sum_ratio) * 360f + 90;
            CurrentRotation = Pie.Rotate;
            EnableRoll = true;
        }

        private void Update()
        {
            if(EnableRoll)
            {
                CurrentRotation = Mathf.MoveTowards(CurrentRotation, TargetRotation, Time.deltaTime * 500);
                Pie.Rotate = CurrentRotation;
                if(CurrentRotation == TargetRotation)
                {
                    EnableRoll = false;
                    Invoke(nameof(OnStateChange), 2);
                }
            }
        }
    }
}

