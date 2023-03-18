using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Mecanics;

public class ChangeMap : MonoBehaviour
{
    public MapLevelGeneratorManager mapGenerator;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            mapGenerator.SetLevel(mapGenerator.CurrentLevelIndex + 1);
        }
    }
}
