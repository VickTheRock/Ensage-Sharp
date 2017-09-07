namespace StormSpirit
{
    using System;
    using System.Collections.Generic;
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Menu;
    using SharpDX;
    using System.Linq;
    using Ensage.Common.Extensions;

    partial class StormSpirit
    {
        public static float qDmg, wDmg, eDmg, rDmg, sheepDmg;
        public static double[] penitence = { 0, 1.15, 1.2, 1.25, 1.3 };
        public static double[] bloodrage = { 0, 1.15, 1.2, 1.25, 1.3 };
        public static double[] souls = { 0, 1.2, 1.3, 1.4, 1.5 };
        public static readonly int[] dagonDmg = { 0, 400, 500, 600, 700, 800 };
        public static Dictionary<uint, float> damage = new Dictionary<uint, float>();

        public static List<Hero> enemies = new List<Hero>();
        public static ParticleEffect BlinkRange { get; set; }
        public static ParticleEffect QRange { get; set; }
        public static ParticleEffect WRange { get; set; }
        public static ParticleEffect RRange { get; set; }

        public static readonly Menu Menu = new Menu("StormSpirit", "StormSpirit by Vick", true, "npc_dota_hero_storm_spirit", true);
        public static Hero me, e;
        public static bool Active;
        public static Ability Q, W, E, R;
        public static Item orchid, sheep, vail, soul, arcane, blink, 
            urn, glimmer, mail, bkb, lotus, shiva, dagon, atos, ethereal, cheese, ghost, shadB, force, cyclone, travel;

        public static ParticleEffect Effect;
        public static bool IsEnable => Menu.Item("enabled").GetValue<bool>();
        public static readonly Menu skills = new Menu("Skills", "Skills");
        public static readonly Menu items = new Menu("Items", "Items");
        public static float lastblinkRange, lastqRange, lastwRange, lastrRange;
        public static float blinkRange, qRange, wRange, rRange;
        public static readonly Menu drawing = new Menu("Drawing", "Drawing");
        public static readonly Menu ult = new Menu("Safe my ass", "AutoAbility");

        public static void OnValueChanged(object sender, OnValueChangeEventArgs args)
        {
            var oldOne = args.GetOldValue<KeyBind>().Active;
            var newOne = args.GetNewValue<KeyBind>().Active;
            if (oldOne == newOne || newOne) return;
            try
            {
                e = null;
                Effect?.Dispose();
                Effect = null;
            }
            catch (Exception)
            {
            }
        }
        public static readonly Menu orbwolk = new Menu("Orbwolk", "Orbwolk");
        public static void Success(string text, params object[] arguments)
        {
            Game.PrintMessage("<font color='#00ff00'>" + text + "</font>");
        }

        public static void Init()
        {
            try
            {
                Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D'))).ValueChanged += OnValueChanged;
                
                skills.AddItem(new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"storm_spirit_static_remnant", true},
                {"storm_spirit_electric_vortex", true},
                {"storm_spirit_overload", true},
                {"storm_spirit_ball_lightning", true}
            })));
                skills.AddItem(new MenuItem("AutoOverload", "Use Overload if target disable?").SetValue(false));

                skills.AddItem(new MenuItem("savemanamode", "Use save mana mode.").SetValue(false));
                items.AddItem(new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {

                {"item_silver_edge", true},
                {"item_glimmer_cape", true},
                {"item_dagon",true},
                {"item_bloodthorn", true},
                {"item_ethereal_blade", true},
                {"item_veil_of_discord", true},
                {"item_rod_of_atos", true},
                {"item_sheepstick", true},
                {"item_arcane_boots", true},
                {"item_shivas_guard",true},
                {"item_blink", true},
                {"item_lotus_orb", true},
                {"item_urn_of_shadows",true},
                {"item_soul_ring", true},
                {"item_ghost", true},
                {"item_cheese", true},
                {"item_blade_mail",true},
                {"item_black_king_bar",true}
            })));
                items.AddItem(new MenuItem("Heel", "Min targets to BKB|BladeMail").SetValue(new Slider(2, 1, 5)));
                ult.AddItem(new MenuItem("BallLightningAbility", "Use abilities if I not have enough health.").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"storm_spirit_ball_lightning", true},
                {"item_travel_boots_2", true}
            })));
                ult.AddItem(new MenuItem("RHealh", "Minimum health in percent to escape.").SetValue(new Slider(15, 02, 50)));
                
                items.AddItem(new MenuItem("autoLink", "Auto triggre Linken").SetValue(true));
                items.AddItem(new MenuItem("Link", "Auto triggre Linken").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"item_sheepstick", true},
                {"item_force_staff", true},
                {"item_cyclone", true},
                {"item_rod_of_atos", true},
                {"item_dagon", true}
            })));
                items.AddItem(new MenuItem("debuff", "Wait ethereal debuff").SetValue(true));
                orbwolk.AddItem(new MenuItem("orbwalk", "Orbwalk").SetValue(true));
                orbwolk.AddItem(new MenuItem("range", "Min. range to move target position.").SetValue(new Slider(50, 25, 100)));
                Menu.AddSubMenu(skills);
                Menu.AddSubMenu(items);
                Menu.AddSubMenu(ult);
                Menu.AddSubMenu(orbwolk);

                drawing.AddItem(new MenuItem("dmg", "Show drawing damage to kill").SetValue(true));
                drawing.AddItem(new MenuItem("Range Blink", "Draw range for Blink").SetValue(true));
                drawing.AddItem(new MenuItem("Range Electric Vortex", "Draw range for Electric Vortex").SetValue(true));
                drawing.AddItem(new MenuItem("Range Static Remnant", "Draw range for Static Remnant").SetValue(true));
                Menu.AddSubMenu(drawing);
                Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
                Menu.AddToMainMenu();
                
                Success("Zip! Zap! Ha ha ha ha! Whoa-ho-ho! Ooh, who's that handsome devil?");
                DelayAction.Add(500, () =>
                {

                    if (Menu.Item("Range Blink").GetValue<bool>())
                    {
                        Menu.Item("Range Blink").SetValue(false);
                        Menu.Item("Range Blink").SetValue(true);
                    }
                    if (Menu.Item("Range Static Remnant").GetValue<bool>())
                    {
                        Menu.Item("Range Static Remnant").SetValue(false);
                        Menu.Item("Range Static Remnant").SetValue(true);
                    }
                    if (Menu.Item("Range Electric Vortex").GetValue<bool>())
                    {
                        Menu.Item("Range Electric Vortex").SetValue(false);
                        Menu.Item("Range Electric Vortex").SetValue(true);
                    }
                });
            }
            catch (Exception ex)
            {
                var st = new System.Diagnostics.StackTrace(ex, true);
                var line = st.GetFrame(0).GetFileLineNumber();
                Console.WriteLine("Menu exception at line: " + line);
            }
        }

        public static bool OnScreen(Vector3 v)
        {
            return Drawing.WorldToScreen(v) != Vector2.Zero;
        }

        public static Hero GetClosestToTarget(List<Hero> units, Hero z)
        {
            Hero closestHero = null;
            foreach (var v in units.Where(v => closestHero == null || closestHero.Distance2D(z) > v.Distance2D(z)))
            {
                closestHero = v;
            }
            return closestHero;
        }
    }
}
