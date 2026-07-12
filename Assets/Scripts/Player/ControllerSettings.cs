using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerSettings : MonoBehaviour
{
    public bool isController;
    [Header("Current Input in Use")]
    public string horizontalInput;
    public string verticalInput,fireInput,mouseX,mouseY,jumpButton,boostButton,meleeButton,focusAimButton;

    [Header("PC Input")]
    public string pcHorizontalInput;
    public string pcVerticalInput,pcFireInput,pcMouseX,pcMouseY,pcJumpButton,pcBoostButton,pcMeleeButton,pcFocusAimButton;

    [Header("Xbox Input")]
    public string xboxHorizontalInput;
    public string xboxVerticalInput,xboxFireInput,xboxMouseX,xboxMouseY,xboxJumpButton,xboxBoostButton,xboxMeleeButton,xboxFocusAimButton;
    // Start is called before the first frame update
    void Awake()
    {
        if(isController)
        {
            horizontalInput = xboxHorizontalInput;
            verticalInput = xboxVerticalInput;
            fireInput = xboxFireInput;
            mouseX = xboxMouseX;
            mouseY = xboxMouseY;
            jumpButton = xboxJumpButton;
            boostButton = xboxBoostButton;
            meleeButton = xboxMeleeButton;
            focusAimButton = xboxFocusAimButton;
        }
        else
        {
            horizontalInput = pcHorizontalInput;
            verticalInput = pcVerticalInput;
            fireInput = pcFireInput;
            mouseX = pcMouseX;
            mouseY = pcMouseY;
            jumpButton = pcJumpButton;
            boostButton = pcBoostButton;
            meleeButton = pcMeleeButton;
            focusAimButton = pcFocusAimButton;
        }
    }
}
