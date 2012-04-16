using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class P31TaskManager : MonoBehaviour
{
	private List<P31AbstractTask> _taskList = new List<P31AbstractTask>();
	private Queue<P31AbstractTask> _completedTaskQueue = new Queue<P31AbstractTask>();
	private bool _isRunningTasks = false;
	
	private List<P31AbstractBackgroundTask> _backgroundTaskList = new List<P31AbstractBackgroundTask>();
	private bool _isRunningBackgroundTasks = false;
	
	
	// only one P31TaskManager can exist
	static P31TaskManager _instance = null;
	public static P31TaskManager instance
	{
		get
		{
			if( !_instance )
			{
				// check if an P31TaskManager is already available in the scene graph
				_instance = FindObjectOfType( typeof( P31TaskManager ) ) as P31TaskManager;

				// nope, create a new one
				if( !_instance )
				{
					var obj = new GameObject( "P31TaskManager" );
					_instance = obj.AddComponent<P31TaskManager>();
				}
			}

			return _instance;
		}
	}
	
	
	void OnApplicationQuit()
	{
		// release reference on exit
		_instance = null;
	}
	
	
	/// <summary>
	/// adds a task to be run on the main thread
	/// </summary>
	public void addTask( P31AbstractTask task, params P31AbstractTask[] otherTasks )
	{
		_taskList.Add( task );
		
		foreach( var t in otherTasks )
			_taskList.Add( t );
		
		// if our update loop isnt running start it up
		if( !_isRunningTasks )
			StartCoroutine( processTasks() );
	}
	
	
	/// <summary>
	/// adds a task to be run on a worker thread
	/// </summary>
	public void addBackgroundTask( P31AbstractBackgroundTask task )
	{
		_backgroundTaskList.Add( task );
		
		// if our update loop isnt running start it up
		if( !_isRunningBackgroundTasks )
			StartCoroutine( processBackgroundTasks() );
	}

	
	/// <summary>
	/// runs through all current tasks (when there are some) and calls their tick method
	/// </summary>
	private IEnumerator processTasks()
	{
		_isRunningTasks = true;
		
		// keep the loop running as long as we have tasks to run
		while( _taskList.Count > 0 )
		{
			foreach( var task in _taskList )
			{
				// if the task is not running, prepare it. taskStarted could set a task to running
				// so this needs to be the first item in the loop
				if( task.state == P31TaskState.NotRunning )
					task.taskStarted();
				
				// tick any tasks that need to run
				if( task.state == P31TaskState.Running )
					task.tick();
				
				// prepare to clear out any tasks that are completed or cancelled
				if( task.state == P31TaskState.Complete || task.state == P31TaskState.Canceled )
					_completedTaskQueue.Enqueue( task );
			}
			
			// done running our tasks so lets clear out the completed queue now
			if( _completedTaskQueue.Count > 0 )
			{
				foreach( var task in _completedTaskQueue )
				{
					// we call taskCompleted here so that it can safely modify the task list
					task.taskCompleted();
					_taskList.Remove( task );
				}
				_completedTaskQueue.Clear();
			}
			
			yield return null;
		}
		
		_isRunningTasks = false;
	}
	
	
	/// <summary>
	/// runs through all background tasks (when there are some) and manages their life cycle
	/// </summary>
	private IEnumerator processBackgroundTasks()
	{
		_isRunningBackgroundTasks = true;
		var completedQueue = new Queue<P31AbstractBackgroundTask>();
		
		// keep the loop running as long as we have tasks to run
		while( _backgroundTaskList.Count > 0 )
		{
			foreach( var task in _backgroundTaskList )
			{
				// if the task is not running, prepare it. taskStarted could set a task to running
				// so this needs to be the first item in the loop
				if( task.state == P31TaskState.NotRunning )
					task.taskStarted();
				
				// tick any tasks that need to run
				if( task.state == P31TaskState.Running )
					System.Threading.ThreadPool.QueueUserWorkItem( ( obj ) =>
					{
						task.tick();
					} );
				
				// prepare to clear out any tasks that are completed
				if( task.state == P31TaskState.Complete )
					completedQueue.Enqueue( task );
			}
			
			// done running our tasks so lets clear out the completed queue now
			if( completedQueue.Count > 0 )
			{
				foreach( var t in completedQueue )
				{
					// we call taskCompleted here so that it can safely modify the task list
					t.taskCompleted();
					_backgroundTaskList.Remove( t );
				}
				completedQueue.Clear();
			}
			
			yield return null;
		}
		
		_isRunningBackgroundTasks = false;
	}

}
