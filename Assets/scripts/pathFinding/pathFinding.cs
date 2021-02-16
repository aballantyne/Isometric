using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

public class pathFinding : MonoBehaviour
{
	private const int MoveCost = 10;
	
    // Start is called before the first frame update
    private void Start(){
    	FindPath(new int2(0,0), new int2(3,1));
    }
    private void FindPath(int2 startingPostion, int2 endPostion)
    {
        int2 gridSize = new int2(4,4); 

        NativeArray<PathNode> pathNodeArray = new NativeArray<PathNode>(gridSize.x * gridSize.y, Allocator.Temp);
        for (int x = 0; x<gridSize.x; x++){
        	for (int y = 0; y<gridSize.y; y++){
        		PathNode pathNode = new PathNode();
        		pathNode.x = x;
        		pathNode.y = y;
        		pathNode.Index = CalculateIndex(x,y, gridSize.x);

        		pathNode.gCost = int.MaxValue;
        		pathNode.hCost = CalculateDistanceCost(new int2(x,y), endPostion);
        		pathNode.CalculateFcost();

        		pathNode.isWalkble = true;
        		pathNode.cameFromNodeIndex = -1;

        		pathNodeArray[pathNode.Index] = pathNode;

        	}
        }
        NativeArray<int2> neighborOffsetArray = new  NativeArray<int2>(new int2[] {
        	new int2(-1, 0), 
        	new int2(+1, 0),
        	new int2(0, +1),
        	new int2(0, -1), 
        }, Allocator.Temp);

        int endNodeIndex = CalculateIndex(endPostion.x, endPostion.y,gridSize.x);
        PathNode startNode = pathNodeArray[CalculateIndex(startingPostion.x,startingPostion.y, gridSize.x)];
        startNode.gCost = 0;
        startNode.CalculateFcost();
        pathNodeArray[startNode.Index] = startNode;

        NativeList<int> openList = new NativeList<int>(Allocator.Temp);
        NativeList<int> closedList = new NativeList<int>(Allocator.Temp);

        openList.Add(startNode.Index);

        while (openList.Length > 0){
        	int currentNodeIndex = GetLowestCostFNodeIndex(openList, pathNodeArray);
        	PathNode currentNode = pathNodeArray[currentNodeIndex];

        	if(currentNodeIndex == endNodeIndex){
        		// we are at the end
        		break;
        	}
        	// remove current
        	for (int i = 0; i < openList.Length; i++){
        		if (openList[i] == currentNodeIndex){
        			openList.RemoveAtSwapBack(i);
        			break;
        		}
        	}
        	closedList.Add(currentNodeIndex);

        	for (int i = 0; i < neighborOffsetArray.Length; i++) {
        		int2 neighborOffset = neighborOffsetArray[i];
        		int2 neighborPosition = new int2(currentNode.x + neighborOffset.x, currentNode.y + neighborOffset.y);
        		if (!IsPositionInsiderGrid(neighborPosition, gridSize)){
        			// Not valid
        			continue;
        		}
        		int neighborNodeIndex = CalculateIndex(neighborPosition.x, neighborPosition.y, gridSize.x);

        		if (closedList.Contains(neighborNodeIndex)){
        			//Searched already
        			continue;
        		}

        		PathNode neighborNode = pathNodeArray[neighborNodeIndex];
        		if (!neighborNode.isWalkble){
        			//not walkable
        			continue;
        		}

        		int2 currentNodePos = new int2(currentNode.x, currentNode.y);

        		int tentaiveGCost = currentNode.gCost + CalculateDistanceCost(currentNodePos, neighborPosition);
        		if (tentaiveGCost < neighborNode.gCost){
        			neighborNode.cameFromNodeIndex = currentNodeIndex;
        			neighborNode.gCost = tentaiveGCost;
        			neighborNode.CalculateFcost();
        			pathNodeArray[neighborNodeIndex] = neighborNode;

        			if (!openList.Contains(neighborNode.Index)){
        				openList.Add(neighborNode.Index);
        			}
        		}

        	}

        }

        PathNode endNode= pathNodeArray[endNodeIndex]; 

        if (endNode.cameFromNodeIndex == -1){
        	//no
        	Debug.Log("didnt Find it");
        }else{
        	//yes

        	NativeList<int2> path = CalculatePath(pathNodeArray, endNode);
        	foreach (int2 pathPosition in path){
        		Debug.Log(pathPosition);
        	}
        	path.Dispose();
        }

        pathNodeArray.Dispose();
        neighborOffsetArray.Dispose();
        openList.Dispose();
        closedList.Dispose();
    }
    private NativeList<int2> CalculatePath(NativeArray<PathNode> pathNodeArray, PathNode endNode) {
            if (endNode.cameFromNodeIndex == -1) {
                // Couldn't find a path!
                return new NativeList<int2>(Allocator.Temp);
            } else {
                // Found a path
                NativeList<int2> path = new NativeList<int2>(Allocator.Temp);
                path.Add(new int2(endNode.x, endNode.y));

                PathNode currentNode = endNode;
            while (currentNode.cameFromNodeIndex != -1) {
                PathNode cameFromNode = pathNodeArray[currentNode.cameFromNodeIndex];
                path.Add(new int2(cameFromNode.x, cameFromNode.y));
                currentNode = cameFromNode;
            }

            return path;
       }
    }   
    private bool IsPositionInsiderGrid(int2 gridPosition, int2 gridSize){
    	return 
    	   gridPosition.x >= 0 &&
    	   gridPosition.y >= 0 &&
    	   gridPosition.x < gridSize.x &&
    	   gridPosition.y < gridSize.y;
    }
    private int CalculateIndex(int x, int y, int gridWidth){
    	return x + y * gridWidth;
    }
    private int CalculateDistanceCost(int2 a, int2 b){
    	int xDistance = math.abs(a.x - b.x);
    	int yDistance = math.abs(a.y - b.y);
    	int remaining = math.abs(xDistance-yDistance);
    	return MoveCost * remaining;

    }
    private int GetLowestCostFNodeIndex(NativeList<int> openList, NativeArray<PathNode> pathNodeArray) {
            PathNode lowestCostPathNode = pathNodeArray[openList[0]];
            for (int i = 1; i < openList.Length; i++) {
                PathNode testPathNode = pathNodeArray[openList[i]];
                if (testPathNode.fCost < lowestCostPathNode.fCost) {
                    lowestCostPathNode = testPathNode;
                }
            }
            return lowestCostPathNode.Index;
        }
    private struct PathNode {
    	public int x;
    	public int y;

    	public int Index;

    	public int gCost;
    	public int hCost;
    	public int fCost;

    	public bool isWalkble;

    	public int cameFromNodeIndex;

    	public void CalculateFcost() {
    		fCost = gCost + hCost;
    	}
    }
}
