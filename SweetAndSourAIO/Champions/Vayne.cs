namespace SweetAndSourAIO.Champions
{
	using System.Collections.Generic;

	using HesaEngine.SDK;

	internal class Vayne
	{
		#region Constructors and Destructors

		public Vayne()
		{
			Game.OnUpdate += () => { };
		}

		#endregion

		#region Public Properties

		public Menu Menu { get; set; }

		public List<Spell> Spells { get; set; }

		#endregion
	}
}