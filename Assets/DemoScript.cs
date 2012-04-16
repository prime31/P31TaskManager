using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Reflection;


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
	
	

	
	public float stuff;
	
	void Start()
	{
		var i = 1000000;
		
		var dir = direct( i );
		var val = getvalue( i );
		var del = delegatemethod( i );
		
		Debug.Log( "direct - val: " + ( val - dir ) );
		Debug.Log( "delegate - val: " + ( del - dir ) );
	}

	
	TimeSpan getvalue( int count )
	{
		ValueType valueSelf = this;
		var type = typeof( DemoScript );
		var field = type.GetField( "stuff", BindingFlags.Instance | BindingFlags.Public );
		
		
		var watch = new System.Diagnostics.Stopwatch();
		watch.Start();
		
		for( var i = 0; i < count; i++ )
		{
			field.SetValue( valueSelf, i );
		}
		
		watch.Stop();
		Debug.Log( "getvalue: " + watch.Elapsed );
		return watch.Elapsed;
	}
	
	
	TimeSpan delegatemethod( int count )
	{
		var methodInfo = typeof( GameObjectExtensions ).GetMethod( "setPosition", BindingFlags.Public | BindingFlags.Static );
		Action<Transform, Vector3> fixedMethod = (Action<Transform, Vector3>)Delegate.CreateDelegate( typeof( Action<Transform, Vector3> ), methodInfo );
		var vec = new Vector3( 1, 1, 1 );
		var transform = gameObject.transform;
		
		var watch = new System.Diagnostics.Stopwatch();
		watch.Start();
		
		for( var i = 0; i < count; i++ )
		{
			fixedMethod( transform, vec );
		}
		
		watch.Stop();
		Debug.Log( "delegatemethod: " + watch.Elapsed );
		return watch.Elapsed;
	}
	
	
	TimeSpan direct( int count )
	{
		var watch = new System.Diagnostics.Stopwatch();
		watch.Start();
		
		for( var i = 0; i < count; i++ )
		{
			stuff = i;
		}
		
		watch.Stop();
		Debug.Log( "direct: " + watch.Elapsed );
		return watch.Elapsed;
	}

}


public static class GameObjectExtensions
{
	public static void setPosition( this Transform self, Vector3 position )
	{
		self.position = position;
	}
}
