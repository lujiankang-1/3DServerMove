using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHuman : MonoBehaviour
{
    public string desc;
    protected bool IsMoving = false;
    public float MoveSpeed = 1f;
    protected Vector3 targetPos;
    protected Vector3 currentPos;
    private Animator animator;

    public void Move()
    {
        if (Vector2.Distance(new Vector2(currentPos.x, currentPos.z),
            new Vector2(targetPos.x, targetPos.z)) < 0.1f)
        {
            animator.SetBool("Move", false);
            IsMoving = false;
            return;
        }
        animator.SetBool("Move", true);
        IsMoving = true;
        currentPos = transform.position;
        currentPos = Vector3.MoveTowards(currentPos, targetPos, MoveSpeed * Time.deltaTime);
        Vector3 forward = targetPos - currentPos;
        forward.y = 0;
        transform.forward = forward;
        transform.position = currentPos;
    }
    protected void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void MoveTo(Vector3 pos)
    {
        targetPos = pos;
    }

    protected void Update()
    {
        Move();
    }
}
