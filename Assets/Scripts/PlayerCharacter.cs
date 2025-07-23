/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{

    public enum StateColor
    {
        Red,
        Yellow,
     
    }
    StateColor stateColor = StateColor.Red;
    Rigidbody rigidbody;
    Animator animator;
    Renderer[] render;
    public float speed = 5f;

    public float jumpForce = 4f;
    public float doubleJumpForce = 6f;
    int jumpCount = 0;
    bool onGround;


    public bool isAlive;
    Collision collisionCut;
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        render = GetComponentsInChildren<Renderer>();
        isAlive = true;
    }

    void FixedUpdate()
    {
        if (collisionCut != null)
        {
            if (collisionCut.gameObject.CompareTag("Red"))
            {
                if (stateColor !=StateColor.Red)
                {
                    Die();
                }

            }
            else if (collisionCut.gameObject.CompareTag("Yellow"))
            {
                if (stateColor != StateColor.Yellow)
                {
                    Die();
                }
            }
            else if (collisionCut.gameObject.CompareTag("Black"))
            {
                Die();
            }
        }
        onGround = GroundCheck();
        animator.SetBool("OnGround", onGround);
    }
    public bool GroundCheck()
    {

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.4f);
        for (int i = 0;i < hitColliders.Length;i++)
        {
            if (hitColliders[i].gameObject != gameObject)
            {
                return true;
            }

        }
        return false;
            
       
    }

    public void Move()
    {
        var vel = rigidbody.velocity;
        vel.z = (Vector3.forward * speed).z;
        rigidbody.velocity = vel;
    }

    public void Jump()
    {
        if (GroundCheck())
        {
            jumpCount = 0;
        }
        if(jumpCount < 2)
        {
            if (jumpCount ==1)
            {
                rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
                rigidbody.AddForce(Vector3.up * doubleJumpForce, ForceMode.Impulse);
                animator.Play("Jump", 0, 0f);
            }
            else if (jumpCount ==0)
            {
               
                rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
                rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                
            }
            jumpCount++;

        }
       
    }
    public void ColorChange()
    {
        if (stateColor == StateColor.Red)
        {
            stateColor = StateColor.Yellow;
            foreach(var item in render)
            {
                item.material.color = Color.yellow;
            }
           
        }
        else if (stateColor == StateColor.Yellow)
        {
            stateColor = StateColor.Red;
            foreach (var item in render)
            {
                item.material.color = Color.red;
             }
        }
    }

    public void Die()
    {
        foreach(var item in render)
        {
            item.enabled = false;
        }
        
        isAlive = false;
        rigidbody.velocity = Vector3.zero;
        Invoke("ReStartGame", 1);
    }

    public void ReStartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }

    private void OnCollisionEnter(Collision collision)
    {
        collisionCut = collision;
    }
    private void OnCollisionStay(Collision collision)
    {
        collisionCut = collision;
    }
    private void OnCollisionExit(Collision collision)
    {
        collisionCut = null;
    }

}
*/








using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public enum StateColor
    {
        Red,
        Yellow
    }

    [Header("颜色与渲染设置")]
    public StateColor currentColor = StateColor.Red;
    public Renderer[] renderers;  // 存储所有需要变色的渲染器
    public Material redMaterial;  // 红色材质（可在Inspector中赋值）
    public Material yellowMaterial;  // 黄色材质（可在Inspector中赋值）

    [Header("移动与跳跃参数")]
    public float moveSpeed = 5f;
    public float jumpForce = 4f;
    public float doubleJumpForce = 6f;
    private int jumpCount = 0;
    private bool isOnGround;

    [Header("地面检测设置")]
    public float groundCheckRadius = 0.4f;
    public LayerMask groundLayer;  // 仅检测地面层的碰撞（需在Inspector中设置）

    [Header("状态")]
    public bool isAlive = true;
    private bool isFalling => !isOnGround && rigidbody.velocity.y < -0.1f;  // 判断是否在下落

    // 组件引用
    private Rigidbody rigidbody;
    private Animator animator;
    private HashSet<GameObject> currentCollisions = new HashSet<GameObject>();  // 记录当前所有碰撞的物体

    public GameObject dieEffect;

    void Awake()
    {
        // 获取组件引用
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        // 自动获取所有子物体的渲染器（如果未手动赋值）
        if (renderers == null || renderers.Length == 0)
        {
            renderers = GetComponentsInChildren<Renderer>();
        }

        // 初始化颜色
        UpdateColorVisual();
        isAlive = true;
        dieEffect.SetActive(false);
    }

    void FixedUpdate()
    {
        if (!isAlive) return;

        // 检测是否在地面上（仅检测指定的地面层）
        isOnGround = Physics.CheckSphere(transform.position, groundCheckRadius, groundLayer);
        animator.SetBool("OnGround", isOnGround);

        // 重置跳跃计数（在地面上时）
        if (isOnGround)
        {
            jumpCount = 0;
        }

        // 检测碰撞并处理死亡逻辑
        CheckCollisionDeath();
    }

    // 检测碰撞并判断是否触发死亡
    private void CheckCollisionDeath()
    {
        foreach (var collisionObj in currentCollisions)
        {
            // 黑色地面：直接死亡
            if (collisionObj.CompareTag("Black"))
            {
                Die();
                return;
            }
            // 红色地面：颜色不匹配则死亡
            else if (collisionObj.CompareTag("Red") && currentColor != StateColor.Red)
            {
                Die();
                return;
            }
            // 黄色地面：颜色不匹配则死亡
            else if (collisionObj.CompareTag("Yellow") && currentColor != StateColor.Yellow)
            {
                Die();
                return;
            }
        }
    }

    // 移动逻辑
    public void Move()
    {
        if (!isAlive) return;

        Vector3 velocity = rigidbody.velocity;
        velocity.z = moveSpeed;  // 持续向前移动
        rigidbody.velocity = velocity;
    }

    // 跳跃逻辑
    public void Jump()
    {
        if (!isAlive) return;

        // 最多允许二段跳
        if (jumpCount < 2)
        {
            // 清除当前Y轴速度，避免力的叠加
            Vector3 velocity = rigidbody.velocity;
            velocity.y = 0;
            rigidbody.velocity = velocity;

            // 施加跳跃力（二段跳力度不同）
            float force = (jumpCount == 0) ? jumpForce : doubleJumpForce;
            rigidbody.AddForce(Vector3.up * force, ForceMode.Impulse);

            // 播放跳跃动画
            animator.Play("Jump", 0, 0f);
            animator.SetTrigger("Change");

            jumpCount++;
        }
    }

    // 切换颜色状态
    public void ToggleColor()
    {
        if (!isAlive) return;

        // 切换颜色状态
        currentColor = (currentColor == StateColor.Red) ? StateColor.Yellow : StateColor.Red;
        // 更新视觉表现
        UpdateColorVisual();
       
    }

    // 更新颜色视觉效果
    private void UpdateColorVisual()
    {
        foreach (var renderer in renderers)
        {
            if (currentColor == StateColor.Red)
            {
                renderer.material = redMaterial;  // 使用红色材质
            }
            else
            {
                renderer.material = yellowMaterial;  // 使用黄色材质
            }
        }
    }

    // 死亡处理
    public void Die()
    {
        dieEffect.SetActive(true);
        if (!isAlive) return;

        isAlive = false;
       
        foreach (var renderer in renderers)
        {
            renderer.enabled = false;
        }
        // 停止移动
        rigidbody.velocity = Vector3.zero;
        rigidbody.isKinematic = true;  // 禁用物理交互

        // 1秒后重启游戏
        Invoke(nameof(RestartGame), 3f);
    }

    // 重启游戏
    private void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    // 碰撞事件：添加碰撞物体
    private void OnCollisionEnter(Collision collision)
    {
        if (groundLayer == (groundLayer | (1 << collision.gameObject.layer)))
        {
            currentCollisions.Add(collision.gameObject);
        }
    }

    // 碰撞事件：保持碰撞物体记录
    private void OnCollisionStay(Collision collision)
    {
        if (groundLayer == (groundLayer | (1 << collision.gameObject.layer)))
        {
            if (!currentCollisions.Contains(collision.gameObject))
            {
                currentCollisions.Add(collision.gameObject);
            }
        }
    }

    // 碰撞事件：移除碰撞物体
    private void OnCollisionExit(Collision collision)
    {
        currentCollisions.Remove(collision.gameObject);
    }

    // 绘制地面检测范围的Gizmo（便于调试）
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, groundCheckRadius);
    }
}
