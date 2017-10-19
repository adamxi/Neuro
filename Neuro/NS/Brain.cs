using System.Collections.Generic;
using System.Linq;
using DXFramework.Util;
using DXPrimitiveFramework;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using System;

namespace Neuro.NS
{
	public class Brain
	{
		public static bool drawDebugLines = false;

		private Primitive[][] connectionToLine;
		private Primitive[][] connectionToArrow;
		private float elapsedTime;
		private List<Neuron> disposed;

		public Brain(int initialNeurons)
		{
			connectionToLine = new Primitive[initialNeurons][];
			connectionToArrow = new Primitive[initialNeurons][];

			for (int i = 0; i < initialNeurons; i++)
			{
				connectionToLine[i] = new Primitive[initialNeurons];
				connectionToArrow[i] = new Primitive[initialNeurons];
			}

			Radius = initialNeurons * 1f;

			disposed = new List<Neuron>();
			Neurons = new List<Neuron>(initialNeurons);
			OutputNeurons = new List<Neuron>();
			InputNeurons = new List<Neuron>();

			for (int i = initialNeurons; --i >= 0;)
			{
				Neurons.Add(new Neuron(this) { Id = i });
			}
			return;

			var xOff = 200;
			int yOff = 200;

			Neurons.AddRange(new List<Neuron>() {
				new Neuron(this) { Position = new Vector2(0, 0) },
				new Neuron(this) { Position = new Vector2(-xOff, yOff) },
				new Neuron(this) { Position = new Vector2(xOff, yOff) },
				new Neuron(this) { Position = new Vector2(-xOff, yOff * 2) },
				//new Neuron(this) { Position = new Vector2(xOff, yOff * 2) }
			});
		}

		public void PrimitiveConnect(Neuron from, Neuron to)
		{
			Neuron a;
			Neuron b;

			if (from.Id > to.Id)
			{
				a = from;
				b = to;
			}
			else
			{
				a = to;
				b = from;
			}

			var slope = Vector2.Normalize(from.Position - to.Position);
			var lineOffset = slope * Neuron.neuronRadius;
			var line = new PLine(from.Position - lineOffset, to.Position + lineOffset, 2) { Color = Color.Black };
			connectionToLine[a.Id][b.Id] = line;


			var endShape = new PTriangle(8, 5, true) { Color = Color.Black };
			endShape.Origin = new Vector2(0, -10);
			endShape.Position = to.Position;// - lineOffset - slope * (6 + Primitive.Thickness);
			endShape.Degrees = MathUtil.RadiansToDegrees(TriangleHelper2D.VectorToRadian(line.Slope));
			connectionToArrow[from.Id][to.Id] = endShape;
		}

		public void PrimitiveRemove(Neuron from, Neuron to)
		{
			Neuron a;
			Neuron b;

			if (from.Id > to.Id)
			{
				a = from;
				b = to;
			}
			else
			{
				a = to;
				b = from;
			}

			connectionToArrow[from.Id][to.Id] = null;

			if (connectionToArrow[to.Id][from.Id] == null)
			{
				connectionToLine[from.Id][to.Id] = null;
				connectionToLine[to.Id][from.Id] = null;
			}
		}

		public float Radius { get; private set; }

		public List<Neuron> Neurons { get; private set; }

		public List<Neuron> OutputNeurons { get; private set; }

		public List<Neuron> InputNeurons { get; private set; }

		public List<Neuron> GetOutputNeurons(int count)
		{
			var neurons = Neurons.Except(InputNeurons).Except(OutputNeurons).SelectRandom(count).ToList();
			neurons.ForEach(n => n.Primitive.Filled = true);

			OutputNeurons.AddRange(neurons);
			return neurons;
		}

		public List<Neuron> GetInputNeurons(int count)
		{
			var neurons = Neurons.Except(InputNeurons).Except(OutputNeurons).SelectRandom(count).ToList();
			neurons.ForEach(n => n.Primitive.Thickness += 4);

			InputNeurons.AddRange(neurons);
			return neurons;
		}

		public void Remove(Neuron neuron)
		{
			disposed.Add(neuron);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			Neurons.ForEach(n => n.Draw(spriteBatch));
			if (drawDebugLines)
			{
				int len = connectionToLine[0].Length;
				for (int x = 0; x < len; x++)
				{
					for (int y = 0; y < len; y++)
					{
						connectionToLine[x][y]?.Draw();
						connectionToArrow[x][y]?.Draw();
					}
				}
			}
		}

		public void Update(GameTime gameTime)
		{
			Neurons.ForEach(n => n.Update(gameTime));

			elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

			if (elapsedTime >= 1000)
			{
				MainScene.uiManager.SetDebugValue("Synapses", Terminal.TotalSynapses.ToString());
				Terminal.TotalSynapses = 0;
				elapsedTime = 0;
			}

			if (disposed.Count > 0)
			{
				disposed.ForEach(n => Neurons.Remove(n));
				disposed.Clear();
			}
		}
	}
}