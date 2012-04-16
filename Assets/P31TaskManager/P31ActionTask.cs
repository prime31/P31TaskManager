using UnityEngine;
using System.Collections;
using System;


/// <summary>
/// simple task that will call a function as its tick method continuously until it returns false
/// </summary>
public class P31ActionTask : P31AbstractTask
{
	private Func<bool> _action;
	
	
	public static P31ActionTask createAndStartTask( Func<bool> action )
	{
		return createAndStartTask( action, null );
	}
	
		
	public static P31ActionTask createAndStartTask( Func<bool> action, Action<P31AbstractTask> completionHandler )
	{
		var actionTask = new P31ActionTask( action );
		actionTask.completionHandler = completionHandler;
		P31TaskManager.instance.addTask( actionTask );
		return actionTask;
	}
	
	
	public P31ActionTask( Func<bool> action )
	{
		_action = action;
	}
	
	
	public override void tick()
	{
		if( !_action() )
			state = P31TaskState.Complete;
	}

}
