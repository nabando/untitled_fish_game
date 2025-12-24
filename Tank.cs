using Godot;
using System;

public partial class Tank : Area2D
{
	PackedScene fishyScene = (PackedScene)ResourceLoader.Load("fishy.tscn"); //peewee
	Label fishCountLabel;
	int height = 648;
	int width = 1152;
	int fishCount = 0;
	public bool spawnModeHold = false;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Set shape of tank
		((RectangleShape2D)GetNode<CollisionShape2D>("CollisionShape2D").Shape).SetSize(new Vector2(width, height));
		
		// Initialize fish label
		fishCountLabel = (Label)GetNode<Label>("FishCount");
		fishCountLabel.AddThemeColorOverride("font_color", new Color(0, 0, 0, 1));
		fishCountLabel.Text = $"Fish Count: {fishCount}\nfps:"; //okay so when there's about 400 fish the fps dips to 5 LOL
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		fishCountLabel.Text = $"Fish Count: {fishCount}\nfps: {Engine.GetFramesPerSecond()}";
		
		if (Input.IsActionJustPressed("ToggleSpawnMode")) {
			spawnModeHold = !spawnModeHold;
			if (spawnModeHold) {
				GD.Print("Hold to spawn");
			}
			else {
				GD.Print("Click to spawn");
			}
		}
		
		if (Input.IsActionJustPressed("Click") && !spawnModeHold) {
			Vector2 relativeMousePos = GetGlobalMousePosition() - Position;
			if (!(relativeMousePos.X < 0 || relativeMousePos.Y < 0 || relativeMousePos.X > width || relativeMousePos.Y > height)) {
				//if click is within bounds of tank
				SpawnFish();
			}
		}
		if (Input.IsActionPressed("Click") && spawnModeHold) {
			Vector2 relativeMousePos = GetGlobalMousePosition() - Position;
			if (!(relativeMousePos.X < 0 || relativeMousePos.Y < 0 || relativeMousePos.X > width || relativeMousePos.Y > height)) {
				//if click is within bounds of tank
				SpawnFish();
				//fishCountLabel.Text = $"Fish Count: {fishCount}";
			}
		}
	}
	
	private void SpawnFish() {
		Fishy newFish = (Fishy)fishyScene.Instantiate();
		newFish.Position = GetGlobalMousePosition();
		GetNode<Node>("PeeWees").AddChild(newFish);
		fishCount++;
		
	}
	
	//signal
	private void BoidHasLeft(Rid body_rid, Node2D body, int BodyShapeIndex, int LocalShapeIndex) {
		Vector2 relativePosition =  body.Position - Position;
		int buffer = 50;
		
		//check left and right
		if (relativePosition.X > width) {
			//GD.Print("exit right");
			body.Position = new Vector2(relativePosition.X - width - buffer, body.Position.Y);
		}
		else if (relativePosition.X < 0) {
			//GD.Print("exit left");
			body.Position = new Vector2(relativePosition.X + width + buffer, body.Position.Y);
		}
		
		//check top and bottom
		// keep in mind the flip in logic since positive Y is down in godot
		if (relativePosition.Y < 0) {
			//GD.Print("exit top");
			body.Position = new Vector2(body.Position.X, relativePosition.Y + height + buffer);
		}
		else if (relativePosition.Y > height) {
			//GD.Print("exit bottom");
			body.Position = new Vector2(body.Position.X, relativePosition.Y - height - buffer);
		}
		
	}
}
