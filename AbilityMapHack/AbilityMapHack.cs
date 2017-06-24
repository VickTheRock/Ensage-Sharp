using Ensage;
using Ensage.Common;
using Ensage.Common.Menu;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
namespace AbilityMapHack
{
    public static class AbilityMapHack
    {
        private static Font Font;
        private static string heroName, particleName;
        private static Vector3 vector;
        private static readonly Menu Menu = new Menu("Ability MapHack", "Ability MapHack", true, "Ability MapHack", true).SetFontColor(Color.DarkRed);

        static void Main()
        {
            Events.OnLoad += EventOnLoad;
            Events.OnClose += EventOnClose;
        }
        public static string GetHeroName(string Name)
        {
            return Name.Split(new[] { "npc_dota_hero_" }, StringSplitOptions.None)[1];
        }
        public static string FirstUpper(string str)
        {
            string[] s = str.Split(' ');

            for (int i = 0; i < s.Length; ++i)
            {
                if (s[i].Length > 1)
                    s[i] = s[i].Substring(0, 1).ToUpper() + s[i].Substring(1, s[i].Length - 1).ToLower();
                else s[i] = s[i].ToUpper();
            }
            str = string.Join(" ", s);

            string[] s1 = str.Split('_');

            for (int i = 0; i < s1.Length; ++i)
            {
                if (s1[i].Length > 1)
                    s1[i] = s1[i].Substring(0, 1).ToUpper() + s1[i].Substring(1, s1[i].Length - 1).ToLower();
                else s1[i] = s1[i].ToUpper();
            }
            return string.Join("_", s1);
        }
        private static void EventOnClose(object sender, EventArgs eventArgs)
        {
            Menu.RemoveFromMainMenu();
        }
        private static void EventOnLoad(object sender, EventArgs eventArgs)
        {
            Menu.AddItem(new MenuItem("enable", "Enable").SetValue(true));
            Menu.AddToMainMenu();

            Entity.OnParticleEffectAdded += OnParticleEvent;
            Events.OnLoad -= EventOnLoad;

            Drawing.OnPostReset += argst => { Font.OnResetDevice(); };
            Drawing.OnPreReset += argst => { Font.OnLostDevice(); };

            if (Drawing.RenderMode == RenderMode.Dx9)
            {
                Font = new Font(Drawing.Direct3DDevice9,
                    new FontDescription
                    {
                        FaceName = "Arial",
                        Height = 11,
                        OutputPrecision = FontPrecision.Default,
                        Quality = FontQuality.Default,
                        CharacterSet = FontCharacterSet.Default,
                        MipLevels = 3,
                        PitchAndFamily = FontPitchAndFamily.Decorative,
                        Weight = FontWeight.Black,
                    });
            }
        }

        private static void OnParticleEvent(Entity hero, ParticleEffectAddedEventArgs args)
        {
            try
            {
                if (hero.Name.Contains("npc_dota_hero_") && Menu.Item("enable").GetValue<bool>())
                {
                    DelayAction.Add(50, () =>
                    {
                       vector = args.ParticleEffect.GetControlPoint(0);
                        List<Vector2> Position = new List<Vector2>();

                        if (!args.ParticleEffect.Owner.IsVisible)
                        {
                            heroName = args.ParticleEffect.Owner.Name;
                            Hero MyHero = ObjectManager.LocalHero;

                             particleName = args.Name;
                            //Console.WriteLine("eff4" + eff4);
                            string partHero = particleName.Substring(particleName.LastIndexOf(@"/") + 1).Replace(".vpcf", "");

                            var result = TextureList.ToArray();
                            foreach (var spell in result)
                            {
                                if (spell == partHero)
                                {
                                    Position.Add(HUDInfo.WorldToMinimap(vector));
                                    DelayAction.Add(2500, () =>
                                    {
                                        Position.RemoveAt(0);
                                    });
                                    Drawing.OnEndScene += argst =>
                                    {
                                        if (Drawing.Direct3DDevice9 == null) return;
                                        foreach (var pos in Position.ToList())
                                        {
                                            if (!pos.IsZero)
                                            {
                                                var HeroName = FirstUpper(GetHeroName(heroName)).Replace("_", "");
                                                Font.DrawText(null, HeroName, (int)pos.X - 17 + 2, (int)pos.Y - 16 + 2, Color.Gold);
                                                Font.DrawText(null, "♉", (int)pos.X - 7 + 2, (int)pos.Y - 8, Color.Red);
                                                //Drawing.DrawRect(pos - 25, new Vector2(7, 7), Drawing.GetTexture("materials/ensage_ui/miniheroes/" + eff5.Substring(eff5.LastIndexOf("hero_") + 5) + ".vmat_c"));
                                                //Console.WriteLine("materials/ensage_ui/miniheroes/" + eff5.Substring(eff5.LastIndexOf("hero_") + 5) + ".vmat_c");

                                                //Drawing.DrawRect(Drawing.WorldToScreen(vector) - 25, new Vector2(50, 50), Drawing.GetTexture("materials/ensage_ui/miniheroes/" + eff5.Substring(eff5.LastIndexOf("hero_") + 5) + ".vmat"));

                                                //Drawing.DrawRect(pos, new Vector2(10, 10), Drawing.GetTexture("materials/ensage_ui/miniheroes/" + eff5.Substring(eff5.LastIndexOf("hero_") + 5) + ".vmat"));
                                            }
                                        }
                                    };

                                    if (!vector.IsZero)
                                    {
										ParticleEffect range = new ParticleEffect(@"materials\ensage_ui\particles\range_display_mod.vpcf", vector);
                                        //ParticleEffect range = new ParticleEffect(particleName, vector);
                                        range.SetControlPoint(1, new Vector3(500, 255, 0));
                                        range.SetControlPoint(2, new Vector3(255, 0, 0));
                                        DelayAction.Add(2500, () =>
                                        {
                                            if (range != null)
                                            {
                                                range.Dispose();
                                                range = null;
                                            }
                                        });
                                    }
                                }
                            }
                        }
                    });
                }
                if (hero.Name.Contains("npc_dota_neutral"))
                {
                    DelayAction.Add(50, () =>
                    {
                        var GetControlPoint = args.ParticleEffect.GetControlPoint(0);
                        List<Vector2> Position = new List<Vector2>();

                        if (!args.ParticleEffect.Owner.IsVisible)
                        {
                            //if(!GetControlPoint.IsZero)
                            //{
                            Position.Add(HUDInfo.WorldToMinimap(GetControlPoint));
                            //}
                            DelayAction.Add(1000, () =>
                            {
                                Position.RemoveAt(0);
                            });

                            Drawing.OnEndScene += argst =>
                            {
                                if (Drawing.Direct3DDevice9 == null) return;
                                foreach (var pos in Position?.ToList())
                                {
                                    if (!pos.IsZero)
                                    {
                                        var HeroName = FirstUpper(GetHeroName(heroName)).Replace("_", "");
                                        Font.DrawText(null, HeroName, (int)pos.X - 12 + 2, (int)pos.Y - 13 + 2, Color.Gold);
                                    }
                                }
                            };
                            if (!GetControlPoint.IsZero)
                            {
                                ParticleEffect range = new ParticleEffect(@"materials\ensage_ui\particles\range_display_mod.vpcf", GetControlPoint);
                                range.SetControlPoint(1, new Vector3(450, 255, 255));
                                range.SetControlPoint(2, new Vector3(255, 0, 0));
                                DelayAction.Add(500, () =>
                                {
                                    if (range != null)
                                    {
                                        range.Dispose();
                                        range = null;
                                    }
                                });
                            }
                        }
                    });
                }
            }
            catch (Exception)
            {
            }
        }

        private static List<string> TextureList = new List<string>()
        {
                {"blink_dagger_start"},
                {"queen_blink_start"},
                {"bounty_hunter_windwalk"},
                {"antimage_blink_start"},
                {"invoker_ice_wall"},
                {"invoker_emp"},
                {"invoker_quas_orb"},
                {"invoker_wex_orb"},
                {"invoker_exort_orb"},
                {"legion_commander_odds"},
                {"tiny_toss"},
                {"earthshaker_fissure"},
                {"shredder_tree_dmg"},
                {"shredder_chakram_stay"},
                {"shredder_chakram_return"},
                {"sandking_burrowstrike"},
                {"sandking_sandstorm"},
                {"alchemist_unstable_concoction_timer"},
                {"alchemist_acid_spray"},
                {"clinkz_windwalk"},
                {"clinkz_death_pact"},
                {"razor_plasmafield"},
                {"venomancer_ward_cast"},
                {"meepo_poof_end"},
                {"slark_dark_pact_pulses"},
                {"slark_pounce_start"},
                {"ember_spirit_sleight_of_fist_cast"},
                {"zuus_arc_lightning_head"},
                {"zuus_thundergods_wrath_start"},
                //{"zuus_lightning_bolt"},
                {"zuus_lightning_bolt_start"},
                {"lina_spell_light_strike_array"},
                {"leshrac_split_earth"},
                {"disruptor_kineticfield_formation"},
                {"jakiro_ice_path"},
                {"jakiro_dual_breath_ice"},
                {"maiden_crystal_nova"},
                {"phantom_assassin_phantom_strike_start"},
                {"phantom_assassin_phantom_strike_end"},
                {"stormspirit_ball_lightning"},
                {"ancient_apparition_ice_blast_final"},
                {"ancient_apparition_chilling_touch"},
                {"ancient_ice_vortex"},
                {"weaver_shukuchi_damage"},
                {"weaver_shukuchi_start"},
                {"pudge_meathook"},
                {"rattletrap_cog_ambient"},
                {"rattletrap_hookshot"},
                {"rattletrap_rocket_flare"},
                {"tinker_rearm"},
                {"luna_lucent_beam"},
                {"pugna_netherblast"},
                {"monkey_king_strike_cast"},
                {"monkey_king_jump_trail"},
                {"monkey_king_jump_launch_ring"},
                {"monkey_king_spring_channel"},
                {"axe_counterhelix"},
                {"legion_commander_courage_hit"},
                {"legion_commander_press_owner"},
                {"stormspirit_overload_discharge"},
                {"tinker_missile_dud"},
                {"tidehunter_anchor_hero"},
                {"techies_remote_mine_plant"},
                {"techies_stasis_trap_plant"},
                {"monkey_king_jump_launch_ring"},
        };
        /*
        private static List<string> SpellList = new List<string>()
        {
                {"nyx_assassin_vendetta"},
                {"queenofpain_blink"},
                {"bounty_hunter_wind_walk"},
                {"antimage_blink"},
                {"invoker_ice_wall"},
                {"invoker_emp"},
                {"invoker_quas"},
                {"invoker_wex"},
                {"invoker_exort"},
                {"legion_commander_overwhelming_odds"},
                {"tiny_avalanche"},
                {"tiny_toss"},
                {"earthshaker_fissure"},
                {"shredder_timber_chain"},
                {"shredder_chakram"},
                {"sandking_burrowstrike"},
                {"sandking_sand_storm"},
                {"alchemist_unstable_concoction"},
                {"alchemist_acid_spray"},
                {"clinkz_wind_walk"},
                {"clinkz_death_pact"},
                {"razor_plasma_field"},
                {"venomancer_plague_ward"},
                {"meepo_poof"},
                {"slark_dark_pact"},
                {"slark_pounce"},
                {"ember_spirit_sleight_of_fist"},
                {"zuus_arc_lightning"},
                {"zuus_thundergods_wrath"},
                {"zuus_lightning_bolt"},
                {"lina_light_strike_array"},
                {"leshrac_split_earth"},
                {"disruptor_kinetic_field"},
                {"jakiro_ice_path"},
                {"jakiro_dual_breath"},
                {"crystal_maiden_crystal_nova"},
                {"phantom_assassin_phantom_strike"},
                {"phantom_assassin_phantom_strike"},
                {"storm_spirit_ball_lightning"},
                {"ancient_apparition_ice_blast"},
                {"ancient_apparition_chilling_touch"},
                {"ancient_apparition_ice_vortex"},
                {"weaver_shukuchi"},
                {"pudge_meat_hook"},
                {"rattletrap_power_cogs"},
                {"rattletrap_hookshot"},
                {"rattletrap_rocket_flare"},
                {"tinker_rearm"},
                {"luna_lucent_beam"},
                {"pugna_nether_blast"},
                {"monkey_king_boundless_strike"},
                {"monkey_king_tree_dance"},
                {"monkey_king_primal_spring"},
                {"axe_counter_helix"},
                {"legion_commander_moment_of_courage"},
                {"legion_commander_press_the_attack"},
                {"storm_spirit_overload"},
                {"tinker_heat_seeking_missile"},
                {"tidehunter_anchor_smash"},
                {"techies_remote_mines"},
                {"techies_stasis_trap"},

        };*/
    }
}
