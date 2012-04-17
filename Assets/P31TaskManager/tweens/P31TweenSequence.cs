using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class P31TweenSequence : P31AbstractTweenTask 
{
	private List<P31AbstractTweenTask> _properties = new List<P31AbstractTweenTask>();
	
	
	public void addProperty( P31AbstractTweenTask tween )
	{
		_properties.Add( tween );
	}
	
	
	public override void setValueForEasedPosition( float easePosition )
	{
		
	}
}
