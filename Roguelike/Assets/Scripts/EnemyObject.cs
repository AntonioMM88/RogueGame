using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObject : CellObject
{
    public int Health = 3;

    private int m_CurrentHealth;
    private Animator m_EnemyAnimator;

    private void Awake()
    {
        GameManager.Instance.turnManager.OnTick += EnemyTurnHappen;
        m_EnemyAnimator = GetComponent<Animator>();
    }
    private void OnDestroy()
    {
        GameManager.Instance.turnManager.OnTick -= EnemyTurnHappen;
    }
    public override void Init(Vector2Int coord)
    {
        base.Init(coord);
        m_CurrentHealth = Health;
    }

    public override bool PlayerWantsToEnter()
    {
        m_CurrentHealth -= 1;
        if (m_CurrentHealth <= 0)
        {
            Destroy(gameObject);
        }
        
        return false;
    }

    bool MoveTo(Vector2Int coord)
    {
        var board = GameManager.Instance.boardManager;
        var targetCell = board.GetCellData(coord);

        if (targetCell == null || !targetCell.passable || targetCell.ContainedObject != null)
        {
            return false;
        }

        var currentCell = board.GetCellData(m_Cell);
        currentCell.ContainedObject = null;

        targetCell.ContainedObject = this;
        m_Cell = coord;
        transform.position = board.CellToWorld(coord);

        return true;
    }

    void EnemyTurnHappen()
    {
        var playerCell = GameManager.Instance.playerController.CellPosition;

        int xDist = playerCell.x - m_Cell.x;
        int yDist = playerCell.y - m_Cell.y;

        int absXDist = Mathf.Abs(xDist);
        int absYDist = Mathf.Abs(yDist);

        if((xDist == 0 && absYDist == 1) || (yDist == 0 && absXDist == 1))
        {
            GameManager.Instance.ChangeFood(-3);
            m_EnemyAnimator.SetBool("Attack", true);
        }

        else
        {
            m_EnemyAnimator.SetBool("Attack", false);
            if (absXDist > absYDist)
            {
                if (!TryMoveInX(xDist))
                {
                    TryMoveInY(yDist);
                }
            }
            else if (!TryMoveInY(yDist))
            {
                TryMoveInX(xDist);
            }

        }
    }

    bool TryMoveInX (int xDist)
    {
        if (xDist > 0)
        {
            return MoveTo(m_Cell + Vector2Int.right);
        }

        return MoveTo(m_Cell + Vector2Int.left);
    }

    bool TryMoveInY(int yDist)
    {
        if (yDist > 0)
        {
            return MoveTo(m_Cell + Vector2Int.up);
        }

        return MoveTo(m_Cell + Vector2Int.down);
    }
}

