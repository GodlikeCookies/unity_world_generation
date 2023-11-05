using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Получение необходимых объектов со сцены при ее старте
public class DebugStarter : MonoBehaviour
{
    void Start()
    {
        ObjectsContainer.character = GameObject.Find("Character").transform;
    }
}
