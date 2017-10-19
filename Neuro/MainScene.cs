using DXFramework;
using DXFramework.SceneManagement;
using DXFramework.UI;
using DXFramework.Util;
using DXPrimitiveFramework;
using Neuro.NS;
using Neuro.Objects;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neuro
{
	public class MainScene : Scene
	{
		private bool wireFrame;
		public static UIManager uiManager;
		public static Camera cam;
		private List<GameObject> objects;
		private Bug bug;
		private bool paused;

		public override void LoadContent()
		{
			uiManager = new UIManager();
			uiManager.EnableProfilling = true;
			UIManager.DrawDebug = false;

			cam = new Camera(GraphicsDevice);
			objects = new List<GameObject>();
			bug = new Bug();
			objects.Add(bug);

			CreateControlPanel();

			FoodManager.Init(graphicsDevice);

			base.LoadContent();
		}

		public static Dictionary<Neuron, UIScrollBar> inputNeuronToSlider;

		private void CreateControlPanel()
		{
			inputNeuronToSlider = new Dictionary<Neuron, UIScrollBar>();

			var panel = new UIPanel();
			panel.DrawBounds = true;
			panel.AddConstraint(Edge.Top, null, Edge.Top, 10);
			panel.AddConstraint(Edge.Left, null, Edge.Left, 10);
			panel.MarginAll = 10;

			UIControl prevControl = null;
			foreach (var neuron in bug.Brain.InputNeurons)
			{
				var sb = new UIScrollBar(UIScrollBar.ScrollBarOrientation.Horizontal);
				sb.Size = new Vector2(300, 15);
				if (prevControl == null)
				{
					sb.AddConstraint(Edge.Top, panel, Edge.Top);
				}
				else
				{
					sb.AddConstraint(Edge.Top, prevControl, Edge.Bottom, -10);
				}

				sb.AddConstraint(Edge.Left, panel, Edge.Left);
				sb.ValueChanged += Sb_ValueChanged;

				var label = new UILabel();
				label.AddConstraint(Edge.CenterY, sb, Edge.CenterY);
				label.AddConstraint(Edge.Left, sb, Edge.Right, -10);
				label.SetText(MathHelper.RoundString(sb.Value));

				sb.Tag = label;

				panel.AddChild(sb);
				panel.AddChild(label);
				prevControl = sb;

				inputNeuronToSlider.Add(neuron, sb);
			}

			panel.DoLayout();
			uiManager.Add(panel);
		}

		private static float inputScale = 100;
		private void Sb_ValueChanged(object sender, EventArgs e)
		{
			var sb = sender as UIScrollBar;
			var label = sb.Tag as UILabel;

			label.SetText(MathHelper.RoundString(sb.Value * inputScale));
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
		}

		public override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			if (wireFrame)
			{
				GraphicsDevice.SetRasterizerState(GraphicsDevice.RasterizerStates.WireFrame);
			}

			spriteBatch.Begin(SpriteSortMode.Deferred, spriteBatch.GraphicsDevice.BlendStates.NonPremultiplied, null, null, null, null, cam.GetTransformation());
			PrimitiveBatch.Begin(cam.GetOrthographicTransformation());

			objects.ForEach(o => o.Draw(spriteBatch));
			//FoodManager.Draw(spriteBatch, gameTime);
			(objects[0] as Bug).Brain.Draw(spriteBatch);

			PrimitiveBatch.End();
			spriteBatch.End();

			uiManager.Draw(spriteBatch);
		}

		public override void Update(GameTime gameTime)
		{
			if (!paused)
			{
				//FoodManager.Update(gameTime);
				objects.ForEach(o => o.Update(gameTime));
				inputNeuronToSlider.ForEach(kvp =>
				{
					var value = kvp.Value.Value * inputScale;
					kvp.Key.Depolarize(value * gameTime.ElapsedGameTime.TotalSeconds);
					kvp.Key.CheckActivation();
				});
				uiManager.Update(gameTime);
				uiManager.SetDebugValue("Pointer handled", InputManager.PointerHandled);

				bug.Brain.OutputNeurons.ForEach(n =>
				{
					if (n.ChargeChanged)
					{
						uiManager.SetDebugValue($"o: {n.Id}", n.PopCharge());
					}
				});
			}

			if (!InputManager.PointerHandled)
			{
				UpdateInput(gameTime);
			}
		}

		private void UpdateInput(GameTime gameTime)
		{
			var wheelDelta = InputManager.MouseWheelDelta;
			if (wheelDelta != 0)
			{
				float scrollSpeed = 0.0005f;

				if (InputManager.Held(Keys.Shift))
				{
					scrollSpeed *= 3;
				}
				cam.Zoom += wheelDelta * cam.Zoom * scrollSpeed;
			}

			if (InputManager.Pressed(Keys.P))
			{
				paused = !paused;
			}
			if (InputManager.Pressed(Keys.F1))
			{
				Neuron.drawDebugText = !Neuron.drawDebugText;
			}
			if (InputManager.Pressed(Keys.F2))
			{
				Brain.drawDebugLines = !Brain.drawDebugLines;
			}

			//if (InputManager.Pressed(Keys.F12))
			//{
			//	Brain.drawDebugLines = !Brain.drawDebugLines;
			//}

			if (InputManager.Pressed(MouseButton.Left))
			{
				cam.InitiateCursorMovement();
			}
			if (InputManager.Held(MouseButton.Left))
			{
				cam.DoCursorMovement();
			}

			if (InputManager.Pressed(Keys.F12))
			{
				wireFrame = !wireFrame;
			}

			if (InputManager.Pressed(Keys.Escape))
			{
				Game.Exit();
			}
		}
	}
}