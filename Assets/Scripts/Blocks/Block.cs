using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

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

        private void Start()
        {
            
        }

        private void Update()
        {
        }

        public void Entry(float delay = 0)
        {
            Invoke(nameof(PlayEntry), delay);
        }

        public void Escape(float delay = 0)
        {
            Invoke(nameof(PlayEscape), delay);
        }

        public void PlayHide()
        {
            gameObject.GetComponent<Animator>().Play("BlockHide");
        }

        private void PlayEntry()
        {
            gameObject.GetComponent<Animator>().Play("BlockEntry");
        }

        private void PlayEscape()
        {
            this.AddComponent<Rigidbody>();
            gameObject.GetComponent<Animator>().Play("BlockEscape");
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}