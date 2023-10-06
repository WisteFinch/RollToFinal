using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RollToFinal
{
    public class Block : MonoBehaviour
    {
        [HideInInspector]
        public enum BlockType{
            Normal,
            Barrier,
            Raffle,
            Trap,
            Empty,
            EndLine,
            Start
        };

        public BlockType Type;

        public List<object> Data = new();

        public int Index;
    }
}