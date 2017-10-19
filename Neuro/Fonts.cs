using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace Neuro
{
	public static class Fonts
	{
		public static SpriteFont Font1 { get; private set; }

		public static void Initialize(Game game)
		{
			Font1 = game.Content.Load<SpriteFont>("Fonts/Font1");
		}
	}
}