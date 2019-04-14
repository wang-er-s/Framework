using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private InputMgr inputMgr;
    private CharacterController cc;
    private Vector3 moveDir;
    private float speed = 50;
    
    void Start()
    {
        inputMgr = InputMgr.Instance;
        cc = GetComponent<CharacterController>();
    }

    private void Update() {
        if(inputMgr.IsAttack)
        {
            
        }
    }

    private void LateUpdate()
    {
        moveDir = Vector3.zero;
        if (inputMgr.IsForward)
            moveDir += Vector3.forward;
        if (inputMgr.IsBack)
            moveDir += Vector3.back;
        if (inputMgr.IsLeft)
            moveDir += Vector3.left;
        if (inputMgr.IsRight)
            moveDir += Vector3.right;
        cc.SimpleMove(moveDir * speed * Time.deltaTime);
        transform.LookAt(transform.position + moveDir);
    }

    private void CreateBoom()
    {
        
    }

}
