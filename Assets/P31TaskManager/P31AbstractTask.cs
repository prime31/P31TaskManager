using UnityEngine;
using System;
using System.Collections;



public abstract class P31AbstractTask
{
	public enum TaskState
	{
		NotRunning,
		Running,
		Paused,
		Canceled,
		Complete
	}
	public TaskState state; // the tasks current state
	public float delay; // delay when starting the task in seconds
	public P31AbstractTask nextTask;
	
	public object userInfo; // random bucket for data associated with the task
	public Action<P31AbstractTask> completionHandler;
	
	
	private float _elapsedTime; // timer used internally for NOTHING RIGHT NOW!
	public float elapsedTime { get { return _elapsedTime; } }
	
	
	/// <summary>
	/// subclasses should override this and set state to Complete when done
	/// </summary>
	public abstract void tick();
	
	
	/// <summary>
	/// reset all state before we start the task
	/// </summary>
	public virtual void resetState()
	{
		_elapsedTime = 0;
		state = TaskState.NotRunning;
	}
	
	
	/// <summary>
	/// called when the task is started. allows setup/cleanup to occur and delays to be used
	/// </summary>
	public virtual void taskStarted()
	{
		resetState();
		
		// if we are delayed then set ourself as paused then unpause after the delay
		if( delay > 0 )
		{
			state = TaskState.Paused;
		
			var delayInMilliseconds = (int)( delay * 1000 );
			new System.Threading.Timer( obj =>
			{
				lock( this )
				{
					state = TaskState.Running;
				}
			}, null, delayInMilliseconds, System.Threading.Timeout.Infinite );
		}
		else
		{
			// start immediately
			state = TaskState.Running;
		}
	}
	
	
	/// <summary>
	/// called when the task is completed
	/// </summary>
	public void taskCompleted()
	{
		// fire off our completion handler if we have one
		if( completionHandler != null )
			completionHandler( this );
		
		// if we have a next task to run and we were not cancelled, start it
		if( nextTask != null && state != TaskState.Canceled )
			P31TaskManager.instance.addTask( nextTask );
	}
	
	
	/// <summary>
	/// cancelling a task stops it immediately and causes its nextTask to not be executed
	/// </summary>
	public void cancel()
	{
		state = TaskState.Canceled;
	}
	
	
	public void pause()
	{
		state = TaskState.Paused;
	}
	
	
	public void unpause()
	{
		state = TaskState.Running;
	}
	
}