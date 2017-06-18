namespace SweetAndSourAIO.Champions
{
	#region Using Directives

	using System.Linq;

	using HesaEngine.SDK;
	using HesaEngine.SDK.Enums;
	using HesaEngine.SDK.GameObjects;

	using SharpDX;

	#endregion

	internal class Lucian
	{
		#region Static Fields

		internal static Spell E;

		internal static Menu MainMenu;

		// ReSharper disable once InconsistentNaming
		internal static Orbwalker.OrbwalkerInstance SSAIOOrbwalker => Core.Orbwalker;

		internal static Spell Q;

		internal static Spell QExt;

		internal static Spell R;

		internal static Spell W;

		#endregion

		#region Constructors and Destructors

		public Lucian()
		{
			while (MainMenu == null)
			{
				CreateMenu();
			}
			while (Q == null || W == null || E == null || R == null)
			{
				CreateSpells();
			}

			Orbwalker.AfterAttack += (source, target) =>
				{
					if (!source.IsMe || !target.IsValid() || !MainMenu.Item("miscPassive").GetValue<bool>()) return;

					if (SSAIOOrbwalker.ActiveMode == Orbwalker.OrbwalkingMode.Combo)
					{
						if (MainMenu.Item("comboE").GetValue<bool>() && E.IsReady())
						{
							CastE(target as Obj_AI_Base);
							Orbwalker.ResetAutoAttackTimer();
							return;
						}

						if (MainMenu.Item("comboQ").GetValue<bool>() && Q.IsReady())
						{
							if (MainMenu.Item("miscQExtended").GetValue<bool>())
							{
								if (ExtQ(target as Obj_AI_Base))
								{
									return;
								}
							}
							Q.CastOnUnit(target as Obj_AI_Base, true);
							return;
						}
						
						if (MainMenu.Item("comboW").GetValue<bool>() && W.IsReady())
						{
							W.Cast(target as Obj_AI_Base, true, true);
						}
					}

					if (SSAIOOrbwalker.ActiveMode != Orbwalker.OrbwalkingMode.Harass) return;
					if (MainMenu.Item("harassE").GetValue<bool>() && E.IsReady())
					{
						CastE(target as Obj_AI_Base);
						return;
					}

					if (MainMenu.Item("harassQ").GetValue<bool>() && Q.IsReady())
					{
						if (MainMenu.Item("miscQExtended").GetValue<bool>())
						{
							if (ExtQ(target as Obj_AI_Base))
							{
								return;
							}
						}
						Q.CastOnUnit(target as Obj_AI_Base, true);
						return;
					}

					if (MainMenu.Item("harassW").GetValue<bool>() && W.IsReady())
					{
						W.Cast(target as Obj_AI_Base, true, true);
					}
				};

			Drawing.OnDraw += args =>
				{
					if (MainMenu.Item("drawingReady").GetValue<bool>())
					{
						if (Q.IsReady() && MainMenu.Item("drawingQ").GetValue<bool>())
						{
							Drawing.DrawCircle(ObjectManager.Player.Position, Q.Range, new ColorBGRA(255, 0, 0, 255));
						}
						if (W.IsReady() && MainMenu.Item("drawingW").GetValue<bool>())
						{
							Drawing.DrawCircle(ObjectManager.Player.Position, W.Range, new ColorBGRA(0, 255, 0, 255));
						}
						if (E.IsReady() && MainMenu.Item("drawingE").GetValue<bool>())
						{
							Drawing.DrawCircle(ObjectManager.Player.Position, E.Range, new ColorBGRA(0, 0, 255, 255));
						}
						if (R.IsReady() && MainMenu.Item("drawingR").GetValue<bool>())
						{
							Drawing.DrawCircle(ObjectManager.Player.Position, R.Range, new ColorBGRA(255, 255, 255, 255));
						}
					}
					else
					{
						if (MainMenu.Item("drawingQ").GetValue<bool>())
						{
							Drawing.DrawCircle(ObjectManager.Player.Position, Q.Range, new ColorBGRA(255, 0, 0, 255));
						}
						if (MainMenu.Item("drawingW").GetValue<bool>())
						{
							Drawing.DrawCircle(ObjectManager.Player.Position, W.Range, new ColorBGRA(0, 255, 0, 255));
						}
						if (MainMenu.Item("drawingE").GetValue<bool>())
						{
							Drawing.DrawCircle(ObjectManager.Player.Position, E.Range, new ColorBGRA(0, 0, 255, 255));
						}
						if (MainMenu.Item("drawingR").GetValue<bool>())
						{
							Drawing.DrawCircle(ObjectManager.Player.Position, R.Range, new ColorBGRA(255, 255, 255, 255));
						}
					}
				};

			//CustomEvents.Unit.OnDash += (sender, args) =>
			//	{
			//		if (sender.Team == ObjectManager.Player.Team) return;

			//		if (args.EndPos.Distance(ObjectManager.Player.ServerPosition) > 300) return;

			//		if (!MainMenu.Item("miscEDodge").GetValue<bool>() || !E.IsReady()) return;

			//		var leftPos = args.EndPos.Extend(args.StartPos, -E.Range).RotateAroundPoint(args.EndPos, 90f);
			//		var rightPos = args.EndPos.Extend(args.StartPos, -E.Range).RotateAroundPoint(args.EndPos, -90f);
			//		E.Cast(
			//			(Game.CursorPosition.To2D() - rightPos).Length() <= (Game.CursorPosition.To2D() - leftPos).Length()
			//				? rightPos
			//				: leftPos);
			//	};
		}

		#endregion

		#region Methods

		private static void CastE(Obj_AI_Base target)
		{
			var dashRange = E.Range;
			switch (MainMenu.Item("comboEDash").GetValue<int>())
			{
				case 0:
					dashRange = E.Range;
					break;
				case 1:
					dashRange = 10f;
					break;
				default:
					dashRange = E.Range;
					break;
			}
			var ePosition = Vector3.Zero;
			var heroPos = ObjectManager.Player.Position;
			var hero2D = heroPos.To2D();
			var target2D = target.Position.To2D();
			var perpendicular = new Vector2(target2D.X - hero2D.X, target2D.Y - hero2D.Y).Normalized().Perpendicular();
			var rightPos = new Vector3(
				(target2D + perpendicular * (heroPos - target.Position).Length()).X,
				(target2D + perpendicular * (heroPos - target.Position).Length()).Y,
				heroPos.Z);
			var leftPos = new Vector3(
				(target2D - perpendicular * (heroPos - target.Position).Length()).X,
				(target2D + perpendicular * (heroPos - target.Position).Length()).Y,
				heroPos.Z);

			switch (MainMenu.Item("comboEMode").GetValue<int>())
			{
				case 0:
					ePosition = heroPos.Extend(Game.CursorPosition, dashRange);
					break;
				case 1:
					if (ObjectManager.Player.GetAutoAttackDamage(target, true) > target.Health)
						ePosition = heroPos.Extend(target.Position, dashRange);
					else if (target.HealthPercent < 20f && ObjectManager.Player.HealthPercent > 40f)
						ePosition = heroPos.Extend(target.Position, dashRange);
					else if ((Game.CursorPosition - rightPos).Length() <= (Game.CursorPosition - leftPos).Length())
						ePosition = heroPos.Extend(rightPos, dashRange);
					else ePosition = heroPos.Extend(leftPos, dashRange);
					break;
				default:
					ePosition = heroPos.Extend(Game.CursorPosition, dashRange);
					break;
			}
			if (ePosition != Vector3.Zero)
			{
				E.Cast(ePosition);
			}
		}

		private static bool ExtQ(Obj_AI_Base target)
		{
			if (!target.IsValidTarget()) return false;
			var targetPos = new Prediction().GetPrediction(target, Q.Delay).UnitPosition;
			var castPos = ObjectManager.Player.Position.Extend(targetPos, 1000);

			return ObjectManager.MinionsAndMonsters.Enemy.Where(
					minion => minion.IsValidTarget(Q.Range))
				.Where(minion => (ObjectManager.Player.Position.Extend(minion.Position, QExt.Range) - castPos).Length() < 65)
				.Any(minion => Q.CastOnUnit(minion));
		}

		private static void CreateMenu()
		{
			MainMenu = Menu.AddMenu("[Sweet&Sour AIO] " + ObjectManager.Player.ChampionName);

			var comboMenu = MainMenu.AddSubMenu("[Sweet&Sour AIO] Combo Menu");
			{
				comboMenu.Add(new MenuCheckbox("comboQ", "[Combo] Use Q", true));
				comboMenu.Add(new MenuCheckbox("comboW", "[Combo] Use W", true));
				comboMenu.Add(new MenuCheckbox("comboE", "[Combo] Use E", true));
				comboMenu.Add(new MenuCombo("comboEMode", "[Combo] E Mode", new[] { "Cursor", "Side" }, 1));
				comboMenu.Add(new MenuCombo("comboEDash", "[Combo] E Dash Mode", new[] { "Full Range", "Short" }));
				//comboMenu.Add(new MenuKeybind("comboR", "[Combo] Semi-Auto R (Only on Killable)", new KeyBind(Key.T)));
			}

			var harassMenu = MainMenu.AddSubMenu("[Sweet&Sour AIO] Harass Menu");
			{
				harassMenu.Add(new MenuCheckbox("harassQ", "[Harass] Use Q", true));
				harassMenu.Add(new MenuCheckbox("harassW", "[Harass] Use W", true));
				harassMenu.Add(new MenuCheckbox("harassE", "[Harass] Use E", true));
				harassMenu.Add(new MenuSlider("harassMana", "[Mana] Min. Mana", new Slider(0, 100, 60)));
			}

			var farmMenu = MainMenu.AddSubMenu("[Sweet&Sour AIO] Farm Menu - DISABLED [SoonTM]");
			{
				farmMenu.Add(new MenuCheckbox("farmQ", "[Farm] Use Q", true));
				farmMenu.Add(new MenuCheckbox("farmQExtended", "[Farm] Use Extended Q", true));
				farmMenu.Add(new MenuCheckbox("farmW", "[Farm] Use W", true));
				farmMenu.Add(new MenuSlider("farmWMinions", "[Farm] Min. W Minions", new Slider(0, 6, 2)));
				farmMenu.Add(new MenuSlider("farmMana", "[Mana] Min. Mana", new Slider(0, 100, 50)));
			}

			var miscMenu = MainMenu.AddSubMenu("[Sweet&Sour AIO] Misc Menu");
			{
				miscMenu.Add(new MenuCheckbox("miscPassive", "[Misc] Smart Passive Usage", true));
				miscMenu.Add(new MenuCheckbox("miscQks", "[Misc] Q KS", true));
				miscMenu.Add(new MenuCheckbox("miscQExtended", "[Misc] Use Extended Q", true));
				//miscMenu.Add(new MenuCheckbox("miscEDodge", "[Misc] Dodge Gapclosers with E", true));
			}

			var drawingMenu = MainMenu.AddSubMenu("[Sweet&Sour AIO] Drawing Menu");
			{
				drawingMenu.Add(new MenuCheckbox("drawingReady", "[Drawing] Draw Only Ready", true));
				drawingMenu.Add(
					new MenuCheckbox("drawingQ", "[Draw] Draw Q", true));
				drawingMenu.Add(
					new MenuCheckbox("drawingW", "[Draw] Draw W", true));
				drawingMenu.Add(
					new MenuCheckbox("drawingE", "[Draw] Draw E", true));
				drawingMenu.Add(
					new MenuCheckbox("drawingR", "[Draw] Draw R", true));
			}
		}

		private static void CreateSpells()
		{
			Q = new Spell(SpellSlot.Q, 675f);
			Q.SetTargetted(0.4f, 1600f);

			QExt = new Spell(SpellSlot.Q, 900f);
			QExt.SetTargetted(0.4f, 1600f);

			W = new Spell(SpellSlot.W, 900f);
			W.SetSkillshot(0.25f, 80f, 1600f, true, SkillshotType.SkillshotLine);

			E = new Spell(SpellSlot.E, 425f);
			E.SetSkillshot(0.25f, 425f, 1000f, false, SkillshotType.SkillshotLine);

			R = new Spell(SpellSlot.R, 1200f);
			R.SetSkillshot(0.5f, 110f, 2800f, true, SkillshotType.SkillshotLine);
		}

		#endregion
	}
}