/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerCharacter playerCharacter;
    void Awake()
    {
        playerCharacter = GetComponent<PlayerCharacter>();
    }
    void Update()
    {
        playerCharacter.Move();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerCharacter.Jump();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            playerCharacter.ColorChange();
        }
    }

}
*/




using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerCharacter playerCharacter;  // ��ҽ�ɫ����

    void Awake()
    {
        // �Զ���ȡ��ҽ�ɫ��������δ��Inspector��ָ����
        if (playerCharacter == null)
        {
            playerCharacter = GetComponent<PlayerCharacter>();
        }
    }

    void Update()
    {
        // ������Ϊ�ջ����������򲻴�������
        if (playerCharacter == null || !playerCharacter.isAlive)
        {
            return;
        }

        // �����ƶ���ʼ����ǰ��
        playerCharacter.Move();

        // �ո����Ծ
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerCharacter.Jump();
           
        }

        // J���л���ɫ
        if (Input.GetKeyDown(KeyCode.J))
        {
            playerCharacter.ToggleColor();
        }
    }
}
