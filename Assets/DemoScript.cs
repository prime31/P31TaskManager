using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.ComponentModel;


public class DemoScript : MonoBehaviour
{
	private int _counter;
	

	void OnGUI()
	{
		if( GUILayout.Button( "Print Task" ) )
		{
			var t = new PrintTask();
			t.completionHandler = ( task ) => { Debug.Log( "DONE with first" ); };
			
			var t2 = new PrintTask();
			t2.completionHandler = ( task ) => { Debug.Log( "DONE with second" ); };
			
			t.nextTask = t2;
			
			P31TaskManager.instance.addTask( t );
		}
		
		
		if( GUILayout.Button( "Action Task" ) )
		{
			_counter = 0;
			P31ActionTask.createAndStartTask( demoActionTask );
		}
		
		
		if( GUILayout.Button( "Background Task" ) )
		{
			var t = new PrintInBackgroundTask();
			t.completionHandler = ( task ) =>
			{
				Debug.Log( "[PrintInBackground] thread is bg? " + System.Threading.Thread.CurrentThread.IsBackground );
			};
			P31TaskManager.instance.addBackgroundTask( t );
		}
	}
	
	
	bool demoActionTask()
	{
		if( _counter == 5 )
			return false;
		
		_counter++;
		Debug.Log( "demoActionTask counter: " + _counter );

		return true;
	}
}
