using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelScrolling : MonoBehaviour {

    enum ButtonModes {
        FORWARD,
        BACKWARD,
        SCROLLBAR
    }
    [SerializeField]
    private ButtonModes mode;
    [SerializeField]
    private LevelSelectController selector;

    public void MoveLevel()
    {
        int value = 0;
        switch (mode)
        {
            case ButtonModes.FORWARD:
                value = selector.currentNode + 1;
                break;
            case ButtonModes.BACKWARD:
                value = selector.currentNode - 1;
                break;
            case ButtonModes.SCROLLBAR:
                value = (int)GetComponent<Slider>().value;
                break;
        }
        if (value >= 0 && value < selector.nodes.Length)
            selector.SetLevel(selector.nodes[value]);
    }
}