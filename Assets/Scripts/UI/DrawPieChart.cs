using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace RollToFinal
{

    [ExecuteInEditMode]
    public class DrawPieChart: MaskableGraphic
    {
        /// <summary>
        /// 形状枚举
        /// </summary>
        public enum ShapeType
        {
            /// <summary>
            /// 圆环
            /// </summary>
            Annulus,
            /// <summary>
            /// 园
            /// </summary>
            Circle,
        }

        /// <summary>
        /// 划分项
        /// </summary>
        [Serializable]
        public struct DivideItem
        {
            /// <summary>
            /// 颜色
            /// </summary>
            public Color color;
            /// <summary>
            /// 比例
            /// </summary>
            [Range(0, 10)]
            public float ratio;
            /// <summary>
            /// 标题
            /// </summary>
            public string title;
        }

        /// <summary>
        /// 划分项(运行时)
        /// </summary>
        [SerializeField]
        public struct RuntimeDivideItem
        {
            /// <summary>
            /// 颜色
            /// </summary>
            public Color color;
            /// <summary>
            /// 比例
            /// </summary>
            public float ratio;
            /// <summary>
            /// 标题
            /// </summary>
            public string title;
            /// <summary>
            /// 绘制终止三角形数
            /// </summary>
            public int terminateCount;
            /// <summary>
            /// 中线
            /// </summary>
            public int median;
        }

        /// <summary>
        /// 划分
        /// </summary>
        public List<DivideItem> Divides = new();

        /// <summary>
        /// 文字半径偏移
        /// </summary>
        public float TextRadiusOffset = 10f;

        /// <summary>
        /// 显示划分占比
        /// </summary>
        public bool ShowDividesRatio = false;

        /// <summary>
        /// 划分占比提示颜色
        /// </summary>
        public Color DivideRatioColor = Color.white;

        /// <summary>
        /// 文字样式
        /// </summary>
        public GUIStyle TextStyle;

        /// <summary>
        /// 精灵图片
        /// </summary>
        [SerializeField]
        Sprite m_sprite;

        /// <summary>
        /// 形状
        /// </summary>
        public ShapeType Type;

        /// <summary>
        /// 圆环内径
        /// </summary>
        public float InnerRadius = 10;

        /// <summary>
        /// 圆环外径
        /// </summary>
        public float OuterRadius = 20;

        /// <summary>
        /// 填充值
        /// </summary>
        [Range(0, 1)]
        [SerializeField]
        float m_fillAmount;

        /// <summary>
        /// 片数
        /// </summary>
        [Range(0, 720)] 
        public int Segments = 360;

        /// <summary>
        /// 填充起点
        /// </summary>
        [SerializeField]
        Image.Origin360 m_originType;

        /// <summary>
        /// 顺时针
        /// </summary>
        [SerializeField]
        bool m_isClockwise = true;

        public override Texture mainTexture => m_sprite == null ? s_WhiteTexture : m_sprite.texture;

        /// <summary>
        /// 根据m_originType设置相关弧度
        /// </summary>
        float m_originRadian = -1;

        public float FillAmount
        {
            get => m_fillAmount;
            set
            {
                m_fillAmount = value;
                SetVerticesDirty();
            }
        }

        public Sprite Image
        {
            get => m_sprite;
            set
            {
                if (m_sprite == value)
                    return;
                m_sprite = value;
                SetVerticesDirty();
                SetMaterialDirty();
            }
        }

        public Image.Origin360 OriginType
        {
            get => m_originType;
            set
            {
                if (m_originType == value)
                    return;
                m_originType = value;
                SetOriginRadian();
                SetVerticesDirty();
            }
        }

        public bool IsClockwise
        {
            get => m_isClockwise;
            set
            {
                if (m_isClockwise != value)
                {
                    m_isClockwise = value;
                    SetVerticesDirty();
                }
            }
        }

        UIVertex[] m_vertexes = new UIVertex[4];
        Vector2[] m_uvs = new Vector2[4];
        Vector2[] m_positions = new Vector2[4];

        protected override void Start()
        {
            if (m_originRadian == -1)
                SetOriginRadian();

            m_uvs[0] = new Vector2(0, 1);
            m_uvs[1] = new Vector2(1, 1);
            m_uvs[2] = new Vector2(1, 0);
            m_uvs[3] = new Vector2(0, 0);
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            //m_fillAmount == 0，什么也不绘制
            if (m_fillAmount == 0) return;

#if UNITY_EDITOR
            SetOriginRadian();
#endif

            //每个面片的角度
            float degrees = 360f / Segments;
            //需要绘制的面片数量
            int count = (int)(Segments * m_fillAmount);

            float cos = Mathf.Cos(m_originRadian);
            float sin = Mathf.Sin(m_originRadian);

            //计算外环起点，例如m_originRadian = 0，x = -outerRadius，y = 0，所以起点是Left（九点钟方向）
            float x = -OuterRadius * cos;
            float y = OuterRadius * sin;
            Vector2 originOuter = new Vector2(x, y);

            //计算内环起点
            x = -InnerRadius * cos;
            y = InnerRadius * sin;
            Vector2 originInner = new Vector2(x, y);

            bool hasDivides = false;
            List<RuntimeDivideItem> divides = new();
            float sum_ratio = 0f;
            foreach (var v in Divides)
                sum_ratio += v.ratio;
            float cur_ratio = 0f;
            float last_ratio = 0f;
            foreach (var v in Divides)
            {
                if(v.ratio > 0f)
                {
                    hasDivides = true;
                    cur_ratio += v.ratio;
                    RuntimeDivideItem item = new()
                    {
                        terminateCount = (int)(cur_ratio / sum_ratio * count),
                        title = v.title,
                        ratio = v.ratio,
                        color = v.color,
                        median = (int)((cur_ratio + last_ratio) / 2f / sum_ratio * count)
                    };
                    divides.Add(item);
                    last_ratio = cur_ratio;
                }
            }
            int current_divide = 0;


            for (int i = 1; i <= count; i++)
            {
                //m_positions[0] 当前面片的外环起点
                m_positions[0] = originOuter;

                //当前面片的弧度 + 起始弧度 = 终止弧度
                float endRadian = i * degrees * Mathf.Deg2Rad * (IsClockwise ? 1 : -1) + m_originRadian;
                cos = Mathf.Cos(endRadian);
                sin = Mathf.Sin(endRadian);

                //m_positions[1] 当前面片的外环终点
                m_positions[1] = new Vector2(-OuterRadius * cos, OuterRadius * sin);

                //m_positions[2] 当前面片的内环终点
                //m_positions[3] 当前面片的内环起点
                if (Type == ShapeType.Annulus)
                {
                    m_positions[2] = new Vector2(-InnerRadius * cos, InnerRadius * sin);
                    m_positions[3] = originInner;
                }
                else
                {
                    m_positions[2] = Vector2.zero;
                    m_positions[3] = Vector2.zero;
                }

                // 设置顶点的颜色坐标以及uv
                if (hasDivides)
                {
                    if (i > divides[current_divide].terminateCount)
                        current_divide++;
                    for (int j = 0; j < 4; j++)
                    {
                        m_vertexes[j].color = divides[current_divide].color;
                        m_vertexes[j].position = m_positions[j];
                        m_vertexes[j].uv0 = m_uvs[j];
                    }
                }
                else
                {
                    for (int j = 0; j < 4; j++)
                    {
                        m_vertexes[j].color = color;
                        m_vertexes[j].position = m_positions[j];
                        m_vertexes[j].uv0 = m_uvs[j];
                    }
                }

                //当前顶点数量
                int vertCount = vh.currentVertCount;

                //如果是圆只需要添加三个顶点，创建一个三角面
                vh.AddVert(m_vertexes[0]);
                vh.AddVert(m_vertexes[1]);
                vh.AddVert(m_vertexes[2]);
                //参数即三角面的顶点绘制顺序
                vh.AddTriangle(vertCount, vertCount + 2, vertCount + 1);

                // 如果是圆环就需要添加第四个顶点，并再创建一个三角面
                if (Type == ShapeType.Annulus)
                {
                    vh.AddVert(m_vertexes[3]);
                    vh.AddTriangle(vertCount, vertCount + 3, vertCount + 2);
                }

                //当前面片的终点就是下个面片的起点
                originOuter = m_positions[1];
                originInner = m_positions[2];
            }
        }

        //m_originType改变的时候需要重新设置m_originRadian
        void SetOriginRadian()
        {
            switch (m_originType)
            {
                case UnityEngine.UI.Image.Origin360.Left:
                    m_originRadian = 0 * Mathf.Deg2Rad;
                    break;
                case UnityEngine.UI.Image.Origin360.Top:
                    m_originRadian = 90 * Mathf.Deg2Rad;
                    break;
                case UnityEngine.UI.Image.Origin360.Right:
                    m_originRadian = 180 * Mathf.Deg2Rad;
                    break;
                case UnityEngine.UI.Image.Origin360.Bottom:
                    m_originRadian = 270 * Mathf.Deg2Rad;
                    break;
            }
        }

        private void OnGUI()
        {
            Vector3 result = transform.localPosition;
            Vector3 realPosition = GetScreenPosition(transform, ref result);

            //每个面片的角度
            float degrees = 360f / Segments;
            //需要绘制的面片数量
            int count = (int)(Segments * m_fillAmount);

            float cos, sin;

            bool hasDivides = false;
            List<RuntimeDivideItem> divides = new();
            float sum_ratio = 0f;
            foreach (var v in Divides)
                sum_ratio += v.ratio;
            float cur_ratio = 0f;
            float last_ratio = 0f;
            foreach (var v in Divides)
            {
                if(v.ratio > 0f)
                {
                    hasDivides = true;
                    cur_ratio += v.ratio;
                    RuntimeDivideItem item = new()
                    {
                        terminateCount = (int)(cur_ratio / sum_ratio * count),
                        title = v.title,
                        ratio = v.ratio,
                        color = v.color,
                        median = (int)((cur_ratio + last_ratio) / 2f / sum_ratio * count)
                    };
                    divides.Add(item);
                    last_ratio = cur_ratio;
                }
            }
            int current_divide = 0;


            for (int i = 1; i <= count; i++)
            {
                //当前面片的弧度 + 起始弧度 = 终止弧度
                float endRadian = i * degrees * Mathf.Deg2Rad * (IsClockwise ? 1 : -1) + m_originRadian;
                cos = Mathf.Cos(endRadian);
                sin = Mathf.Sin(endRadian);

                if (hasDivides)
                {
                    if (i > divides[current_divide].terminateCount)
                        current_divide++;
                    if(i == divides[current_divide].median)
                    {
                        Vector2 dir = new(-OuterRadius * cos, OuterRadius * sin);
                        dir.Normalize();
                        if(ShowDividesRatio)
                        {
                            GUI.Label(new Rect(LocalToScreen(realPosition, dir * (OuterRadius + TextRadiusOffset)), new(0, 0)), $"{divides[current_divide].title}<color=#{DivideRatioColor.ToHexString()}>({(int)(divides[current_divide].ratio / sum_ratio * 100)}%)</color>", TextStyle);
                        }
                        else
                        {

                            GUI.Label(new Rect(LocalToScreen(realPosition, dir * (OuterRadius + TextRadiusOffset)), new(0, 0)), divides[current_divide].title, TextStyle);
                        }
                    }
                }
            }
        }

        private Vector2 GetScreenPosition(Transform trans, ref Vector3 result)
        {
            if (null != trans.parent && null != trans.parent.parent)
            {
                result += trans.parent.localPosition;
                GetScreenPosition(trans.parent, ref result);
            }
            if (null != trans.parent && null == trans.parent.parent)
                return result;
            return result;
        }

        //本地坐标转化屏幕坐标绘制GUI文字
        private Vector2 LocalToScreen(Vector2 parentPos, Vector2 localPosition)
        {
            Vector2 pos = localPosition + parentPos;
            float xValue, yValue = 0;
            if (pos.x > 0)
                xValue = pos.x + Screen.width / 2.0f;
            else
                xValue = Screen.width / 2.0f - Mathf.Abs(pos.x);
            if (pos.y > 0)
                yValue = Screen.height / 2.0f - pos.y;
            else
                yValue = Screen.height / 2.0f + Mathf.Abs(pos.y);
            return new Vector2(xValue, yValue);
        }

        /// <summary>
        /// 重新绘制
        /// </summary>
        public void ReDraw()
        {
            UpdateGeometry();
        }
    }
}