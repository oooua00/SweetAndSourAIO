namespace SweetAndSourAIO
{
	using System;
	using System.Diagnostics.CodeAnalysis;

	using HesaEngine.SDK;

	using SweetAndSourAIO.Champions;

	public class Load : IScript
	{
		#region Public Properties

		public string Author => "Sweet&Sour";

		public string Name => "[Sweet&Sour AIO]";

		public string Version => "1.0.0";

		#endregion

		#region Public Methods and Operators

		public void OnInitialize()
		{
			try
			{
				SSAIO.Load();
			}
			catch (Exception e)
			{
				Logger.Log("@Program.cs: Cannot Load Program - " + e);
			}
		}

		#endregion
	}

	internal class SSAIO
	{
		#region Properties

		#endregion

		#region Public Methods and Operators

		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "They would not be used.")]
		public static void Load()
		{
			Game.OnGameLoaded += () =>
				{
					switch (ObjectManager.Player.ChampionName)
					{
						case "Vayne":
							new Vayne();
							Chat.Print("[Sweet&Sour AIO] - Vayne Loaded");
							break;
						case "Lucian":
							new Lucian();
							Chat.Print("[Sweet&Sour AIO] - Lucian Loaded");
							break;
						default:
							Chat.Print("[Sweet&Sour AIO] - Nothing Loaded - Champion Unsupported.");
							break;
					}
				};
		}

		#endregion
	}
}