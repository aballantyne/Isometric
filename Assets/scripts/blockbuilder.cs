using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;  

public class blockbuilder : MonoBehaviour
{ 

  

	private new List<Vector2> avalable = new List<Vector2>();
  private new List<Vector2> unavalable = new List<Vector2>();
  float x = 5.0f;
	float y = 8.0f;
  float ax;
  float ay;

	Vector2 endPosition;
	Vector2 buildingPosition;
	Vector2 saved;

  public GameObject block;
  public GameObject storage;

  int avalableLenth;

  Vector2 topRight;
  Vector2 topLeft;
  Vector2 bottomRight;
  Vector2 bottomLeft;

  private bool workingtopRight;
  private bool workingtopLeft;
  private bool workingbottomRight;
  private bool workingbottomLeft;
  void Awake(){
    buildingPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    saved = buildingPosition;
    move();
    // this sets up all the start positions 
    ax =  0.5f;
    ay =  0.25f;
    topRight = new Vector2(ax, ay);
    ax = -0.5f;
    ay = 0.25f;
    topLeft = new Vector2(ax, ay);
    ax = 0.5f;
    ay = -0.25f;
    bottomRight = new Vector2(ax, ay);
    ax = -0.5f;
    ay = -0.25f;
    bottomLeft = new Vector2(ax, ay);
    avalable.Add(topRight);
    avalable.Add(topLeft);
    avalable.Add(bottomRight);
    avalable.Add(bottomLeft);

    unavalable.Add(new Vector2(0,0));
  }
  void Update()
  {
    buildingPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    if (buildingPosition != saved){
     saved = buildingPosition;
     move();
    }
    transform.position = endPosition;
    if (Input.GetMouseButtonDown(0)){
      GameObject obj = Instantiate(block, endPosition, Quaternion.identity);
      obj.transform.SetParent(storage.transform);
      
      listUpdate();
      move();
    }

    }
    static float distance(float x1, float y1, float x2, float y2){
   	float result = 0.0f;
       result = Mathf.Sqrt(((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1))) ; 
       return result; 
   }

   void move()
   {
    
   	Vector2 buildingPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
   	x = buildingPosition.x;
   	y = buildingPosition.y;
   	float min = 10000.0f;
    avalableLenth = avalable.Count;
   	for( int i = 0; i < avalableLenth; i++)
   	{

   		float d= distance(x,y, avalable[i].x, avalable[i].y); 
   		if(d < min){
        min = d;  
        endPosition = new Vector2(avalable[i].x, avalable[i].y);

    }
  }
  }
  void listUpdate(){

    workingtopRight = true;
    workingtopLeft = true;
    workingbottomRight = true;
    workingbottomLeft = true;

    unavalable.Add(endPosition);
    avalable.Remove(endPosition);
    avalable.Remove(endPosition);
    avalable.Remove(endPosition);
    avalable.Remove(endPosition);
    //This adds more places 
    ax = endPosition.x + 0.5f;
    ay = endPosition.y + 0.25f;
    topRight = new Vector2(ax, ay);
    ax = endPosition.x - 0.5f;
    ay = endPosition.y + 0.25f;
    topLeft = new Vector2(ax, ay);
    ax = endPosition.x + 0.5f;
    ay = endPosition.y - 0.25f;
    bottomRight = new Vector2(ax, ay);
    ax = endPosition.x - 0.5f;
    ay = endPosition.y - 0.25f;
    bottomLeft = new Vector2(ax, ay);
    int unavalableLength = unavalable.Count;
    
    for(int i = 0; i < unavalableLength; i++){
      
      if (nearlyEquals(topRight.x,unavalable[i].x) && nearlyEquals(topRight.y,unavalable[i].y)){
        workingtopRight = false;
        
      }
      if (nearlyEquals(topLeft.x,unavalable[i].x) && nearlyEquals(topLeft.y,unavalable[i].y)){
        workingtopLeft = false ;
        
      }
      if (nearlyEquals(bottomRight.x,unavalable[i].x) && nearlyEquals(bottomRight.y,unavalable[i].y)){
        workingbottomRight = false ;
        
      }
      if (nearlyEquals(bottomLeft.x,unavalable[i].x) && nearlyEquals(bottomLeft.y,unavalable[i].y)){
        workingbottomLeft = false;
        
      }

    }
    if (workingtopRight == true){
      avalable.Add(topRight);
    }
    if (workingtopLeft == true){
      avalable.Add(topLeft);
    }
    if (workingbottomRight == true){
      avalable.Add(bottomRight);
    }
    if (workingbottomLeft == true){
      avalable.Add(bottomLeft);
    }
      
  }

  bool nearlyEquals(float value1, float value2){
     double unimportantDifference = 0.0001;
        if (value1 != value2){
          return Math.Abs(value1 - value2) < unimportantDifference;
        }
        return true;
  }

}
  
