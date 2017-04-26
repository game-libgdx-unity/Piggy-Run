using UnityEngine;
using System.Collections;

public class Path : MonoBehaviour { 

	public RoadGenerator Road;
	public bool NeedJump; 
	public bool NeedLongJump;
    public Direction GetDirection;
    internal bool needAction;
    public SpriteRenderer spriteR;
}
