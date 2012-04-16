using UnityEngine;
using System.Collections;


public class PrintTask : P31AbstractTask
{
	private int _count;
	
	
	public override void tick()
	{
		if( _count == 1 )
			state = P31AbstractTask.TaskState.Complete;
		_count++;
		
		Debug.Log( "[PrintTask] count: " + _count );
	}
}
