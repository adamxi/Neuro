using System.Linq;
using DXPrimitiveFramework;
using Neuro.NS;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace Neuro.Objects
{
	public class Motor : GameObject
	{
		private Bug bug;

		public Motor(Bug bug)
		{
			this.bug = bug;

			Primitive = new PCircle(0, 0, 3, true) { Color = Color.Yellow };

			Neuron = bug.Brain.GetOutputNeurons(1).FirstOrDefault();
		}

		public Neuron Neuron { get; set; }

		public float Force { get; set; }

		public override void Draw(SpriteBatch spriteBatch)
		{
		}

		public override void Update(GameTime gameTime)
		{
			//Force = (float)Neuron.PopCharge();
		}
	}
}