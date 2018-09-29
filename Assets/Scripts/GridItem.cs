using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridItem : MonoBehaviour {

	public static event Action<GridItem, bool> ToggleTouch = (item, isTouched) => { };

	private ItemType itemType;
	private bool isItemTouched = false;
	private List<GridItem> neighbours;
	private List<GridItem> path;

	private SpriteRenderer spriteRenderer;

	private ItemSkin itemSkin;
	private Vector3 realPos;
	private Vector3 gridPos;


	void Awake()
	{
		neighbours = new List<GridItem> ();
	}


	void Start ()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		itemSkin = GetComponent<ItemSkin> ();
		itemSkin.SetBoardView (spriteRenderer,itemType);
		path = new List <GridItem>();
	}


	void OnMouseDown()
	{
        ToggleTouch(this, true);
        LeanTween.scaleX(gameObject, 0f, 0.05f).setOnComplete(() => {
            LeanTween.scaleX(gameObject, 0.3f, 0.08f);
            });
		

	}

	void OnMouseUp()
	{
		ToggleTouch(this, false);
	}

		
	public void VisitBoardItem ()
	{
		itemSkin.SetVisitedItemSprite (spriteRenderer);
		isItemTouched = true;
	}

	public void UnvisitBoardItem ()
	{
		if (itemType == ItemType.Regular) 
		{
			itemSkin.SetRegularItemSprite (spriteRenderer);
		}

		if (itemType == ItemType.Target) 
		{
			itemSkin.SetTargetItemSprite (spriteRenderer);
		}

		isItemTouched = false;
	}

	public void AddEnemy ()
	{
		if (this.itemType != ItemType.Target)
		this.itemType = ItemType.Enemy;
		itemSkin.SetEnemyItemSprite (spriteRenderer);
		isItemTouched = false;
	}

	public void RemoveEnemy ()
	{
		if (this.itemType != ItemType.Target) {
			this.itemType = ItemType.Regular;
			itemSkin.SetRegularItemSprite (spriteRenderer);
			isItemTouched = false;
		} 
		else 
		{
			itemSkin.SetTargetItemSprite (spriteRenderer);
		}


	}


	public bool CanTouchItem()
	{
		if (!isItemTouched && itemType == ItemType.Regular)
			return true;
		
		return false;
	}

	public ItemType GetItemType()
	{
		return itemType;
	}

	public bool IsPassable()
	{
		return CanTouchItem ();
	}

	public Vector3 GetItemPosition()
	{
		return realPos;
	}

	public void InitGridItem (ItemType type, Vector3 realPos, Vector3 gridPos)
	{
		this.itemType = type;
		this.realPos = new Vector3(realPos.x, realPos.y, realPos.z);
		this.gridPos = new Vector3(gridPos.x, gridPos.y, gridPos.z);
		gameObject.transform.position = this.realPos;
	}
		
	public Vector3 GetGridPos()
	{
		return gridPos;
	}
	public void AddNeighbour(GridItem neigbour)
	{
		neighbours.Add (neigbour);
	}

	public List<GridItem> GetNeighbours()
	{
		return neighbours;
	}

	public void AddParentInPath(GridItem item)
	{
		path.Add (item);
	}

	public void ClearPath()
	{
		path.Clear ();
	}

	public List<GridItem> GetPath()
	{
		return path;
	}

	public GridItem GetPath(int index)
	{
		return path[index];
	}

	public void SetPathFromParent(List<GridItem> parentPath)
	{
		path = parentPath;
	}

	public void RemoveDuplicatesFromPath()
	{
		for (int i = 0; i < path.Count ; i++) 
		{
			for (int j = i+1; j < path.Count ; j++) 
			{
				if (path [i] == path [j]) 
				{
					path.Remove (path [j]);
					j -= 1;
				}
			}
		}

	}



}
