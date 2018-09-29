using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Grid : MonoBehaviour {

	public GameObject hexPrefab;
	public GameObject enemyPrefab;
	public LevelConfiguration levelConfig;
    public Shaker shaker;
 

    int gridWidth;
	int gridHeight;
	int currentLevel = 1;
	int maxNumberOfLevels = 3;

	float hexWidth = 2.3f;
	float hexHeight = 1.7f;

	GridItem [,] gridItems;
	List<GridItem> reachableItems;

	
	ItemType [,] currentLevelConfig;
	public float gap = 0.0f;
	List<GridItem> playerMoves ;

	List<Enemy> enemies;
	Vector3 startPos;

	bool levelFail = false;
	GameState gameState;

	List<GridItem> currentPath;



	List<List<GridItem>> originalPath;
	List<List<GridItem>> allTargetsPath;
	List<Vector3> targetsPosition;
	List<GridItem> pathToMoveTo;

    public static event Action ShakeRevertButton = () => { };
        // LeanTween.scale (backButton.gameObject, Vector3.one * 0.9f, 0.2f).setLoopPingPong (2);

    void Awake()
	{
		
		reachableItems = new List<GridItem> ();
		playerMoves = new List<GridItem> ();

		gameState = GameState.Init;
		enemies = new List<Enemy> ();

		originalPath = new List<List<GridItem>> ();
		allTargetsPath = new List<List<GridItem>> ();
		currentPath = new List<GridItem> ();

		pathToMoveTo = new List<GridItem> ();

		targetsPosition = new List<Vector3> ();

	}



	void Start()
	{
		
		levelConfig.CreateLevelConfig (currentLevel);
		currentLevelConfig = levelConfig.GetLevelConfig ();
		gridWidth = levelConfig.GetNumberOfRows();
		gridHeight = levelConfig.GetNumberOfColumns();
		gridItems = new GridItem[gridWidth, gridHeight];


		AddGap();
		CalcStartPos();
		CreateGrid();
		//CreateUI ();
		AddListeners ();
		gameState = GameState.AwaitingInput;
		GenerateNeighbours ();

	}

	public void ClearOldPath ()
	{
		foreach (GridItem item in gridItems)
			item.ClearPath ();
	}

	

	bool GetReachableItems(GridItem start, GridItem target)
	{
		var neighbours = start.GetNeighbours ();


		foreach (var neighbour in neighbours) 
		{
			if (neighbour == target) 
			{
				reachableItems.Add (neighbour);
				return true;
			}
			
			if (neighbour.IsPassable () && !reachableItems.Contains(neighbour)) 
			{
				reachableItems.Add (neighbour);
				GetReachableItems (neighbour,target);
		    }
	    }

		return false;

	}
		

	public bool AlreadyVisited(GridItem item, List<GridItem> order)
	{
		int x = (int)item.GetGridPos ().x;
		int y = (int)item.GetGridPos ().y;

		foreach(var hexItem in order)
		{
			int hx = (int)hexItem.GetGridPos ().x;
			int hy = (int)hexItem.GetGridPos ().y;

			if (gridItems[hx,hy] == gridItems[x,y]) 
			{
				return true;
			}

			var path = gridItems[hx,hy].GetPath ();

			foreach (var node in path) 
			{
				if (node == item) 
				{
					return true;
				}
					
			}

		}	

		return false;
	}

	public void GetParentPath(GridItem node, GridItem start)
	{
		int nx = (int)node.GetGridPos ().x;
		int ny = (int)node.GetGridPos ().y;

		int sx = (int)start.GetGridPos ().x;
		int sy = (int)start.GetGridPos ().y;


		if (gridItems[nx,ny] == gridItems[sx,sy]) 
		{
			originalPath.Add (currentPath);
			currentPath = new List<GridItem> ();
			return;
		}
			
		List<GridItem> nodePath = node.GetPath ();

		if (gridItems[nx,ny] != gridItems[sx,sy])
		{
			currentPath.Add (nodePath[0]);
		}

		GetParentPath (nodePath[0],start);

	}


	public List<GridItem> FindShortPath(GridItem start,GridItem goal)
	{
		int x = (int)goal.GetGridPos ().x;
		int y = (int)goal.GetGridPos ().y;

		int sx = (int)start.GetGridPos ().x;
		int sy = (int)start.GetGridPos ().y;

		List<GridItem> goalPath = gridItems[x,y].GetPath ();
	

		foreach (GridItem node in goalPath) 
		{

			int nx = (int)node.GetGridPos ().x;
			int ny = (int)node.GetGridPos ().y;

			currentPath.Add (gridItems[x,y]);
			currentPath.Add (gridItems[nx,ny]);
			GetParentPath (gridItems[nx,ny],gridItems[sx,sy]);
		}
		List<GridItem> FINALPATHFINAL = new List<GridItem> ();

		FINALPATHFINAL = originalPath [0];
		bool isBlocked = false;
		for (int i = 0; i < originalPath.Count ; i++) 
		{
			if (originalPath [i].Count < FINALPATHFINAL.Count) 
			{
				isBlocked = true;
				foreach (var item in originalPath[i]) 
				{
					if (!item.IsPassable ())
						isBlocked = true;
				}
				if (isBlocked == false) 
				{
					FINALPATHFINAL = originalPath [i];
				}
			}
				

		}
		FINALPATHFINAL.Reverse ();
		return FINALPATHFINAL;

	}




	public void Traverse(GridItem root,GridItem goal)
	{
		Queue<GridItem> traverseOrder = new Queue<GridItem>();
		List<GridItem> order = new List<GridItem> ();

		Queue<GridItem> Q = new Queue<GridItem>();
		HashSet<GridItem> S = new HashSet<GridItem>();
		Q.Enqueue(root);
		S.Add(root);

		while (Q.Count > 0)
		{
			GridItem e = Q.Dequeue();
			traverseOrder.Enqueue(e);
			order.Add (e);


			foreach (GridItem emp in e.GetNeighbours())
			{
				int x = (int)emp.GetGridPos ().x;
				int y = (int)emp.GetGridPos ().y;

				if (emp == goal) {
//					gridItems [x, y].SetPathFromParent (e.GetPath());
					gridItems [x, y].AddParentInPath (e);
					break;
				} else {
					if (!S.Contains (emp) && emp.IsPassable () && !AlreadyVisited(emp,order)) {
						Q.Enqueue (emp);
						S.Add (emp);

//						gridItems [x, y].SetPathFromParent (e.GetPath());
						gridItems [x, y].AddParentInPath (e);

					}
				}
			}
		}

		while (traverseOrder.Count > 0)
		{
			GridItem e = traverseOrder.Dequeue();
		}
	}



	void CreateNeighboursForItem1(GridItem item)
	{	
		int x = (int)item.GetGridPos ().x;
		int y = (int)item.GetGridPos ().y;


		if (y-2 >= 0 && gridItems[x,y-2] != null) {
			item.AddNeighbour (gridItems [x, y - 2]);
		}

		if (y-1 >= 0 && gridItems[x,y-1] != null) {
			item.AddNeighbour (gridItems [x, y - 1]);
		}

		if (y+1 < gridItems.GetLength(1) && gridItems[x,y+1] != null) {
			item.AddNeighbour (gridItems [x, y+1]);
		}

		if (y+2 < gridItems.GetLength(1) && gridItems[x,y+2] != null) {
			item.AddNeighbour (gridItems [x, y + 2]);
		}

		if (x-1 >= 0 && y+1 < gridItems.GetLength(1) &&  gridItems[x-1,y+1] != null) {
			item.AddNeighbour (gridItems[x-1,y+1]);
		}
		if (x-1 >= 0 && y-1 > 0 && gridItems[x-1,y-1] != null) {
			item.AddNeighbour (gridItems[x-1,y-1]);
		}
	}

	void CreateNeighboursForItem2(GridItem item)
	{

		int x = (int)item.GetGridPos ().x;
		int y = (int)item.GetGridPos ().y;
		if (y-2 >= 0 && gridItems[x,y-2] != null) {
			item.AddNeighbour (gridItems [x, y - 2]);
		}


		if (x+1 < gridItems.GetLength(0)&& y-1 >= 0 && gridItems[x+1,y-1] != null) {
			item.AddNeighbour (gridItems [x+1, y - 1]);
		}

		if (x+1 < gridItems.GetLength(0)&& y+1 < gridItems.GetLength(1)&& gridItems[x+1,y+1] != null) {
			item.AddNeighbour (gridItems [x+1, y+1]);
		}

		if (y+2 < gridItems.GetLength(1)&&  gridItems[x,y+2] != null) {
			item.AddNeighbour (gridItems [x, y + 2]);
		}

		if (y+1 < gridItems.GetLength(1)&& gridItems[x,y+1] != null) {
			item.AddNeighbour (gridItems[x,y+1]);
		}
		if (y-1 >= 0 &&gridItems[x,y-1] != null) {
			item.AddNeighbour (gridItems[x,y-1]);
		}
	}


	void GenerateNeighbours()
	{
		for (int i = 0; i < gridItems.GetLength(0); i++) {
			for (int j = 0; j < gridItems.GetLength(1); j++) {
				GridItem currentItem = gridItems [i, j];

				if  (currentItem.GetGridPos().y%2 ==0 ) {
					CreateNeighboursForItem1 (currentItem);
				}else{
					CreateNeighboursForItem2 (currentItem);
	
				}
			}
		}
	}


	void AddGap()
	{
		hexWidth += hexWidth * gap;
		hexHeight += hexHeight * gap;
	}

	void CalcStartPos()
	{
		float offset = 0;
		if (gridHeight / 2 % 2 != 0)
			offset = hexWidth / 4;

		float x = -hexWidth * (gridWidth / 2) - offset;
		float y = hexHeight * 0.4f * (gridHeight / 2);

		startPos = new Vector3(x, y, 0);
	}

	Vector3 CalcWorldPos(Vector2 gridPos)
	{
		float offset = 0;
		if (gridPos.y % 2 != 0)
			offset = hexWidth / 2;

		float x = startPos.x + gridPos.x * hexWidth + offset;
		float y = startPos.y - gridPos.y * hexHeight * 0.4f;

		return new Vector3(x, y, 0);
	}

	void CreateGrid()
	{
		for (int x = 0; x < gridWidth; x++)
		{
			for (int y = 0; y < gridHeight; y++)
			{
				GameObject hex = Instantiate(hexPrefab);
                hex.transform.localScale = new Vector3(0,0,1);
                LeanTween.scale(hex,new Vector3(0.3f,0.3f,0.3f), UnityEngine.Random.Range(0.1f, 0.7f));
                Vector3 gridPos = new Vector3(x, y, 1.0f);
				Vector3 realPos = CalcWorldPos(gridPos);
				hex.transform.parent = this.transform;

				hex.name = "Hexagon" + x + "|" + y;

				gridItems [x,y] = hex.GetComponent<GridItem>();
				gridItems [x,y].InitGridItem (currentLevelConfig[x,y], realPos, gridPos);

				if (currentLevelConfig[x,y] == ItemType.Enemy) {
					SpawnEnemy (x, y, gridItems [x,y]);
				}
			}
		}
	}


	void SpawnEnemy(int x, int y,GridItem item)
	{
		Enemy currentEnemy = Instantiate (enemyPrefab).GetComponent<Enemy> ();
		currentEnemy.SpawnEnemy (x, y, item);
		enemies.Add (currentEnemy);
	}

	void OnDestroy ()
	{
		RemoveListeners ();
	}

	void AddListeners ()
	{
		GridItem.ToggleTouch += ItemTouched;
		UIController.RevertLastMove += RevertLastMove;
	}

	void RevertLastMove()
	{
			levelFail = false;
			RevertEnemyMoves ();
			RevertPlayerMoves ();
			AddItemsListener ();
			
	}

	void RevertEnemyMoves()
	{
		foreach (Enemy enemy in enemies) 
		{
			List<GridItem> enemyMoves = enemy.GetEnemyMoves ();
			if (enemyMoves.Count > 1) 
			{
				GridItem lastMove = enemyMoves [enemyMoves.Count - 2];
				int nX = (int)lastMove.GetGridPos ().x;
				int nY = (int)lastMove.GetGridPos ().y;

				enemy.MoveToOldPosition (nX,nY,lastMove);

			}
		}
	}

	void RevertPlayerMoves()
	{
		if (playerMoves.Count > 0) 
		{
			GridItem lastMove = playerMoves [playerMoves.Count - 1];
			int nX = (int)lastMove.GetGridPos ().x;
			int nY = (int)lastMove.GetGridPos ().y;

			gridItems [nX, nY].UnvisitBoardItem ();
			playerMoves.RemoveAt (playerMoves.Count - 1);
		}
	}

	void ItemTouched (GridItem item, bool isTouched)
	{
		if (isTouched) {
			HandleItemTouch (item);
		} 
		else
		{
		   HandleEnemyMove (item);

		}
	}


	void HandleItemTouch (GridItem item)
	{
		
		if (item.CanTouchItem ()) 
		{   
			item.VisitBoardItem ();
		}
	    
	}
		

	List<GridItem> FindClosesetTargetPath()
	{
		var closestTargetPath = allTargetsPath[0];
		
		foreach (var path in allTargetsPath) 
		{
			if (path.Count < closestTargetPath.Count)
				closestTargetPath = path;
		}

		return closestTargetPath;
	}

	void HandleEnemyMove(GridItem item)
	{   

		if (item.GetItemType() != ItemType.Regular)
			return;

		if (playerMoves.Contains (item)) {
			return;
		} 
		else 
		{
			playerMoves.Add(item);
		}
			

		var LastItemTouched = playerMoves[playerMoves.Count - 1];

		targetsPosition = new List<Vector3> ();
		targetsPosition = levelConfig.GetTargetsPositions (currentLevel);
		allTargetsPath = new List<List<GridItem>> ();
//		int targetXPos = (int)levelConfig.GetTargetPosition ().x;
//		int targetYPos = (int)levelConfig.GetTargetPosition ().y;

//		GridItem goal = gridItems[targetXPos,targetYPos];


		foreach (Enemy enemy in enemies)
		{
			int enemyX = (int)enemy.GetGridPos ().x;
			int enemyY = (int)enemy.GetGridPos ().y;

			if (CheckWinCondition (gridItems [enemyX, enemyY],targetsPosition)) 
			{
				return;
			}

				
			foreach (var target in targetsPosition) 
			{
				if (IsTargetReachable (gridItems[enemyX,enemyY],gridItems[(int)target.x,(int)target.y])) {		
					ClearOldPath ();
					originalPath = new List<List<GridItem>> ();
					Traverse (gridItems [enemyX, enemyY], gridItems [(int)target.x, (int)target.y]);
					pathToMoveTo = new List<GridItem> ();
					pathToMoveTo = FindShortPath (gridItems [enemyX, enemyY], gridItems [(int)target.x, (int)target.y]);
					allTargetsPath.Add (pathToMoveTo);
				}
			}

			if (allTargetsPath.Count > 1) 
			{
				pathToMoveTo = new List<GridItem>(); 
				pathToMoveTo = FindClosesetTargetPath ();
			}
				
			int firstMoveX = (int)pathToMoveTo[1].GetGridPos().x;
			int firstMoveY = (int)pathToMoveTo[1].GetGridPos().y;

			enemy.MoveToNewPosition (firstMoveX ,firstMoveY,gridItems [firstMoveX, firstMoveY] );
		    
			levelFail = CheckFailCondition(enemy,targetsPosition);
			if (levelFail) 
			{
				RemoveItemsListener ();
				StartFailLevelAnimation ();
			}
		}
	}

	void ResetAllData()
	{
		reachableItems = new List<GridItem> ();
		playerMoves = new List<GridItem> ();

		gameState = GameState.Init;
		enemies = new List<Enemy> ();

		originalPath = new List<List<GridItem>> ();
		allTargetsPath = new List<List<GridItem>> ();
		currentPath = new List<GridItem> ();

		pathToMoveTo = new List<GridItem> ();

		targetsPosition = new List<Vector3> ();

		foreach (var item in gridItems) 
		{ Destroy (item.gameObject);
		}
	}

	void GoToNextLevel()
	{
		if (currentLevel <= maxNumberOfLevels) {
			ResetAllData ();
			levelConfig.CreateLevelConfig (currentLevel);
			currentLevelConfig = levelConfig.GetLevelConfig ();
			gridWidth = levelConfig.GetNumberOfRows ();
			gridHeight = levelConfig.GetNumberOfColumns ();
			gridItems = new GridItem[gridWidth, gridHeight];


			AddGap ();
			CalcStartPos ();
			CreateGrid ();
			//CreateUI ();
			AddListeners ();
			gameState = GameState.AwaitingInput;
			GenerateNeighbours ();
		} else 
		{
			GoBackToCover ();
		}
	}

	void GoBackToCover()
	{
		Application.LoadLevel(0);
	}

	void StartEndLevelAnimation()
	{
		foreach (GridItem item in gridItems) 
		{
            LeanTween.scale(item.gameObject,Vector3.zero,UnityEngine.Random.Range(0.1f,0.7f));
		}

		foreach (Enemy enemy in enemies) 
		{
            LeanTween.scale(enemy.gameObject, Vector3.zero, UnityEngine.Random.Range(0.1f, 0.7f));
        }
		
	}

	void StartFailLevelAnimation()
	{
        shaker.StartShake(0.5f, 0.5f);
        ShakeRevertButton();

	}

	bool CheckFailCondition(Enemy enemy, List<Vector3> targets)
	{

		foreach (var target in targets) 
		{
			if ((int)enemy.GetGridPos ().x == (int)target.x && (int)enemy.GetGridPos ().y == (int)target.y)
			{
				return true;
			}
		}


		return false;
	}

	bool CheckWinCondition(GridItem start, List<Vector3> targets)
    {
		bool targetCanBeReached = false;

		foreach (var target in targets) 
		{
			reachableItems.Clear();
			GetReachableItems(start,gridItems[(int)target.x,(int)target.y]);
			if (reachableItems.Contains (gridItems [(int)target.x,(int)target.y])) 
			{
				targetCanBeReached = true;
				break;
			} 

		}

		if (targetCanBeReached) {
			return false;
		} else 
		{
			currentLevel += 1;
			RemoveListeners ();
			StartEndLevelAnimation ();	
			Invoke("GoToNextLevel",3);
			return true;
		}
			
	}


	bool IsTargetReachable(GridItem start, GridItem goal)
	{
		
		reachableItems.Clear ();
		GetReachableItems(start,goal);

		if (reachableItems.Contains (goal))
			{
				return true;
			} 
		return false;

	}


	void RemoveListeners ()
	{
		GridItem.ToggleTouch -= ItemTouched;
		UIController.RevertLastMove -= RevertLastMove;
	}

	void RemoveItemsListener ()
	{
		GridItem.ToggleTouch -= ItemTouched;
	}
	void AddItemsListener ()
	{
		GridItem.ToggleTouch += ItemTouched;
	}



	public GridItem[,] GetGridItems()
	{
		return gridItems;
	}

	public List<GridItem> GetPlayerMoves()
	{
		return playerMoves;
	}

	public List<Enemy> GetEnemies()
	{
		return enemies;
	}

}


public enum GameState
{
	Init,
	AwaitingInput,
	EnemyTurn
}
