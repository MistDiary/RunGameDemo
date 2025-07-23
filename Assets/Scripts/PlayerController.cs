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
    [SerializeField] private PlayerCharacter playerCharacter;  // 玩家角色引用

    void Awake()
    {
        // 自动获取玩家角色组件（如果未在Inspector中指定）
        if (playerCharacter == null)
        {
            playerCharacter = GetComponent<PlayerCharacter>();
        }
    }

    void Update()
    {
        // 如果玩家为空或已死亡，则不处理输入
        if (playerCharacter == null || !playerCharacter.isAlive)
        {
            return;
        }

        // 持续移动（始终向前）
        playerCharacter.Move();

        // 空格键跳跃
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerCharacter.Jump();
           
        }

        // J键切换颜色
        if (Input.GetKeyDown(KeyCode.J))
        {
            playerCharacter.ToggleColor();
        }
    }
}
