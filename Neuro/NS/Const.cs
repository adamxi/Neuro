namespace Neuro.NS
{
	public static class Const
	{
		#region Charge
		public const int MV_RESTING = 0;
		public const int MV_THREASHOLD = 15;
		public const int MV_PEEK = 100;
		public const int MV_HYPERPOLARIZED = -10;
		public const int MV_MAX_EPSP = 10; // Excitatory Postsynaptic Potential
		#endregion

		#region Connections
		public const int MAX_CONNECTIONS = 10000;
		#endregion

		#region Excitation
		public const int EXCITATION_MIN = 1;
		public const int EXCITATION_AVG_MIN = 5;
		public const int EXCITATION_AVG_MAX = 50;
		public const int EXCITATION_SAMPLE_PERIOD = 60 * 1;
		#endregion

		#region Periods
		public const int TIME_SCALE = 1;

		public const int MS_MAX_IDLE_LIFE = 1000 * 60;
		public const int MS_THREASHOLD_TO_PEEK = TIME_SCALE * 1;
		public const int MS_PEEK_TO_HYPERPOLARIZED = TIME_SCALE * 2;
		public const int MS_HYPERPOLARIZED_TO_RESTING = TIME_SCALE * 1;

		public const int MS_AP_MIN = TIME_SCALE * 10;
		public const int MS_AP_MAX = TIME_SCALE * 40;

		public const int MS_PSP_MIN = TIME_SCALE * 1;
		public const int MS_PSP_MAX = TIME_SCALE * 5;
		#endregion
	}

	public enum NeuronState
	{
		/// <summary>
		/// Active state.
		/// Subject to depolarization from incoming connections.
		/// </summary>
		Resting,

		/// <summary>
		/// Active state.
		/// Neuron is depolarizing until peek Mv.
		/// Subject to further depolarization from incoming connections.
		/// </summary>
		Depolarizing,

		/// <summary>
		/// Inactive state.
		/// Absolute refractory period.
		/// </summary>
		Repolarizing,

		/// <summary>
		/// Active state.
		/// Relative refractory period.
		/// </summary>
		Hyperpolarized
	}
}