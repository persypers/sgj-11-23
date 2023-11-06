using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelController : Fancy.MonoSingleton< FuelController >
{
	public float fuel = 10.0f;
	public float fuelBurnPerSecond = 0.02f;

	public void AddFuel( float amount )
	{
		fuel += amount;
	}

	void Update()
	{
		
	}
}
