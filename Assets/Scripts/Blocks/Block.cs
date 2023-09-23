using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RollToFinal
{
    public class Block : MonoBehaviour
    {
        [HideInInspector]
        public enum BlockType{
            Normal
        };

        public BlockType Type;
    }
}