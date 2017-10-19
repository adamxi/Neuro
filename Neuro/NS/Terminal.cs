using SharpDX;
using SharpDX.Toolkit;
using System;

namespace Neuro.NS
{
	public class Terminal
	{
		public static int TotalSynapses { get; set; }

		public Terminal(Neuron neuron)
		{
			Neuron = neuron;
			Sensitivity = 1;
		}

		public Neuron Neuron { get; set; }

		public double Sensitivity { get; set; }

		public void DoSynapse(double charge)
		{
			TotalSynapses++;
			//Sensitivity = MathUtil.Clamp((float)(Sensitivity + charge * 0.01d), -0.999f, 0.999f);
			//Sensitivity += charge * 0.001d;
			var transmitterRelease = charge * Sensitivity;
			Neuron.Depolarize(transmitterRelease);
		}

		public void Update(GameTime gameTime)
		{
			//Sensitivity = Math.Max(Sensitivity - gameTime.ElapsedGameTime.TotalSeconds * 0.0001d, 0);
		}

		public override int GetHashCode()
		{
			return Neuron.GetHashCode();
		}
	}
}