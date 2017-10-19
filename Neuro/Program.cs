using System;

namespace Neuro
{
	class Program
	{
#if NETFX_CORE
        [MTAThread]
#else
		[STAThread]
#endif
		static void Main()
		{
			using (var program = new Main())
				program.Run();

		}
	}
}