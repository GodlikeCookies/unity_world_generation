using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��������� ����������� �������� �� ����� ��� �� ������
public class DebugStarter : MonoBehaviour
{
    void Start()
    {
        ObjectsContainer.character = GameObject.Find("Character").transform;
    }
}
