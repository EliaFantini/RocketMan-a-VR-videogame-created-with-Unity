using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Challenge3press : MonoBehaviour
{
    public ButtonLamp buttonLamp;

    public press1 button1;
    public Button3 button2;
    public Button3 button3;
    int b1press = 0;
    int b2press = 0;
    int b3press = 0;
    // Start is called before the first frame update
    void Start()
    {
        buttonLamp.on = true;
        buttonLamp.lightColor = ButtonLamp.eColor.Green;
    }

    void Update() {
        if (button1.isPressed == true) {
            buttonLamp.lightColor = ButtonLamp.eColor.Red;
        }
    }

    // Update is called once per frame
    void check_button_status() {
        buttonLamp.on = true;
        if(b1press + b2press + b3press == 3) {
            buttonLamp.lightColor = ButtonLamp.eColor.Green;
        }

        if(b1press + b2press + b3press == 2) {
            buttonLamp.lightColor = ButtonLamp.eColor.Yellow;
        }

         if(b1press + b2press + b3press == 1) {
            buttonLamp.lightColor = ButtonLamp.eColor.Red;
        }
    }

    public void button1Press(){
        b1press = 1;
        check_button_status();
    }

    public void button1Release(){
         b1press = 0;
         check_button_status();
    }

    public void button2Press(){
         b2press = 1;
         check_button_status();
    }

    public void button2Release(){
         b2press = 0;
         check_button_status();
    }
    public void button3Press(){
         b3press = 1;
         check_button_status();
    }

    public void button3Release(){
         b3press = 0;
         check_button_status();
    }
}
