using UnityEngine;
using System.Collections;


public class PrintInBackgroundTask : P31AbstractBackgroundTask
{
	private int _count;
	
	
	public override void tick()
	{
		System.Threading.Thread.Sleep( 1000 );
		
		Debug.Log( "[PrintInBackground] thread is bg? " + System.Threading.Thread.CurrentThread.IsBackground );
		state = P31TaskState.Complete;
	}
}
