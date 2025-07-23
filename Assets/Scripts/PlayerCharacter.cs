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

    [Header("��ɫ����Ⱦ����")]
    public StateColor currentColor = StateColor.Red;
    public Renderer[] renderers;  // �洢������Ҫ��ɫ����Ⱦ��
    public Material redMaterial;  // ��ɫ���ʣ�����Inspector�и�ֵ��
    public Material yellowMaterial;  // ��ɫ���ʣ�����Inspector�и�ֵ��

    [Header("�ƶ�����Ծ����")]
    public float moveSpeed = 5f;
    public float jumpForce = 4f;
    public float doubleJumpForce = 6f;
    private int jumpCount = 0;
    private bool isOnGround;

    [Header("����������")]
    public float groundCheckRadius = 0.4f;
    public LayerMask groundLayer;  // ������������ײ������Inspector�����ã�

    [Header("״̬")]
    public bool isAlive = true;
    private bool isFalling => !isOnGround && rigidbody.velocity.y < -0.1f;  // �ж��Ƿ�������

    // �������
    private Rigidbody rigidbody;
    private Animator animator;
    private HashSet<GameObject> currentCollisions = new HashSet<GameObject>();  // ��¼��ǰ������ײ������

    public GameObject dieEffect;

    void Awake()
    {
        // ��ȡ�������
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        // �Զ���ȡ�������������Ⱦ�������δ�ֶ���ֵ��
        if (renderers == null || renderers.Length == 0)
        {
            renderers = GetComponentsInChildren<Renderer>();
        }

        // ��ʼ����ɫ
        UpdateColorVisual();
        isAlive = true;
        dieEffect.SetActive(false);
    }

    void FixedUpdate()
    {
        if (!isAlive) return;

        // ����Ƿ��ڵ����ϣ������ָ���ĵ���㣩
        isOnGround = Physics.CheckSphere(transform.position, groundCheckRadius, groundLayer);
        animator.SetBool("OnGround", isOnGround);

        // ������Ծ�������ڵ�����ʱ��
        if (isOnGround)
        {
            jumpCount = 0;
        }

        // �����ײ�����������߼�
        CheckCollisionDeath();
    }

    // �����ײ���ж��Ƿ񴥷�����
    private void CheckCollisionDeath()
    {
        foreach (var collisionObj in currentCollisions)
        {
            // ��ɫ���棺ֱ������
            if (collisionObj.CompareTag("Black"))
            {
                Die();
                return;
            }
            // ��ɫ���棺��ɫ��ƥ��������
            else if (collisionObj.CompareTag("Red") && currentColor != StateColor.Red)
            {
                Die();
                return;
            }
            // ��ɫ���棺��ɫ��ƥ��������
            else if (collisionObj.CompareTag("Yellow") && currentColor != StateColor.Yellow)
            {
                Die();
                return;
            }
        }
    }

    // �ƶ��߼�
    public void Move()
    {
        if (!isAlive) return;

        Vector3 velocity = rigidbody.velocity;
        velocity.z = moveSpeed;  // ������ǰ�ƶ�
        rigidbody.velocity = velocity;
    }

    // ��Ծ�߼�
    public void Jump()
    {
        if (!isAlive) return;

        // ������������
        if (jumpCount < 2)
        {
            // �����ǰY���ٶȣ��������ĵ���
            Vector3 velocity = rigidbody.velocity;
            velocity.y = 0;
            rigidbody.velocity = velocity;

            // ʩ����Ծ�������������Ȳ�ͬ��
            float force = (jumpCount == 0) ? jumpForce : doubleJumpForce;
            rigidbody.AddForce(Vector3.up * force, ForceMode.Impulse);

            // ������Ծ����
            animator.Play("Jump", 0, 0f);
            animator.SetTrigger("Change");

            jumpCount++;
        }
    }

    // �л���ɫ״̬
    public void ToggleColor()
    {
        if (!isAlive) return;

        // �л���ɫ״̬
        currentColor = (currentColor == StateColor.Red) ? StateColor.Yellow : StateColor.Red;
        // �����Ӿ�����
        UpdateColorVisual();
       
    }

    // ������ɫ�Ӿ�Ч��
    private void UpdateColorVisual()
    {
        foreach (var renderer in renderers)
        {
            if (currentColor == StateColor.Red)
            {
                renderer.material = redMaterial;  // ʹ�ú�ɫ����
            }
            else
            {
                renderer.material = yellowMaterial;  // ʹ�û�ɫ����
            }
        }
    }

    // ��������
    public void Die()
    {
        dieEffect.SetActive(true);
        if (!isAlive) return;

        isAlive = false;
       
        foreach (var renderer in renderers)
        {
            renderer.enabled = false;
        }
        // ֹͣ�ƶ�
        rigidbody.velocity = Vector3.zero;
        rigidbody.isKinematic = true;  // ����������

        // 1���������Ϸ
        Invoke(nameof(RestartGame), 3f);
    }

    // ������Ϸ
    private void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    // ��ײ�¼��������ײ����
    private void OnCollisionEnter(Collision collision)
    {
        if (groundLayer == (groundLayer | (1 << collision.gameObject.layer)))
        {
            currentCollisions.Add(collision.gameObject);
        }
    }

    // ��ײ�¼���������ײ�����¼
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

    // ��ײ�¼����Ƴ���ײ����
    private void OnCollisionExit(Collision collision)
    {
        currentCollisions.Remove(collision.gameObject);
    }

    // ���Ƶ����ⷶΧ��Gizmo�����ڵ��ԣ�
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, groundCheckRadius);
    }
}
