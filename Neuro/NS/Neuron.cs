using DXFramework.Util;
using DXPrimitiveFramework;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neuro.NS
{
	public class Neuron
	{
		public static bool drawDebugText = false;

		private static int nextId = 0;
		private const int SAMPLE_INTERVAL_MS = 1000;

		private Brain brain;

		private int excitationCounter;
		private float elapsedTime;
		private double stateElapsed;
		public const float neuronRadius = 8;
		private NeuronState state;
		public double maxConnectionRange;

		public Neuron(Brain brain)
		{
			this.Id = nextId++;
			this.brain = brain;

			Primitive = new PCircle(Vector2.Zero, neuronRadius, 2) { Color = Color.Black };
			this.Position = Randomizer.Range(Vector2.Zero, brain.Radius);

			Charge = Const.MV_RESTING;
			Threashold = Const.MV_THREASHOLD;
			TerminalConnections = new Dictionary<Neuron, Terminal>();
			DendriteConnections = new HashSet<Neuron>();

			MaxConnectionRange = Math.Sqrt(brain.Radius) * 3;
			//SetState(NeuronState.Resting);
		}

		public double MaxConnectionRange
		{
			get { return maxConnectionRange; }
			set
			{
				maxConnectionRange = value;
				MaxConnectionRangeSquared = value * value;
			}
		}

		public double MaxConnectionRangeSquared { get; private set; }

		public int Id { get; set; }

		public HashSet<Neuron> DendriteConnections { get; set; }

		public Dictionary<Neuron, Terminal> TerminalConnections { get; set; }

		public Vector2 Position
		{
			get { return Primitive?.Position ?? Vector2.Zero; }
			set { Primitive.Position = value; }
		}

		private double oldCharge;
		private double charge;

		public bool ChargeChanged
		{
			get { return oldCharge != charge; }
		}

		public double Charge
		{
			get { return charge; }
			private set
			{
				oldCharge = charge;
				charge = value;
			}
		}

		public double Threashold { get; set; }

		public Primitive Primitive { get; private set; }

		public bool CreateTerminalConnection(Neuron targetNeuron)
		{
			// A.TerminalConnect(B) =>	A: Add terminal connection to B
			//							B: Add dendrite connection to A
			if (targetNeuron == null)
			{
				throw new Exception("Target neuron is null.");
			}
			if (targetNeuron == this)
			{
				return false;
			}

			if (!TerminalConnections.ContainsKey(targetNeuron)
				//&& !targetNeuron.TerminalConnections.ContainsKey(this)
				)
			{
				TerminalConnections.Add(targetNeuron, new Terminal(targetNeuron));
				brain.PrimitiveConnect(this, targetNeuron);
				targetNeuron.DendriteConnections.Add(this);
				return true;
			}

			return false;
		}

		private void RemoveTerminalConnection(Neuron neuron)
		{
			TerminalConnections.Remove(neuron);
			brain.PrimitiveRemove(this, neuron);
		}

		private void RemoveDendriteConnection(Neuron targetNeuron)
		{
			targetNeuron.RemoveTerminalConnection(this);
			DendriteConnections.Remove(targetNeuron);
		}

		public double PopCharge()
		{
			var value = Charge;
			Charge = 0;
			oldCharge = 0;
			return value;
		}

		private double sensitivity = 0.5d;

		public void Depolarize(double value)
		{
			if (state != NeuronState.Repolarizing)
			{
				//sensitivity = value * 0.01d;
				value = value * sensitivity;
				//value = Sigmoid(value, Const.MV_MAX_EPSP);
				Charge = Math.Min(Charge + value, Const.MV_PEEK);
			}
		}

		public double Sigmoid(double x, double max)
		{
			return (max * 2) / (1 + Math.Exp(-(x / (max * 0.5d)))) - max;
		}

		public void CheckActivation()
		{
			if (state != NeuronState.Repolarizing && Charge >= Threashold)
			{
				Activate();
			}
		}

		private void Activate()
		{
			++excitationCounter;
			SetState(NeuronState.Repolarizing);

			Task.Run(async () =>
			{
				await Task.Delay(Const.MS_HYPERPOLARIZED_TO_RESTING);
				SetState(NeuronState.Hyperpolarized);
			});

			if (TerminalConnections.Count > 0)
			{
				double invTotalWeight = 1d / TerminalConnections.Count;
				double c = Const.MV_PEEK;

				//Parallel.ForEach(TerminalConnections, con =>
				foreach (var con in TerminalConnections)
				{
					double weightedCharge = invTotalWeight * c;
					con.Value.DoSynapse(weightedCharge);
					//con.Value.Neuron.CheckActivation();
				}
				//);

				//Parallel.ForEach(TerminalConnections, con =>
				foreach (var con in TerminalConnections)
				{
					con.Value.Neuron.CheckActivation();
				}
				//);
			}
		}

		private void SetState(NeuronState state)
		{
			this.state = state;
			stateElapsed = 0;

			switch (state)
			{
				case NeuronState.Resting:
					Primitive.Color = Color.Green;
					break;

				case NeuronState.Depolarizing:
					Primitive.Color = Color.Blue;
					break;

				case NeuronState.Repolarizing:
					Primitive.Color = Color.Red;
					break;

				case NeuronState.Hyperpolarized:
					Primitive.Color = Color.Yellow;
					Charge = Const.MV_HYPERPOLARIZED;
					break;
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			Primitive.Draw();

			if (drawDebugText)
			{
				spriteBatch.DrawString(Fonts.Font1, $"{Id.ToString()} | {excitationCounter.ToString()} | {MathHelper.RoundString(Charge, 2)}", Position, Color.White, 0f, new Vector2(16, -32), MainScene.cam.InverseZoom, SpriteEffects.None, 0f);
			}
		}

		public void Update(GameTime gameTime)
		{
			double elapsedMs = gameTime.ElapsedGameTime.TotalMilliseconds;
			stateElapsed += elapsedMs;
			elapsedTime += (float)elapsedMs;

			if (elapsedTime >= SAMPLE_INTERVAL_MS)
			{
				CheckExcitation();
				excitationCounter = 0;
				elapsedTime = 0;
			}

			foreach (var terminal in TerminalConnections)
			{
				terminal.Value.Update(gameTime);
			}

			switch (state)
			{
				case NeuronState.Resting:
					if (stateElapsed >= Const.MS_MAX_IDLE_LIFE && excitationCounter < Const.EXCITATION_MIN)
					{
						//Kill();
					}
					break;

				case NeuronState.Depolarizing:
					break;

				case NeuronState.Repolarizing:
					break;

				case NeuronState.Hyperpolarized:
					if (Charge >= Const.MV_RESTING)
					{
						SetState(NeuronState.Resting);
						break;
					}
					else if (stateElapsed >= Const.MS_HYPERPOLARIZED_TO_RESTING)
					{
						Charge = Math.Max(Charge, Const.MV_RESTING);
						SetState(NeuronState.Resting);
						break;
					}

					var amount = stateElapsed / Const.MS_HYPERPOLARIZED_TO_RESTING;
					Charge = SmoothStep(Charge, Const.MV_RESTING, amount);

					if (Charge >= Const.MV_RESTING)
					{
						SetState(NeuronState.Resting);
					}
					break;
			}
		}

		public static double SmoothStep(double from, double to, double amount)
		{
			return Lerp(from, to, (amount * amount) * (3f - (2f * amount)));
		}

		public static double Lerp(double from, double to, double amount)
		{
			return from + (to - from) * amount;
		}

		private void CheckExcitation()
		{
			if (excitationCounter > Const.EXCITATION_AVG_MAX)
			{
				// Quote:
				// "If the average electrical activity of a neuron exceeds some maximum level, it will withdraw dendritic spines 
				// and retract neurite branches[65], which may reduce connectivity and hence activity"
				KillRandomDendriteConnection();
			}
			else if (excitationCounter < Const.EXCITATION_AVG_MIN)
			{
				// Quote:
				// "If activity becomes lower, the neuron will generate vacant synaptic elements[66], increasing connectivity and activity."
				CreateRandomDendriteConnection();
			}
		}

		private void Kill()
		{
			foreach (var neuron in DendriteConnections)
			{
				neuron.RemoveTerminalConnection(this);
			}
			foreach (var kvp in TerminalConnections)
			{
				brain.PrimitiveRemove(this, kvp.Key);
			}
			brain.Remove(this);
		}

		private void KillRandomDendriteConnection()
		{
			if (DendriteConnections.Count <= 1)
			{
				return;
			}

			var neuron = DendriteConnections.OrderBy(n => Vector2.DistanceSquared(Position, n.Position)).SelectFirstRandom();
			RemoveDendriteConnection(neuron);
		}

		public void CreateRandomDendriteConnection()
		{
			if (brain.Neurons.Count <= 1 || DendriteConnections.Count >= Const.MAX_CONNECTIONS)
			{
				return;
			}

			var selectedNeuron = SelectClosestNeuron();
			//var selectedNeuron = SelectMostProbable();

			if (selectedNeuron != null)
			{
				selectedNeuron.CreateTerminalConnection(this);
			}
		}

		private Neuron SelectMostProbable()
		{
			return brain.Neurons
							.OrderByDescending(n => Vector2.DistanceSquared(Position, n.Position))
							.Where(n => Vector2.DistanceSquared(Position, n.Position) <= MaxConnectionRange)
							.SelectFirstRandom();
		}

		private Neuron SelectClosestNeuron()
		{
			Neuron closestNeuron = null;
			double closestDist = MaxConnectionRangeSquared;
			float x = Position.X;
			float y = Position.Y;

			foreach (var neuron in brain.Neurons)
			{
				if (neuron == this || DendriteConnections.Contains(neuron))
				{
					continue;
				}

				float distX = neuron.Position.X - x;
				if (distX > MaxConnectionRange)
				{
					continue;
				}

				float distY = neuron.Position.Y - y;
				if (distY > MaxConnectionRange)
				{
					continue;
				}

				float distSquared = (distX * distX) + (distY * distY);

				if (distSquared < closestDist)
				{
					closestDist = distSquared;
					closestNeuron = neuron;
				}
			}

			return closestNeuron;
		}

		public override string ToString()
		{
			return Id.ToString();
		}
	}
}