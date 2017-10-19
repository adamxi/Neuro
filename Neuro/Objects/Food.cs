using DXPrimitiveFramework;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace Neuro.Objects
{
	public class Food : GameObject
	{
		public Food()
		{
			Primitive = new PCircle(0, 0, 5, 2) { Color = Color.Purple };
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			Primitive.Draw();
		}

		public override void Update(GameTime gameTime)
		{
		}
	}
}