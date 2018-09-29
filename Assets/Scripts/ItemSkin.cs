using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSkin : MonoBehaviour {

	public Sprite RegularSprite; 
	public Sprite VisitedSprite;
	public Sprite TargetSprite;
	public Sprite EnemySprite;
	public Sprite InvisibleSprite;

	void Start () {
		
	}
	
	public void SetBoardView(SpriteRenderer spriteRenderer, ItemType itemType){

	   spriteRenderer.sprite = GetSpriteForItem(itemType);

	}

	public void SetVisitedItemSprite(SpriteRenderer spriteRenderer){

		spriteRenderer.sprite = VisitedSprite;

	}

	public void SetUnvisitedItemSprite(SpriteRenderer spriteRenderer){

		spriteRenderer.sprite = RegularSprite;

	}

	public void SetRegularItemSprite(SpriteRenderer spriteRenderer){

		spriteRenderer.sprite = RegularSprite;

	}

	public void SetEnemyItemSprite(SpriteRenderer spriteRenderer){

		spriteRenderer.sprite = EnemySprite;

	}

	public void SetTargetItemSprite(SpriteRenderer spriteRenderer){

		spriteRenderer.sprite = TargetSprite;

	}



	Sprite GetSpriteForItem(ItemType type)
	{

		switch (type)
		{
		case ItemType.Regular:
			return RegularSprite;
			break;
		case ItemType.Visited:
			return VisitedSprite;
			break;
		case ItemType.Target:
			return TargetSprite;
			break;
		case ItemType.Enemy:
			return EnemySprite;
			break;
		case ItemType.Invisible:
			return InvisibleSprite;
			break;
		default:
			return RegularSprite;
			break;
		}

	}
}
