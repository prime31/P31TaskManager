using UnityEngine;
using System.Collections;


public class P31PositionTween : P31AbstractTweenTask
{
	private Transform _target;
	private Vector3 _startValue;
	private Vector3 _endValue;
	
	
	public P31PositionTween( Transform target, Vector3 endPosition, float duration ) : base( duration )
	{
		_target = target;
		_endValue = endPosition;
	}
	
	
	public override void setValueForEasedPosition( float easePosition )
	{
		_target.position = Vector3.Lerp( _startValue, _endValue, easePosition );
	}
	
	
	public override void taskStarted()
	{
		base.taskStarted();
		
		_startValue = _target.position;
	}

}
