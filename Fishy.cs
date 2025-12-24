using Godot;
using System;
using System.Collections.Generic;

public partial class Fishy : CharacterBody2D
{
	public const float Speed = 100.0f; 
	public float Dir;
	private Area2D PerceptionRange;
	private Area2D TooCloseRange;
	private Fishy[] NearbyFishies;
	private Fishy[] TooCloseFishies;
	
	public override void _Ready()
	{
		// When a fish spawns
		PerceptionRange = (Area2D)GetNode<Area2D>("PerceptionRange");
		TooCloseRange = (Area2D)GetNode<Area2D>("TooCloseBackAwayPls");
		RandomNumberGenerator rng = new RandomNumberGenerator();
		Dir = rng.RandfRange(0, Mathf.Pi * 2);
		Velocity = new Vector2(Mathf.Cos(Dir), Mathf.Sin(Dir)) * Speed;
	}
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		Vector2 newVel = velocity;
		
		if (PerceptionRange.GetOverlappingBodies().Count > 1) {
			//GD.Print($"hi {PerceptionRange.GetOverlappingBodies().Count}");
			//GD.Print(PerceptionRange.GetOverlappingBodies()[0].GetClass());
			NearbyFishies = convertWeirdToArr(PerceptionRange.GetOverlappingBodies());
			//GD.Print(Alignment(NearbyFishies));
			//newVel = Alignment(NearbyFishies);
			newVel = Cohesion(NearbyFishies);
			newVel = newVel.Normalized() * Speed;
			velocity = velocity.Lerp(newVel, 0.05f);
		}
		
		//TooCloseFishies = convertWeirdToArr(TooCloseRange.GetOverlappingBodies());
		
		
		
		Rotation = Mathf.Atan2(velocity.Y, velocity.X); //face direction its moving in
		Velocity = velocity;
		MoveAndSlide();
	}
	
	//follow da mouse cursor !
	private Vector2 followMe() {
		return (GetGlobalMousePosition() - Position).Normalized() * Speed;
	}
	
	private Vector2 Alignment(Fishy[] otherFishies) {
		Vector2 avgDir = new Vector2(0, 0);
		
		for (int i = 0; i < otherFishies.Length; i++) {
			avgDir += otherFishies[i].Velocity;
		}
		avgDir /= otherFishies.Length;
		avgDir = avgDir.Normalized() * Speed;
		
		return avgDir;
	}
	
	private Vector2 Cohesion(Fishy[] otherFishies) {
		Vector2 centerPt = new Vector2(0, 0);
		Vector2 dirTowardsCenter = new Vector2(0, 0);
		
		// get average position (center) of nearby fish (not including self)
		for (int i = 0; i < otherFishies.Length; i++) {
			// change this to relative position
			centerPt += otherFishies[i].Position; 
		}
		// create a vector pointing towards the center
		dirTowardsCenter = centerPt.Normalized() * Speed;
		
		return dirTowardsCenter;
	}
	
	private Vector2 Separation(Fishy[] tooCloseFishies) {
		Vector2 centerPt = new Vector2(0, 0);
		Vector2 dirFromCenter = new Vector2(0, 0);
		
		// get average position (center) of too close fish (not including self)
		for (int i = 0; i < tooCloseFishies.Length; i++) {
			// change this to relative position
			centerPt += tooCloseFishies[i].Position; 
		}
		centerPt /= tooCloseFishies.Length;
		
		// create a vector pointing away from the center
		dirFromCenter = -centerPt.Normalized();
		
		return dirFromCenter;
	}
	
	private Fishy[] convertWeirdToArr(Godot.Collections.Array<Godot.Node2D> weird) {
		Fishy[] fishyArr = new Fishy[weird.Count];
		for (int i = 0; i < weird.Count; i++) {
			fishyArr[i] = (Fishy)weird[i];
		}
		return fishyArr;
	}
}
