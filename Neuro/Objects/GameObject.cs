using System.Collections.Generic;
using DXPrimitiveFramework;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace Neuro.Objects
{
	public abstract class GameObject
	{
		public GameObject()
		{
		}

		public Primitive Primitive { get; protected set; }

		public Vector2 Position
		{
			get { return Primitive.Position; }
			set { Primitive.Position = value; }
		}

		public abstract void Draw(SpriteBatch spriteBatch);

		public abstract void Update(GameTime gameTime);
	}
}
