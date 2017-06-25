using Ensage;
using Ensage.Common;
using Ensage.Common.Menu;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace AbilityMapHack
{
    public static class AbilityMapHack
    {
        private static Font Font;
        private static string heroName;
        private static readonly Menu Menu = new Menu("Ability MapHack", "Ability MapHack", true, "Ability MapHack", true).SetFontColor(Color.DarkRed);
        private static ParticleEffect range;
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
            if (hero.Name.Contains("npc_dota_hero_") || hero.Name.Contains("npc_dota_neutral"))
            {
                if (!args.ParticleEffect.Owner.IsVisible)
                {
                    if (hero.Name.Contains("npc_dota_hero_"))
                    heroName = args.ParticleEffect.Owner.Name;
                    string particleName = args.Name;
                    Task.Delay(50).ContinueWith(_1 =>
                    {
                        Vector3 vector = args.ParticleEffect.GetControlPoint(0);
                        string partHero = particleName.Substring(particleName.LastIndexOf(@"/") + 1).Replace(".vpcf", "");

                        List<Vector2> Position = new List<Vector2>();
                        Position.Add(HUDInfo.WorldToMinimap(vector));

                        Task.Delay(2000).ContinueWith(_2 =>
                        {
                            if (Position != null)
                                Position.RemoveAt(0);
                        });
                        foreach (var spell in TextureList)
                        {
                        if (partHero == spell && heroName != null)
                        {
                       
                        Drawing.OnEndScene += argst =>
                        {
                         if (Drawing.Direct3DDevice9 == null) return;
                             foreach (var pos in Position?.ToList())
                             {
                                 if (!pos.IsZero)
                                {
                                    string HeroName = FirstUpper(GetHeroName(heroName)).Replace("_", "");
                                    if (partHero != "generic_hit_blood")
                                     {
                                         Font.DrawText(null, HeroName, (int)pos.X - 17 + 2, (int)pos.Y - 16 + 2, Color.Gold);
                                         Font.DrawText(null, "♉", (int)pos.X - 7 + 2, (int)pos.Y - 8, Color.Red);
                                     }
                                     else if (partHero == "generic_hit_blood" && hero.Name.Contains("npc_dota_neutral"))
                                     {
                                         Font.DrawText(null, HeroName, (int)pos.X - 17 + 2, (int)pos.Y - 16 + 2, Color.Gold);
                                         Font.DrawText(null, "♉", (int)pos.X - 7 + 2, (int)pos.Y - 8, Color.Red);
                                     }
                                 }
                             }
                         };
                         
                        if (!vector.IsZero)
                                {
                                   
                                    if (partHero != "generic_hit_blood")
                                    {
                                        ParticleEffect range = new ParticleEffect(@"materials\ensage_ui\particles\range_display_mod.vpcf", vector);
                                        //ParticleEffect range = new ParticleEffect(particleName, vector);
                                        range.SetControlPoint(1, new Vector3(500, 255, 0));
                                        range.SetControlPoint(2, new Vector3(255, 0, 0));
                                        Task.Delay(2500).ContinueWith(_3 =>
                                        {
                                            if (range != null)
                                            {
                                                range.Dispose();
                                                range = null;
                                            }
                                        });
                                    }
                                    else if (partHero == "generic_hit_blood" && hero.Name.Contains("npc_dota_neutral"))
                                    {
                                        ParticleEffect range = new ParticleEffect(@"materials\ensage_ui\particles\range_display_mod.vpcf", vector);
                                        //ParticleEffect range = new ParticleEffect(particleName, vector);
                                        range.SetControlPoint(1, new Vector3(500, 255, 0));
                                        range.SetControlPoint(2, new Vector3(255, 0, 0));
                                        Task.Delay(2500).ContinueWith(_4 =>
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
            }
        }

        private static List<string> TextureList = new List<string>()
        {

                {"broodmother_spin_web_cast"},
                {"broodmother_huger_buff"},
                {"batrider_firefly_ember"},
                {"shredder_timber_chain_tree"},
                {"shredder_whirling_death"},
				{"death_prophet_silence_cast"},
                {"generic_hit_blood"},
                {"blink_dagger_start"},
                {"queen_blink_start"},
                {"blink_dagger_end"},
                {"queen_blink_end"},
                {"bounty_hunter_windwalk"},
                //{"antimage_blink_end"},
                //{"antimage_blink_start"},
                //{"invoker_ice_wall"},
                //{"invoker_emp"},
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
    }
}
