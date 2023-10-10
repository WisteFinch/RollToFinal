using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{

    public int gamemode = 0;
    public void startGame() {
        switch (gamemode)
        {
            //case 0:
            //    //1P keyboard
            //    RollToFinal.Transfer.Instance.EnableAI = true;
            //    RollToFinal.Transfer.Instance.UseJoyStick = false;
            //    break;
            //case 1:
            //    //1P joystick
            //    RollToFinal.Transfer.Instance.EnableAI = true;
            //    RollToFinal.Transfer.Instance.UseJoyStick = true;
            //    break;
            //case 2:
            //    //2P keyboard
            //    RollToFinal.Transfer.Instance.EnableAI = false;
            //    RollToFinal.Transfer.Instance.UseJoyStick = false;
            //    break;
            //case 3:
            //    //2P key+joystick
            //    RollToFinal.Transfer.Instance.EnableAI = false;
            //    RollToFinal.Transfer.Instance.UseJoyStick = true;
            //    break

            case 0:
                //1P keyboard
                RollToFinal.Transfer.Instance.EnableAI = true;
                RollToFinal.Transfer.Instance.UseJoyStick = false;
                break;
            case 1:
                //2P keyboard
                RollToFinal.Transfer.Instance.EnableAI = false;
                RollToFinal.Transfer.Instance.UseJoyStick = false;
                break;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
