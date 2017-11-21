namespace StormSpirit
{
    using System;
    using System.Collections.Generic;

    using Ensage;
    using Ensage.Common.Menu;
    using Ensage.SDK.Menu;

    partial class ConfigInit
    {
        private bool disposed;
        public static void Success(string text, params object[] arguments)
        {
            Game.PrintMessage("<font color='#00ff00'>" + text + "</font>");
        }
        public ConfigInit()
        {
            Menu = MenuFactory.Create("StormSpirit by Vick", "item_orchid");
            SpellPanel = Menu.Menu("Spells");
            ItemPanel = Menu.Menu("Items");
            AutoEscapeMode = Menu.Menu("Auto Escape Mode");
            DrawingTargetMaker = Menu.Menu("Drawing Enemy Marker");
            DrawingDamagePanel = Menu.Menu("Drawing Damage Panel");
            DrawingRangeDisplay = Menu.Menu("Drawing Range Display");
            AntiLinkensSphere = Menu.Menu("Auto Anti Linkens Sphere");
            var items = new Dictionary<string, bool>
            {
                {"item_dagon_5", true},
                {"item_bloodthorn", true},
                {"item_mjollnir", true},
                {"item_ethereal_blade", true},
                {"item_veil_of_discord", true},
                {"item_rod_of_atos", true},
                {"item_sheepstick", true},
                {"item_arcane_boots", true},
                {"item_shivas_guard", true},
                {"item_lotus_orb", true},
                {"item_urn_of_shadows", true},
                {"item_soul_ring", true},
                {"item_cheese", true},
                {"item_black_king_bar", true}
            };
            var link = new Dictionary<string, bool>
            {
                {"item_sheepstick", true},
                {"item_force_staff", true},
                {"item_cyclone", true},
                {"item_rod_of_atos", true},
                {"item_dagon_5", true},
                {"storm_spirit_electric_vortex", true}
            };
            debuff = ItemPanel.Item("Wait ethereal debuff", true);
            var skills = new Dictionary<string, bool>
            {
                {"storm_spirit_static_remnant", true},
                {"storm_spirit_electric_vortex", true},
                {"storm_spirit_overload", true},
                {"storm_spirit_ball_lightning", true}
            };
            var escape = new Dictionary<string, bool>
            {
                {"item_travel_boots_2", true},
                {"storm_spirit_ball_lightning", true}
            };
            DrawingDamageEnabled = DrawingDamagePanel.Item("Show drawing damage to kill", true);
            DrawingTargetEnabled = DrawingTargetMaker.Item("Show target", true);

            DrawingRangeEnabled = DrawingRangeDisplay.Item("Show Spell Range", true);
            RangeElectricVortex = DrawingRangeDisplay.Item("Draw range for Electric Vortex", true);
            RangeStaticRemnant = DrawingRangeDisplay.Item("Draw range for Static Remnant", true);
            AutoAbilityEnabled = AutoEscapeMode.Item("Use abilities if i not have enough health.", true);

            AutoOverload = SpellPanel.Item("Use Overload if target disable?", false);
            savemanamode = SpellPanel.Item("The extended logic of using the ultimate on the Storm position.", true);
            savemanamode.Item.Tooltip = "Compares the rate of storm attack and the rate of use of the ability,logic chooses a more advantageous option.";
            fastVortex = SpellPanel.Item("Use instant Vortex", true);
            fastVortex.Item.Tooltip = "Ignoring the passive ability of Overload.";
            Heel = ItemPanel.Item("Min targets to BKB|Lotus", new Slider(2, 1, 5));
            
            AbilityToggler = SpellPanel.Item("Spells", new AbilityToggler(skills));
            ItemToggler = ItemPanel.Item("Items", new AbilityToggler(items));
            Link = AntiLinkensSphere.Item("Auto triggre Linken", new AbilityToggler(link));
            RHealh = AutoEscapeMode.Item("Minimum health in percent to escape.", new Slider(15, 02, 50));
            Escape = AutoEscapeMode.Item("Use abilities if I not have enough health.", new AbilityToggler(escape));
            Key = Menu.Item("Combo Key", new KeyBind(68));
            Success("Zip! Zap! Ha ha ha ha! Whoa-ho-ho! Ooh, who's that handsome devil?");
        }

        public MenuFactory Menu { get; set; }
        public MenuFactory SpellPanel { get; set; }
        public MenuFactory ItemPanel { get; set; }
        public MenuFactory DrawingTargetMaker { get; set; }
        public MenuFactory DrawingDamagePanel { get; set; }
        public MenuFactory DrawingRangeDisplay { get; set; }
        public MenuFactory AutoEscapeMode { get; set; }
        public MenuFactory AntiLinkensSphere { get; set; }

        public MenuItem<bool> DrawingDamageEnabled { get; }

        public MenuItem<bool> RangeBlink { get; }
        public MenuItem<bool> RangeElectricVortex { get; }
        public MenuItem<bool> RangeStaticRemnant { get; }
        
        public MenuItem<bool> DrawingTargetEnabled { get; }
        public MenuItem<bool> DrawingRangeEnabled { get; }
        public MenuItem<bool> debuff { get; }
        public MenuItem<bool> AutoOverload { get; }
        public MenuItem<bool> fastVortex { get; }
        public MenuItem<bool> savemanamode { get; }
        public MenuItem<bool> AutoAbilityEnabled { get; }
        public MenuItem<Slider> RHealh { get; }
        
        public MenuItem<AbilityToggler> AbilityToggler { get; }

        public MenuItem<Slider> Heel { get; }

        public MenuItem<KeyBind> Key { get; }

        public MenuItem<AbilityToggler> ItemToggler { get; }
        public MenuItem<AbilityToggler> Link { get; }
        public MenuItem<AbilityToggler> Escape { get; }

        public MenuItem<Slider> DistanceForUlt { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                Menu.Dispose();
            }

            disposed = true;
        }
    }
}