namespace CrystalMaiden
{
    using System;
    using System.Collections.Generic;
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Menu;
    public static class Toolset
    {
        public static double wDmg;
        public static double qDmg;
        public static double[] penitence = { 0, 1.15, 1.2, 1.25, 1.3 };
        public static double[] bloodrage = { 0, 1.15, 1.2, 1.25, 1.3 };
        public static double[] souls = { 0, 1.2, 1.3, 1.4, 1.5 };
        public static readonly int[] dagonDmg = { 0, 400, 500, 600, 700, 800 };

        public static Dictionary<uint, double> damage = new Dictionary<uint, double>();

        public static List<Hero> enemies = new List<Hero>();
        public static readonly Menu autoA = new Menu("AutoUsage Abilities and Items to Solo enemy kill", "id");
        public static ParticleEffect BlinkRange { get; set; }
        public static ParticleEffect QRange { get; set; }
        public static ParticleEffect WRange { get; set; }
        public static ParticleEffect RRange { get; set; }
        public static readonly Menu Menus = new Menu("CrystalMaiden", "CrystalMaiden by Vick", true, "npc_dota_hero_crystal_maiden", true);
        public static Hero me, e;
        public static bool Active;
        public static Ability Q, W, R;
        public static Item orchid, sheep, vail, soul, arcane, blink
            , urn, glimmer, mail, bkb, lotus, shiva, dagon, atos, ethereal, cheese, ghost, shadB, force, cyclone;

        public static ParticleEffect Effect;
        public static bool IsEnable => Menus.Item("enabled").GetValue<bool>();
        public static readonly Menu skills = new Menu("Skills", "Skills");
        public static readonly Menu items = new Menu("Items", "Items");
        public static float lastblinkRange, lastqRange, lastwRange, lastrRange;
        public static float blinkRange, qRange, wRange, rRange;
        public static readonly Menu drawing = new Menu("Drawing", "Drawing");
        public static readonly Menu ult = new Menu("AutoAbility", "AutoAbility");
        public static readonly Menu healh = new Menu("Healh", "Max Enemy Healh % to Ult");

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
                Menus.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
                Menus.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D'))).ValueChanged += OnValueChanged;


                skills.AddItem(new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"crystal_maiden_crystal_nova", true},
                {"crystal_maiden_frostbite", true},
                {"crystal_maiden_freezing_field", true}
            })));
                items.AddItem(new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {

                {"item_silver_edge", true},
                {"item_glimmer_cape", true},
                {"item_dagon",true},
                {"item_orchid", true},
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
                ult.AddItem(new MenuItem("autoUlt", "AutoAbility").SetValue(true));
                ult.AddItem(new MenuItem("AutoAbility", "AutoAbility").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"crystal_maiden_frostbite", true},
                {"item_glimmer_cape", true},
                { "item_silver_edge", true},
                {"item_ghost", true},
                {"item_rod_of_atos", true},
                {"item_black_king_bar",true}
            })));
                autoA.AddItem(new MenuItem("dmg", "Show draw damage to kill").SetValue(true));
                autoA.AddItem(new MenuItem("AutoUsage", "AutoUsage").SetValue(true));
                autoA.AddItem(new MenuItem("minHealth", "Min me healh % to blink in killsteal").SetValue(new Slider(25, 05))); // x/ 10%
                autoA.AddItem(new MenuItem("solo_kill", "Max Enemies in Range to solo kill").SetValue(new Slider(2, 1, 5)));
                autoA.AddItem(new MenuItem("AutoSpells", "Auto spells to enemies kill").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"crystal_maiden_crystal_nova", true},
                {"crystal_maiden_frostbite", true}
            })));
                autoA.AddItem(new MenuItem("AutoItems", "Auto items to enemies kill").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"item_rod_of_atos", true},
                {"item_bloodthorn", true},
                {"item_blink", true},
                {"item_dagon", true},
                {"item_shivas_guard", true},
                {"item_veil_of_discord", true},
                {"item_ethereal_blade", true}
            })));
                items.AddItem(new MenuItem("autoLink", "Auto triggre Linken").SetValue(true));
                items.AddItem(new MenuItem("Link", "Auto triggre Linken").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"crystal_maiden_frostbite", true},
                {"item_sheepstick", true},
                {"item_force_staff", true},
                {"item_cyclone", true},
                {"item_rod_of_atos", true},
                {"item_dagon", true}
            })));
                items.AddItem(new MenuItem("atosRange", "Min Cast Range Atos").SetValue(new Slider(700, 750, 1400)));
                items.AddItem(new MenuItem("debuff", "Wait ethereal debuff").SetValue(true));
                healh.AddItem(new MenuItem("Healh", "Min healh % to ult").SetValue(new Slider(35, 10, 70)));
                orbwolk.AddItem(new MenuItem("orbwalk", "Orbwalk").SetValue(true));
                orbwolk.AddItem(new MenuItem("range", "Min. range to move target position.").SetValue(new Slider(50, 25, 100)));
                Menus.AddSubMenu(skills);
                Menus.AddSubMenu(items);
                Menus.AddSubMenu(ult);
                Menus.AddSubMenu(healh);
                Menus.AddSubMenu(autoA);
                Menus.AddSubMenu(orbwolk);
                drawing.AddItem(new MenuItem("Range Blink", "Draw range for Blink").SetValue(true));
                drawing.AddItem(new MenuItem("Range Crystal Nova", "Draw range for Crystal Nova").SetValue(true));
                drawing.AddItem(new MenuItem("Range Frostbite", "Draw range for Frostbite").SetValue(true));
                drawing.AddItem(new MenuItem("Range Freezing Field", "Draw range for Freezing Field").SetValue(true));
                Menus.AddSubMenu(drawing);
                Menus.AddToMainMenu();

                Success("Play It's all fun and games until someone's frozen solid.");
                DelayAction.Add(500, () =>
                {

                    if (Menus.Item("Range Blink").GetValue<bool>())
                    {
                        Menus.Item("Range Blink").SetValue(false);
                        Menus.Item("Range Blink").SetValue(true);
                    }
                    if (Menus.Item("Range Crystal Nova").GetValue<bool>())
                    {
                        Menus.Item("Range Crystal Nova").SetValue(false);
                        Menus.Item("Range Crystal Nova").SetValue(true);
                    }
                    if (Menus.Item("Range Frostbite").GetValue<bool>())
                    {
                        Menus.Item("Range Frostbite").SetValue(false);
                        Menus.Item("Range Frostbite").SetValue(true);
                    }
                    if (Menus.Item("Range Freezing Field").GetValue<bool>())
                    {
                        Menus.Item("Range Freezing Field").SetValue(false);
                        Menus.Item("Range Freezing Field").SetValue(true);
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
    }
}