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
            case 0:
                //1P keyboard
                RollToFinal.DataSystem.Instance.EnableAI = true;
                RollToFinal.DataSystem.Instance.UseJoyStick = false;
                break;
            case 1:
                //1P joystick
                RollToFinal.DataSystem.Instance.EnableAI = true;
                RollToFinal.DataSystem.Instance.UseJoyStick = true;
                break;
            case 2:
                //2P keyboard
                RollToFinal.DataSystem.Instance.EnableAI = false;
                RollToFinal.DataSystem.Instance.UseJoyStick = false;
                break;
            case 3:
                //2P key+joystick
                RollToFinal.DataSystem.Instance.EnableAI = false;
                RollToFinal.DataSystem.Instance.UseJoyStick = true;
                break;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
