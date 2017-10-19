using System;
using DXFramework;
using DXFramework.SceneManagement;
using DXFramework.Util;
using DXPrimitiveFramework;
using SharpDX.Toolkit;

namespace Neuro
{
	public class Main: Game
	{
		private GraphicsDeviceManager graphicsDeviceManager;
		private SceneManager sceneManager;

		public Main()
		{
			graphicsDeviceManager = new GraphicsDeviceManager(this);
			graphicsDeviceManager.PreferredBackBufferWidth = 1024;
			graphicsDeviceManager.PreferredBackBufferHeight = 768;
			graphicsDeviceManager.DeviceCreationFlags = SharpDX.Direct3D11.DeviceCreationFlags.Debug;
			
			Content.RootDirectory = "Content";

			// Frame rate set to 30 fps.
			TargetElapsedTime = TimeSpan.FromTicks(333333);

			// Set IsFixedTimeStep to true, to avoid game speedups/downs.
			IsFixedTimeStep = false;

			// Extend battery life under lock.
			InactiveSleepTime = TimeSpan.FromSeconds(1);
		}

		protected override void Initialize()
		{
			Window.Title = "Neuro";
			IsMouseVisible = true;
			
			Engine.Initialize(this);
			Fonts.Initialize(this);
			InputManager.Initialize(this);
			PrimitiveBatch.Initialize(this.GraphicsDevice);

			sceneManager = new SceneManager(this);
			FrameRateCounter fpsCounter = new FrameRateCounter(this);

			GameSystems.Add(sceneManager);
			GameSystems.Add(fpsCounter);

			base.Initialize();
		}

		protected override void LoadContent()
		{
			base.LoadContent();
			sceneManager.Set<MainScene>();
		}

		protected override void UnloadContent()
		{
			InputManager.ClearMouseClip(); // Precaution to avoid mouse being clipped after application is closed.
			base.UnloadContent();
		}

		protected override void Update(GameTime gameTime)
		{
			InputManager.Update();
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
		}
	}
}