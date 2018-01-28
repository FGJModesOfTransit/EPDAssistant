using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveResults 
{
	public int WaveNumber { get; set; }
	public string DiseaseName { get; set; }
	public List<int> SavedNodePopulations { get; set; }
	public List<int> LostNodePopulations { get; set; }
	public int Infections { get; set; }
}
