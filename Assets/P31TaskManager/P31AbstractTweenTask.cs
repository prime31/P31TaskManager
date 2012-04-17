using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public abstract class P31AbstractTweenTask : P31AbstractTask
{
	public Func<float, float, float, float, float> ease;
	public P31UpdateType updateType;
	public P31LoopType loopType;
	public int loopCount;
	public float duration;
	
	private float _startTime;
	
	
	public P31AbstractTweenTask() : this( 0 )
	{}
	
	
	public P31AbstractTweenTask( float duration ) : this( duration, P31UpdateType.Update )
	{}
	
	
	public P31AbstractTweenTask( float duration, P31UpdateType updateType ) : this( duration, updateType, P31Easing.Bounce.EaseIn )
	{}
	
	
	public P31AbstractTweenTask( float duration, P31UpdateType updateType, Func<float,float,float,float,float> ease )
	{
		this.duration = duration;
		this.updateType = updateType;
		this.ease = ease;
	}

	
	public float getEasedPosition( float currentTime )
	{
		return ease( currentTime, 0, 1, duration );
	}
	
	
	public override void taskStarted()
	{
		base.taskStarted();
		
		_startTime = Time.time;
	}
	
	
	public override void tick()
	{
		var easePosition = getEasedPosition( Time.time );
		setValueForEasedPosition( easePosition );
		
		// are we done yet?
		if( ( _startTime + duration ) <= Time.time )
			state = P31TaskState.Complete;
	}
	
	
	public abstract void setValueForEasedPosition( float easePosition );

}