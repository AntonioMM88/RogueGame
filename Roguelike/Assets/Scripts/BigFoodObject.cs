using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigFoodObject : CellObject
{
    public override void PlayerEntered()
    {
            Destroy(gameObject);

            GameManager.Instance.ChangeFood(+10);
    }
    
}
