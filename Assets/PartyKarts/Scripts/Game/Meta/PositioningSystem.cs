using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

/// <summary>
/// PositioningSystem for create points and control all cars position in runtime.
/// TODO calculate cars anteriority.
/// </summary>
public class PositioningSystem :MonoBehaviour
{
	[SerializeField] WaypointCircuit m_PositioningAndAiPath;		
	[SerializeField] WaypointCircuit m_PathForVisual;

	static public PositioningSystem Instance;
	static public WaypointCircuit PositioningAndAiPath { get { return Instance.m_PositioningAndAiPath; } }
	static public int LapsCount { get; private set; }

	public WaypointCircuit GetPathForVisual { get { return m_PathForVisual; } }

	static public List<PositioningCar> OrderedCars;				//For order cars positions in race.

	private void Awake ()
	{
		Instance = this;
		LapsCount = WorldLoading.LapsCount;
	}

	void Start ()
	{
		OrderedCars = new List<PositioningCar> ();
		foreach (var car in GameController.AllCars)
		{
			OrderedCars.Add (car.PositioningCar);
		}
	}

	void Update ()
	{
		SortCars ();
	}

	void SortCars ()
	{
		OrderedCars.Sort (new CarsComparer ());
	}

	static public int GetCarPos (PositioningCar car)
	{
		return OrderedCars.IndexOf (car);
	}

	static public int GetCarPos (CarController car)
	{
		return GetCarPos (car.PositioningCar);
	}

	private class CarsComparer :IComparer<PositioningCar>
	{
		public int Compare (PositioningCar x, PositioningCar y)
		{
			if (x.ProgressDistance > y.ProgressDistance)
			{
				return -1;
			}
			else
			{
				return 1;
			}
		}
	}
}
