using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	Vector3 currentGridPos;
	GridItem parentItem;
	List<GridItem> enemyMoves ;

	void Awake()
	{
		enemyMoves = new List<GridItem> ();
	}


	public void SpawnEnemy(int x, int y, GridItem item)
	{
		currentGridPos = new Vector3 (x, y, 1.0f);
		gameObject.transform.position = item.GetItemPosition ();
		parentItem = item;
		enemyMoves.Add (parentItem);
	}

	public void MoveToNewPosition(int x, int y, GridItem item)
	{
		
		parentItem.RemoveEnemy();
		parentItem = item;
		currentGridPos = new Vector3 (x, y, 1.0f);

//		this.transform.LookAt (new Vector3(parentItem.GetItemPosition ().x,0,0));
		Vector3 diff = parentItem.transform.position - this.transform.position;
		float angle = Vector3.Angle (Vector3.up, diff);
		this.transform.rotation = Quaternion.identity;
		this.transform.Rotate (Vector3.forward, angle);
        LeanTween.scale(gameObject,Vector3.zero,0.05f).setOnComplete(()=>
        {
            gameObject.transform.position = parentItem.GetItemPosition();
            parentItem.AddEnemy();
            enemyMoves.Add(item);
            LeanTween.scale(gameObject, Vector3.one, 0.03f);
        });
       
	}

	public void MoveToOldPosition(int x, int y, GridItem item)
	{
		parentItem.RemoveEnemy();
		parentItem = item;
		this.transform.LookAt (parentItem.transform);
		currentGridPos = new Vector3 (x, y, 1.0f);
//		this.transform.LookAt (parentItem.GetItemPosition (),Vector3.up);
		Vector3 diff = parentItem.transform.position - this.transform.position;
		float angle = Vector3.Angle (Vector3.up, diff);
		this.transform.rotation = Quaternion.identity;
		this.transform.Rotate (Vector3.forward, angle);
		gameObject.transform.position = parentItem.GetItemPosition ();
		parentItem.AddEnemy();
		enemyMoves.RemoveAt (enemyMoves.Count-1);
	}

	 void LookAtTarget(GridItem target)
	{
//		this.transform.LookAt (target);
	}
		
	public Vector3 GetGridPos()
	{
		return currentGridPos;
	}

	public List<GridItem> GetEnemyMoves()
	{
		return enemyMoves;
	}

}
