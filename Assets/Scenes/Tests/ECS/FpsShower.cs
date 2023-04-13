using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsShower : MonoBehaviour
{
    public UnityEngine.UI.Text text;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        InvokeRepeating(nameof(Show), 0.3f, 0.3f);
    }

    // Update is called once per frame
    void Show()
    {
        text.text = "fps: " + (1f / Time.smoothDeltaTime).ToString("0");
    }
}
