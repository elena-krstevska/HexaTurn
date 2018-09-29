using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
	Regular,
	Visited,
	Target,
	Enemy,
	Invisible

}

public class Level {

	public int level;
	public int numberOfRows;
	public int numberOfColumns;
	public Vector3 enemyPosition;
	public List<Vector3> targetsPosition;
	public List<Vector3> itemsToIgnore;

	public Level(int level,int numberOfColumns,int numberOfRows,Vector3 enemyPosition,List<Vector3> targetsPosition,List<Vector3> itemsToIgnore)
	{
		this.level = level;
		this.numberOfRows = numberOfRows;
		this.numberOfColumns = numberOfColumns;
		this.enemyPosition = enemyPosition;
		this.targetsPosition = targetsPosition;
		this.itemsToIgnore = itemsToIgnore;
	}

}


public class LevelConfiguration : MonoBehaviour {

	List<Level> levelInfo = new List<Level>();
	List<Vector3> targets = new List<Vector3>();
	List<Vector3> ignoredItems = new List<Vector3>();

	ItemType [,] levelConfig;
	int numberOfRows =6;
	int numberOfColumns =6;
	int enemyX = 4;
	int enemyY =5;
	int targetX = 1;
	int targetY =1;

	void Awake()
	{
		SetLevelInfo ();	
	}

	void Start()
	{
		
	}

	public void SetLevelInfo()
	{
		// Level1
		ignoredItems = new List<Vector3>();
		targets = new List<Vector3>();

		targets.Add(new Vector3(1,2,0));

		ignoredItems.Add(new Vector3(0,0,0));
		ignoredItems.Add(new Vector3(0,2,0));
		ignoredItems.Add(new Vector3(0,4,0));
		ignoredItems.Add(new Vector3(0,6,0));
		ignoredItems.Add(new Vector3(0,8,0));

		levelInfo.Add(new Level(1,9,2,new Vector3(1,6,0),targets,ignoredItems));

		//Level2

		ignoredItems = new List<Vector3>();
		targets = new List<Vector3>();

		targets.Add(new Vector3(1,1,0));
		targets.Add(new Vector3(0,1,0));

		ignoredItems.Add(new Vector3(0,0,0));
		ignoredItems.Add(new Vector3(0,6,0));
		ignoredItems.Add(new Vector3(2,0,0));
		ignoredItems.Add(new Vector3(2,6,0));
		ignoredItems.Add(new Vector3(2,1,0));
		ignoredItems.Add(new Vector3(2,3,0));
		ignoredItems.Add(new Vector3(2,5,0));


		levelInfo.Add(new Level(2,7,3,new Vector3(1,6,0),targets,ignoredItems));

		//Level3
		ignoredItems = new List<Vector3>();
		targets = new List<Vector3>();

		targets.Add(new Vector3(1,0,0));
		targets.Add(new Vector3(0,11,0));
//		targets.Add(new Vector3(1,11,0));


		ignoredItems.Add(new Vector3(0,0,0));
		ignoredItems.Add(new Vector3(0,2,0));
		ignoredItems.Add(new Vector3(0,4,0));
		ignoredItems.Add(new Vector3(0,6,0));
		ignoredItems.Add(new Vector3(0,8,0));
		ignoredItems.Add(new Vector3(0,10,0));
		ignoredItems.Add(new Vector3(0,12,0));


		levelInfo.Add(new Level(3,13,2,new Vector3(1,6,0),targets,ignoredItems));
	}

	bool ItemShouldBeIgnored(int x, int y,int levelId)
	{
		foreach (Vector3 itemPos in levelInfo[levelId - 1].itemsToIgnore) 
		{
			if ((int)itemPos.x == x && (int)itemPos.y == y)
				return true;
		}

		return false;
	}

	bool ItemIsTarget(int x, int y,int levelId)
	{
		foreach (Vector3 itemPos in levelInfo[levelId - 1].targetsPosition) 
		{
			if ((int)itemPos.x == x && (int)itemPos.y == y)
				return true;
		}

		return false;
	}






	public ItemType [,] CreateLevelConfig(int level)
	{
		numberOfRows = levelInfo [level - 1].numberOfRows;
		numberOfColumns = levelInfo [level - 1].numberOfColumns;

		enemyX = (int)levelInfo [level - 1].enemyPosition.x;
		enemyY = (int)levelInfo [level - 1].enemyPosition.y;

		targetX = (int)levelInfo [level - 1].targetsPosition [0].x;
		targetY = (int)levelInfo [level - 1].targetsPosition [0].y;

		levelConfig = new ItemType[numberOfRows, numberOfColumns];

		for (int i = 0; i < numberOfRows; i++) {
			for (int j = 0; j < numberOfColumns; j++) {


				if (i == enemyX && j==enemyY) {
					levelConfig [i, j] = ItemType.Enemy;
					continue;
				}

				if (ItemIsTarget (i, j, level)) 
				{
					levelConfig [i, j] = ItemType.Target;
					continue;
				}


//				if (i == targetX && j==targetY) {
//					levelConfig [i, j] = ItemType.Target;
//					continue;
//				}

				if (ItemShouldBeIgnored(i,j,level))
					levelConfig [i, j] = ItemType.Invisible;
				else
				levelConfig [i, j] = ItemType.Regular;


			}
		}
		return levelConfig;
	}


	public ItemType [,] GetLevelConfig()
	{
		return levelConfig;
		
	}

	public int GetNumberOfRows()
	{
		return numberOfRows;
	}

	public int GetNumberOfColumns()
	{
		return numberOfColumns;
	}

	public Vector3 GetTargetPosition()
	{
		return new Vector3(targetX,targetY,0);
	}

	public List<Vector3> GetTargetsPositions(int lvl)
	{
		return levelInfo [lvl - 1].targetsPosition;
	}

	public Level GetLevelInfo(int lvl)
	{
		return levelInfo [lvl - 1];
	}



}
