using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private bool m_isMoving;
    public float MoveSpeed = 5f;
    private Vector3 m_MoveTarget;

    private BoardManager m_Board;
    public Vector2Int CellPosition;

    public bool m_IsGameOver;

    private Animator m_Animator;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    public void Init()
    {
        m_IsGameOver = false;
        m_isMoving = false;
        m_Animator.SetBool("Breaking", false);
       
    }

    public void Spawn(BoardManager boardManager, Vector2Int cell)
    {
        m_Board = boardManager;
        MoveTo(cell, true);
        m_Animator.SetBool("Breaking", false);
        
    }

    public void MoveTo(Vector2Int cell, bool inmediate)
    {
        CellPosition = cell; 

        if(inmediate )
        {
            m_isMoving= false;
            transform.position = m_Board.CellToWorld(CellPosition);
        }
        else
        {
            m_isMoving = true;
            m_MoveTarget = m_Board.CellToWorld(CellPosition);
        }
        m_Animator.SetBool("Moving", m_isMoving);

    }

    public void GameOver()
    {
        m_IsGameOver = true;
    }

    private void Update()
    {
        Vector2Int newCellTraget = CellPosition;
        bool hasMoved = false;

        if (m_IsGameOver)
        {
            if(Keyboard.current.enterKey.wasPressedThisFrame)
            {
                GameManager.Instance.StartNewGame();
            }
            return;
        }
        if (m_isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, m_MoveTarget, MoveSpeed*Time.deltaTime);
            m_Animator.SetBool("Breaking", false);

            if (transform.position == m_MoveTarget)
            {
                m_isMoving = false;
                m_Animator.SetBool("Moving", false);
                var cellData = m_Board.GetCellData(CellPosition);
                if (cellData.ContainedObject != null) cellData.ContainedObject.PlayerEntered();
            }
            return;
        }
        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            newCellTraget.y += 1;
            hasMoved = true; 
        }
        else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            newCellTraget.y -= 1;
            hasMoved = true;
        }
        else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            newCellTraget.x -= 1;
            hasMoved = true;
        }
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            newCellTraget.x += 1;
            hasMoved = true;
        }
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            GameManager.Instance.turnManager.Tick();
        }

            if (hasMoved)
        {
            BoardManager.CellData cellData = m_Board.GetCellData(newCellTraget);
            

            if (cellData != null && cellData.passable)
            {
                GameManager.Instance.turnManager.Tick();
                m_Animator.SetBool("Breaking", true);

                if (cellData.ContainedObject == null)
                {
                    MoveTo(newCellTraget, false);
                }
                else if (cellData.ContainedObject.PlayerWantsToEnter())
                {
                    MoveTo(newCellTraget, false);
                    cellData.ContainedObject.PlayerEntered();
                }

                

            }

        }

        
    }
}
