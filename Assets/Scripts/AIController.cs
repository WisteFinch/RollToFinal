using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RollToFinal
{
    public class AIController : MonoBehaviour
    {
        void Start()
        {
            if (Transfer.Instance.EnableAI)
                InvokeRepeating(nameof(Check), 0.5f, 0.5f);
        }

        void Check()
        {
            if(GameLogic.Instance.CurrentPlayer == 2)
            {
                if(GameLogic.Instance.CurrentGameState == GameLogic.GameState.PlayerIdle)
                {
                    if (DataSystem.Instance.GetData("Player2SpecialRollCount") > 0)
                        GameLogic.Instance.PlayerSpecialRoll(2);
                    else
                        GameLogic.Instance.PlayerRoll(2);
                }
            }
        }
    }
}

