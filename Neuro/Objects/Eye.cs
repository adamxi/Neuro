  using System.Linq;
using DXPrimitiveFramework;
using Neuro.NS;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using DXFramework.Util;
using System.Collections.Generic;

namespace Neuro.Objects
{
	public class Eye : GameObject
	{
		private float los;
		private float fov;
		private Bug bug;
		private List<Neuron> neurons;

		private Primitive viewPrimitive;
		private Primitive eyePrimitive;

		public Eye(Bug bug)
		{
			this.bug = bug;
			los = 40;
			fov = 90;

			eyePrimitive = new PCircle(0, 0, 2, true) { Color = Color.Black };
			viewPrimitive = new PArc(0, 0, los, fov, 1) { Color = Color.Red };

			var cp = new CompoundPrimitive();
			cp.Add(viewPrimitive);
			cp.Add(eyePrimitive);
			Primitive = cp;

			neurons = bug.Brain.GetInputNeurons(1);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
		}

		public override void Update(GameTime gameTime)
		{
			//neurons.ForEach(n =>
			//{
			//	n.Depolarize(Randomizer.Range(45, 200) * gameTime.ElapsedGameTime.TotalSeconds);
			//	n.CheckActivation();
			//});

			FoodManager.FoodObjects.ForEach(f =>
			{
				if (viewPrimitive.Intersects(f.Position))
				{
					FoodManager.Remove(f);
				}
			});
		}
	}
}