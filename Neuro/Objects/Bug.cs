using DXFramework;
using DXPrimitiveFramework;
using Neuro.NS;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;

namespace Neuro.Objects
{
	public class Bug : GameObject
	{
		private Eye eyeLeft;
		private Eye eyeRight;
		private Motor leftMotor;
		private Motor rightMotor;
		private Primitive body;

		public Bug() : base()
		{
			Brain = new Brain(302);
			Initialize();
		}

		public Brain Brain { get; private set; }

		private void Initialize()
		{
			eyeLeft = new Eye(this);
			eyeLeft.Position = new Vector2(-5, -5);

			eyeRight = new Eye(this);
			eyeRight.Position = new Vector2(5, -5);

			leftMotor = new Motor(this);
			leftMotor.Position = new Vector2(-5, 0);

			rightMotor = new Motor(this);
			rightMotor.Position = new Vector2(5, 0);

			body = new PCircle(0, 0, 5, true) { Color = Color.Green };

			var cp = new CompoundPrimitive();
			cp.Add(leftMotor.Primitive);
			cp.Add(rightMotor.Primitive);
			cp.Add(body);
			cp.Add(eyeLeft.Primitive);
			cp.Add(eyeRight.Primitive);
			cp.Origin = body.GetCentroid();
			cp.Scale += 2;

			Primitive = cp;

			//Brain.InitializeConnections();
        }

		public override void Draw(SpriteBatch spriteBatch)
		{
			Primitive.Draw();
		}

		public override void Update(GameTime gameTime)
		{
			Brain.Update(gameTime);

			eyeLeft.Update(gameTime);
			eyeRight.Update(gameTime);
			leftMotor.Update(gameTime);
			rightMotor.Update(gameTime);

			//Primitive.Degrees += 0.1f;
			if (InputManager.Pressed(MouseButton.Right))
			{
				Primitive.Position = MainScene.cam.CursorPosition;
            }
			if (InputManager.Held(MouseButton.Right))
			{
				Primitive.Position += InputManager.MouseDelta;
			}

			FoodManager.FoodObjects.ForEach(f =>
			{
				if (body.Intersects(f.Position))
				{
					FoodManager.Remove(f);
				}
			});

			if (InputManager.Held(Keys.Q))
			{
				Primitive.Degrees -= 1f;
			}
			if (InputManager.Held(Keys.E))
			{
				Primitive.Degrees += 1f;
			}

			//Vector2 vector = new Vector2(leftMotor.Force, rightMotor.Force);
			//Primitive.Degrees = MathUtil.RadiansToDegrees(TriangleHelper2D.VectorToRadian(vector));

			//Position += vector;
		}
	}
}