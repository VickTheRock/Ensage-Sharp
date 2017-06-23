namespace CrystalMaiden
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Ensage;
    using SharpDX;
    using Ensage.Common.Extensions;
    using Ensage.Common;
    using Ensage.Common.Menu;
    using static Toolset;
    public class CrystalMaiden
    {


        private static void Main()
        {
            Events.OnLoad += (sender, args) =>
            {
                if (ObjectManager.LocalHero.ClassId != ClassId.CDOTA_Unit_Hero_CrystalMaiden) return;
                me = ObjectManager.LocalHero;
                Init();
                Game.OnUpdate += Action;
               Drawing.OnDraw += DrawUltiDamage;
            };
            Events.OnClose += (sender, args) =>
            {
               
                Drawing.OnDraw -= DrawUltiDamage;
                me = null;
                e = null;
                Effect?.Dispose();
                Effect = null;
                Menus.RemoveFromMainMenu();
                Game.OnUpdate -= Action;
            };
        }


        private static void DrawingOnCore()
        {
            if (Menus.Item("Range Blink").GetValue<bool>() && blink != null)
            {
                blinkRange = 1200;
                if (BlinkRange == null)
                {
                    if (me.IsAlive)
                    {
                        BlinkRange = me.AddParticleEffect("materials/ensage_ui/particles/range_display_mod.vpcf");

                        BlinkRange.SetControlPoint(3, new Vector3(5, 0, 0));
                        BlinkRange.SetControlPoint(2, new Vector3(255, 0, 222));
                        BlinkRange.SetControlPoint(1, new Vector3(blinkRange, 0, 222));
                    }
                }
                else
                {
                    if (!me.IsAlive)
                    {
                        BlinkRange.Dispose();
                        BlinkRange = null;
                    }
                    else if (lastblinkRange != blinkRange)
                    {
                        BlinkRange.Dispose();
                        lastblinkRange = blinkRange;
                        BlinkRange = me.AddParticleEffect("materials/ensage_ui/particles/range_display_mod.vpcf");

                        BlinkRange.SetControlPoint(3, new Vector3(5, 0, 0));
                        BlinkRange.SetControlPoint(2, new Vector3(255, 0, 222));
                        BlinkRange.SetControlPoint(1, new Vector3(blinkRange, 0, 222));
                    }
                }
            }
            else
            {
                if (BlinkRange != null) BlinkRange.Dispose();
                BlinkRange = null;
            }
            if (Menus.Item("Range Crystal Nova").GetValue<bool>() && Q != null && Q.Level > 0)
            {
                qRange = Q.GetCastRange();
                if (QRange == null)
                {
                    if (me.IsAlive)
                    {
                        QRange = me.AddParticleEffect("materials/ensage_ui/particles/range_display_mod.vpcf");

                        QRange.SetControlPoint(3, new Vector3(5, 0, 0));
                        QRange.SetControlPoint(2, new Vector3(152, 92, 255));
                        QRange.SetControlPoint(1, new Vector3(qRange, 0, 222));
                    }
                }
                else
                {
                    if (!me.IsAlive)
                    {
                        QRange.Dispose();
                        QRange = null;
                    }
                    else if (lastqRange != qRange)
                    {
                        QRange.Dispose();
                        lastqRange = qRange;
                        QRange = me.AddParticleEffect("materials/ensage_ui/particles/range_display_mod.vpcf");

                        QRange.SetControlPoint(3, new Vector3(5, 0, 0));
                        QRange.SetControlPoint(2, new Vector3(152, 92, 255));
                        QRange.SetControlPoint(1, new Vector3(qRange, 0, 222));
                    }
                }
            }
            else
            {
                if (QRange != null) QRange.Dispose();
                QRange = null;
            }
            if (Menus.Item("Range Frostbite").GetValue<bool>() && W != null && W.Level > 0)
            {
                wRange = W.GetCastRange();
                if (WRange == null)
                {
                    if (me.IsAlive)
                    {
                        WRange = me.AddParticleEffect("materials/ensage_ui/particles/range_display_mod.vpcf");

                        WRange.SetControlPoint(3, new Vector3(5, 0, 0));
                        WRange.SetControlPoint(2, new Vector3(64, 224, 208));
                        WRange.SetControlPoint(1, new Vector3(wRange, 0, 222));
                    }
                }
                else
                {
                    if (!me.IsAlive)
                    {
                        WRange.Dispose();
                        WRange = null;
                    }
                    else if (lastwRange != wRange)
                    {
                        WRange.Dispose();
                        lastwRange = wRange;
                        WRange = me.AddParticleEffect("materials/ensage_ui/particles/range_display_mod.vpcf");

                        WRange.SetControlPoint(3, new Vector3(5, 0, 0));
                        WRange.SetControlPoint(2, new Vector3(64, 224, 208));
                        WRange.SetControlPoint(1, new Vector3(wRange, 0, 222));
                    }
                }
            }
            else
            {
                if (WRange != null) WRange.Dispose();
                WRange = null;
            }
            if (Menus.Item("Range Freezing Field").GetValue<bool>() && R != null && R.Level >0)
            {
                rRange = R.GetCastRange();
                if (RRange == null)
                {
                    if (me.IsAlive)
                    {
                        RRange = me.AddParticleEffect("materials/ensage_ui/particles/range_display_mod.vpcf");

                        RRange.SetControlPoint(3, new Vector3(5, 0, 0));
                        RRange.SetControlPoint(2, new Vector3(255, 0, 22));
                        RRange.SetControlPoint(1, new Vector3(rRange, 0, 222));
                    }
                }
                else
                {
                    if (!me.IsAlive)
                    {
                        RRange.Dispose();
                        RRange = null;
                    }
                    else if (lastrRange != rRange)
                    {
                        RRange.Dispose();
                        lastrRange = rRange;
                        RRange = me.AddParticleEffect("materials/ensage_ui/particles/range_display_mod.vpcf");

                        RRange.SetControlPoint(3, new Vector3(5, 0, 0));
                        RRange.SetControlPoint(2, new Vector3(255, 0, 22));
                        RRange.SetControlPoint(1, new Vector3(rRange, 0, 222));
                    }
                }
            }
            else
            {
                if (RRange != null) RRange.Dispose();
                RRange = null;
            }
        }
        private static void Action(EventArgs args)
        {
            Q = me.Spellbook.SpellQ;
            W = me.Spellbook.SpellW;
            R = me.Spellbook.SpellR;
            // Item
            ethereal = me.FindItem("item_ethereal_blade");
            cyclone = me.FindItem("item_cyclone");
            force = me.FindItem("item_force_staff");
            urn = me.FindItem("item_urn_of_shadows");
            glimmer = me.FindItem("item_glimmer_cape");
            bkb = me.FindItem("item_black_king_bar");
            mail = me.FindItem("item_blade_mail");
            lotus = me.FindItem("item_lotus_orb");
            vail = me.FindItem("item_veil_of_discord");
            cheese = me.FindItem("item_cheese");
            ghost = me.FindItem("item_ghost");
            orchid = me.FindItem("item_orchid") ?? me.FindItem("item_bloodthorn");
            atos = me.FindItem("item_rod_of_atos");
            soul = me.FindItem("item_soul_ring");
            arcane = me.FindItem("item_arcane_boots");
            blink = me.FindItem("item_blink");
            shiva = me.FindItem("item_shivas_guard");
            shadB = me.FindItem("item_silver_edge") ?? me.FindItem("item_invis_sword");
            dagon = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));
            e = TargetSelector.ClosestToMouse(me);

            if (e == null || !e.IsValid || !e.IsAlive) return;
            if (Effect == null || !Effect.IsValid)
            {
                Effect = new ParticleEffect(@"particles\ui_mouseactions\range_finder_tower_aoe.vpcf", e);
                Effect.SetControlPoint(2, new Vector3(me.Position.X, me.Position.Y, me.Position.Z));
                Effect.SetControlPoint(6, new Vector3(1, 0, 0));
                Effect.SetControlPoint(7, new Vector3(e.Position.X, e.Position.Y, e.Position.Z));
            }
            else
            {
                Effect.SetControlPoint(2, new Vector3(me.Position.X, me.Position.Y, me.Position.Z));
                Effect.SetControlPoint(6, new Vector3(1, 0, 0));
                Effect.SetControlPoint(7, new Vector3(e.Position.X, e.Position.Y, e.Position.Z));
            }
            Active = Game.IsKeyDown(Menus.Item("keyBind").GetValue<KeyBind>().Key);
            


            var noUlty = !R.IsInAbilityPhase && !R.IsChanneling && !me.IsChanneling() &&
                         !me.HasModifier("modifier_crystal_maiden_freezing_field");
           
            if (Menus.Item("autoUlt").IsActive() && me.IsAlive)
            {
                AutoAbilities();
            }
            if (Menus.Item("AutoUsage").IsActive())
            {
                AutoSpells();
            }

            if (Active && noUlty)
            {
                try
                {
                    var enemies = ObjectManager.GetEntities<Hero>()
                .Where(x => x.IsAlive && x.Team != me.Team && !x.IsIllusion && !x.IsMagicImmune()
                            && (!x.HasModifier("modifier_abaddon_borrowed_time")
                                || !x.HasModifier("modifier_item_blade_mail_reflect")))
                .ToList();
                    if (e.HasModifier("modifier_abaddon_borrowed_time")
                        || e.HasModifier("modifier_item_blade_mail_reflect")
                        || e.IsMagicImmune())
                    {

                        e = GetClosestToTarget(enemies, e) ?? null;
                        if (Utils.SleepCheck("spam"))
                        {
                            Utils.Sleep(5000, "spam");
                        }
                    }
                    if (e == null) return;
                    //spell


                    sheep = e.ClassId == ClassId.CDOTA_Unit_Hero_Tidehunter ? null : me.FindItem("item_sheepstick");

                    var stoneModif = e.HasModifier("modifier_medusa_stone_gaze_stone");

                    var stayHere = e.HasModifiers(new[]
                            {
                            "modifier_rod_of_atos_debuff",
                            "modifier_crystal_maiden_frostbite"
                            }, false);
                    var ally = ObjectManager.GetEntities<Hero>().Where(x => x.IsAlive && x.Team == me.Team && !x.IsIllusion).ToList();

                    var n = GetClosestToTarget(ally, e);
                    if (me.IsAlive && e.IsAlive && me.CanCast() && me.Distance2D(e)<= 1400 && Utils.SleepCheck("combosleep"))
                    {
                        Utils.Sleep(350, "combosleep");
                        if (R.IsInAbilityPhase || R.IsChanneling || me.IsChanneling())
                            return;
                        if (stoneModif) return;
                        //var noBlade = e.HasModifier("modifier_item_blade_mail_reflect");
                        
                        if (e.IsVisible && me.Distance2D(e) <= 2300)
                        {
                                var distance = me.IsVisibleToEnemies ? 1400 : W.GetCastRange() + me.HullRadius;

                                float angle = me.FindAngleBetween(e.Position, true);
                                Vector3 pos = new Vector3((float)(e.Position.X - 250 * Math.Cos(angle)),
                                    (float)(e.Position.Y - 250 * Math.Sin(angle)), 0);
                                if (e.IsLinkensProtected())
                                    linken(e);
                                if (
                                    // cheese
                                    cheese != null
                                    && cheese.CanBeCasted()
                                    && me.Health <= me.MaximumHealth * 0.3
                                    && me.Distance2D(e) <= 700
                                    && Menus.Item("Items").GetValue<AbilityToggler>().IsEnabled(cheese.Name)
                                    )
                                    cheese.UseAbility();
                                if ( // SoulRing Item 
                                    soul != null
                                    && soul.CanBeCasted()
                                    && Menus.Item("Items").GetValue<AbilityToggler>().IsEnabled(soul.Name)
                                    && me.CanCast()
                                    && me.Health >= me.MaximumHealth * 0.4
                                    && me.Mana <= R.ManaCost
                                    )
                                    soul.UseAbility();
                                if ( // Arcane Boots Item
                                    arcane != null
                                    && arcane.CanBeCasted()
                                    && Menus.Item("Items").GetValue<AbilityToggler>().IsEnabled(arcane.Name)
                                    && me.CanCast()
                                    && me.Mana <= me.MaximumMana * 0.5
                                    )
                                    arcane.UseAbility();
                                if ( //Ghost
                                    ghost != null
                                    && ghost.CanBeCasted()
                                    && me.CanCast()
                                    && me.Distance2D(e) < me.AttackRange
                                    && (me.Health <= me.MaximumHealth * 0.5 
                                    || ((enemies.Count(x => x.Distance2D(me) <= 650) <=
                                    Menus.Item("Heel").GetValue<Slider>().Value 
                                    || !Menus.Item("Items").GetValue<AbilityToggler>().IsEnabled(bkb.Name)
                                    || bkb == null || !bkb.CanBeCasted())
                                    && R.CanBeCasted() && Menus.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)))
                                    && Menus.Item("Items").GetValue<AbilityToggler>().IsEnabled(ghost.Name)
                                    && !me.IsMagicImmune()
                                    )
                                    ghost.UseAbility();
                                if (blink != null
                                    && W.CanBeCasted()
                                    && me.CanCast()
                                    && blink.CanBeCasted()
                                    && Menus.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name)
                                    && me.Distance2D(e) >= 490
                                    && me.Distance2D(pos) <= 1180
                                    )
                                    blink.UseAbility(pos);
                                uint elsecount = 0;
                                elsecount += 1;
                                if (elsecount == 1
                                    && urn != null && urn.CanBeCasted() && urn.CurrentCharges > 0
                                    && me.Distance2D(e) <= urn.GetCastRange() 
                                    && Menus.Item("Items").GetValue<AbilityToggler>().IsEnabled(urn.Name)
                                    )
                                    urn.UseAbility(e);
                                else elsecount += 1;
                                if (elsecount == 2
                                    && glimmer != null
                                    && glimmer.CanBeCasted()
                                    && me.Distance2D(e) <= 900 
                                    && Menus.Item("Items").GetValue<AbilityToggler>().IsEnabled(glimmer.Name)
                                    )
                                    glimmer.UseAbility(me);
                                else elsecount += 1;
                                if (elsecount == 3
                                    && mail != null
                                    && mail.CanBeCasted()
                                    && enemies.Count(x => x.Distance2D(me) <= 700) >=
                                    Menus.Item("Heel").GetValue<Slider>().Value
                                    && Menus.Item("Items").GetValue<AbilityToggler>().IsEnabled(mail.Name)
                                    )
                                    mail.UseAbility();
                                else elsecount += 1;
                                if (elsecount == 4
                                    && bkb != null
                                    && bkb.CanBeCasted()
                                    && enemies.Count(x => x.Distance2D(me) <= 700) >=
                                    Menus.Item("Heel").GetValue<Slider>().Value
                                    && Menus.Item("Items").GetValue<AbilityToggler>().IsEnabled(bkb.Name)
                                    )
                                    bkb.UseAbility();
                                else elsecount += 1;
                                if (elsecount == 5
                                    && lotus != null
                                    && lotus.CanBeCasted()
                                    && enemies.Count(x => x.Distance2D(me) <= 700) >= 1
                                    && Menus.Item("Items").GetValue<AbilityToggler>().IsEnabled(lotus.Name)
                                    )
                                    lotus.UseAbility(me);
                                else elsecount += 1;
                                if (elsecount == 6
                                    && sheep != null
                                    && sheep.CanBeCasted()
                                    && me.CanCast()
                                    && !e.IsLinkensProtected()
                                    && !e.IsMagicImmune()
                                    && Menus.Item("Items").GetValue<AbilityToggler>().IsEnabled(sheep.Name)
                                    )
                                    sheep.UseAbility(e);
                                else elsecount += 1;
                                if (elsecount == 7
                                    && vail != null
                                    && vail.CanBeCasted()
                                    && Menus.Item("Items").GetValue<AbilityToggler>().IsEnabled(vail.Name)
                                    && me.CanCast()
                                    && !e.IsMagicImmune()
                                    && (me.Distance2D(e) < Q.GetCastRange()+50
                                    || n.Distance2D(e) <= 300 && me.Distance2D(e) <= 1200)
                                    )
                                    vail.UseAbility(e.Position);
                                else elsecount += 1;
                                if (elsecount == 8
                                    && orchid != null
                                    && orchid.CanBeCasted()
                                    && me.CanCast()
                                    && !e.IsLinkensProtected()
                                    && !e.IsMagicImmune()
                                    && Menus.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_bloodthorn")
                                    )
                                    orchid.UseAbility(e);
                                else elsecount += 1;
                                if (elsecount == 9
                                    && ethereal != null
                                    && ethereal.CanBeCasted()
                                    && me.CanCast()
                                    && !e.IsLinkensProtected()
                                    && !e.IsMagicImmune()
                                    && Menus.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name)
                                    )
                                {
                                    if (Menus.Item("debuff").GetValue<bool>())
                                    {
                                        var speed = ethereal.GetAbilityData("projectile_speed");
                                        var time = e.Distance2D(me) / speed;
                                        Utils.Sleep((int)(time * 1000.0f + Game.Ping) + 30, "combosleep");
                                        ethereal.UseAbility(e);
                                    }
                                    else
                                    {
                                        ethereal.UseAbility(e);
                                        Utils.Sleep(130, "combosleep");
                                    }
                                }
                                else elsecount += 1;
                                if (elsecount == 10
                                    && Q != null
                                    && Q.CanBeCasted()
                                    && Menus.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
                                    && me.CanCast()
                                    && !e.IsMagicImmune()
                                    )
                                    Q.UseAbility(e.Position);
                                elsecount += 1;
                                if (elsecount == 11
                                    && atos != null
                                    && atos.CanBeCasted()
                                    && me.CanCast()
                                    && !stayHere
                                    && !e.IsLinkensProtected()
                                    && !e.IsMagicImmune()
                                    && Menus.Item("Items").GetValue<AbilityToggler>().IsEnabled(atos.Name)
                                    && (me.Distance2D(e) < distance
                                    || n.Distance2D(e) <= 300 && me.Distance2D(e) <= 1400
                                    || me.Distance2D(e) <= Menus.Item("atosRange").GetValue<Slider>().Value)
                                    )
                                    atos.UseAbility(e);
                                else elsecount += 1;
                                if (elsecount == 12
                                    && W != null
                                    && W.CanBeCasted()
                                    && me.CanCast()
                                    && !stayHere
                                    && !e.IsMagicImmune()
                                    && Menus.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
                                    )
                                    W.UseAbility(e);
                                else elsecount += 1;
                                if (elsecount == 13
                                    && shiva != null
                                    && shiva.CanBeCasted()
                                    && me.CanCast()
                                    && !e.IsMagicImmune()
                                    && Menus.Item("Items").GetValue<AbilityToggler>().IsEnabled(shiva.Name)
                                    && me.Distance2D(e) <= 600
                                    )
                                    shiva.UseAbility();
                                else elsecount += 1;
                                if (elsecount == 14
                                    && dagon != null
                                    && dagon.CanBeCasted()
                                    && Menus.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon")
                                    && me.CanCast()
                                    && (ethereal == null
                                    || e.HasModifier("modifier_item_ethereal_blade_slow")
                                    || ethereal.Cooldown < 17)
                                    && !e.IsLinkensProtected()
                                    && !e.IsMagicImmune()
                                    )
                                    dagon.UseAbility(e);
                                else elsecount += 1;
                                if (elsecount == 15
                                    && R != null
                                    && R.CanBeCasted()
                                    && me.Distance2D(e) <= R.GetCastRange() - 100
                                    && !me.HasModifier("modifier_pugna_nether_ward_aura")
                                    && (e.HasModifiers(new[]
                                    {
                                    "modifier_rod_of_atos_debuff",
                                    "modifier_meepo_earthbind",
                                    "modifier_pudge_dismember",
                                    "modifier_lone_druid_spirit_bear_entangle_effect",
                                    "modifier_legion_commander_duel",
                                    "modifier_kunkka_torrent",
                                    "modifier_ice_blast",
                                    "modifier_crystal_maiden_crystal_nova",
                                    "modifier_enigma_black_hole_pull",
                                    "modifier_ember_spirit_searing_chains",
                                    "modifier_dark_troll_warlord_ensnare",
                                    "modifier_crystal_maiden_frostbite",
                                    "modifier_axe_berserkers_call",
                                    "modifier_bane_fiends_grip",
                                    "modifier_faceless_void_chronosphere_freeze",
                                    "modifier_storm_spirit_electric_vortex_pull",
                                    "modifier_naga_siren_ensnare"
                                    }, false)
                                    || e.ClassId != ClassId.CDOTA_Unit_Hero_FacelessVoid
                                        && !e.HasModifier("modifier_faceless_void_chronosphere_freeze")
                                        || e.IsStunned()
                                        || e.IsHexed()
                                        || e.IsRooted())
                                    && !e.HasModifiers(new[]
                                    {
                                    "modifier_medusa_stone_gaze_stone",
                                    "modifier_faceless_void_time_walk",
                                    "modifier_item_monkey_king_bar",
                                    "modifier_rattletrap_battery_assault",
                                    "modifier_item_blade_mail_reflect",
                                    "crystal_maiden_frostbite",
                                    "modifier_pudge_meat_hook",
                                    "modifier_zuus_lightningbolt_vision_thinker",
                                    "modifier_puck_phase_shift",
                                    "modifier_eul_cyclone",
                                    "modifier_dazzle_shallow_grave",
                                    "modifier_mirana_leap",
                                    "modifier_abaddon_borrowed_time",
                                    "modifier_winter_wyvern_winters_curse",
                                    "modifier_earth_spirit_rolling_boulder_caster",
                                    "modifier_brewmaster_storm_cyclone",
                                    "modifier_spirit_breaker_charge_of_darkness",
                                    "modifier_shadow_demon_disruption",
                                    "modifier_tusk_snowball_movement",
                                    "modifier_invoker_tornado",
                                    "modifier_obsidian_destroyer_astral_imprisonment_prison"
                                    }, false)
                                    && (e.FindSpell("abaddon_borrowed_time")?.Cooldown > 0
                                        || e.FindSpell("abaddon_borrowed_time") == null)

                                    && !e.IsMagicImmune()
                                    && e.Health >= e.MaximumHealth / 100 * Menus.Item("Healh").GetValue<Slider>().Value
                                    && Menus.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
                                )
                                {
                                    R.UseAbility();
                                }
                                else elsecount += 1;
                                if (elsecount == 16
                                    && shadB != null
                                    && shadB.CanBeCasted()
                                    && me.CanCast()
                                    && !e.IsMagicImmune()
                                    && me.HasModifier("modifier_crystal_maiden_freezing_field")
                                    && Menus.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_silver_edge")
                                    && me.Distance2D(e) <= 900
                                    )
                                    shadB.UseAbility();
                                //try
                                //{
                                if (Menus.Item("orbwalk").GetValue<bool>())
                                {
                                    var modifInv = me.IsInvisible();
                                    var range = modifInv
                                        ? me.Distance2D(e) <= me.GetAttackRange() / 100 * Menus.Item("range").GetValue<Slider>().Value
                                        : me.Distance2D(e) >= me.GetAttackRange() / 100 * Menus.Item("range").GetValue<Slider>().Value;

                                    if (!e.IsAttackImmune() && range)
                                    {
                                        Orbwalking.Orbwalk(e, 0, 1600, false, true);
                                    }
                                    else if (!e.IsAttackImmune() && me.Distance2D(e) <= me.GetAttackRange() / 100 * Menus.Item("range").GetValue<Slider>().Value && !e.IsAttackImmune() && Utils.SleepCheck("attack"))
                                    {
                                        Orbwalking.Orbwalk(e, 0, 1600, false, false);
                                    }
                                }
                            }
                        }
                }
                catch (Exception ex)
                {
                    var st = new System.Diagnostics.StackTrace(ex, true);
                    var line = st.GetFrame(0).GetFileLineNumber();

                    Console.WriteLine("Combo exception at line: " + line);
                }
            }
        }
        public static void linken(Hero z)
        {
            W = me.Spellbook.SpellW;

            cyclone = me.FindItem("item_cyclone");
            force = me.FindItem("item_force_staff");
            atos = me.FindItem("item_rod_of_atos");
            sheep = me.FindItem("item_sheepstick");

            dagon = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));

            if ((cyclone.CanBeCasted() || force.CanBeCasted() 
                || sheep.CanBeCasted() || atos.CanBeCasted() || W.CanBeCasted()) 
                && me.Distance2D(z)<=900 && Utils.SleepCheck("Combo2"))
            {
                Utils.Sleep(200, "Combo2");
                if (cyclone != null && cyclone.CanBeCasted()
                    && Menus.Item("Link").GetValue<AbilityToggler>().IsEnabled(cyclone.Name))
                    cyclone.UseAbility(z);
                else if (force != null && force.CanBeCasted() &&
                         Menus.Item("Link").GetValue<AbilityToggler>().IsEnabled(force.Name))
                    force.UseAbility(z);
                else if(W != null && W.CanBeCasted() &&
                   Menus.Item("Link").GetValue<AbilityToggler>().IsEnabled(W.Name)
                   && !me.IsMagicImmune())
                    W.UseAbility(z);
                else if(atos != null && atos.CanBeCasted()
                    && Menus.Item("Link").GetValue<AbilityToggler>().IsEnabled(atos.Name))
                    atos.UseAbility(z);
                else if (sheep != null && sheep.CanBeCasted() 
                    && Menus.Item("Link").GetValue<AbilityToggler>().IsEnabled(sheep.Name))
                    sheep.UseAbility(z);
            }
        }
        private static void AutoAbilities()
        {
            try
            {
                W = me.Spellbook.SpellW;
                R = me.Spellbook.SpellR;
                // Item
                cyclone = me.FindItem("item_cyclone");
                force = me.FindItem("item_force_staff");
                glimmer = me.FindItem("item_glimmer_cape");
                bkb = me.FindItem("item_black_king_bar");
                ghost = me.FindItem("item_ghost");
                atos = me.FindItem("item_rod_of_atos");
                shadB = me.FindItem("item_silver_edge") ?? me.FindItem("item_invis_sword");


                dagon = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));
                var v = ObjectManager.GetEntities<Hero>()
                             .Where(x => x.IsVisible && x.IsAlive && x.Team != me.Team && !x.IsIllusion).ToList();
                
                if (shadB != null
                   && shadB.CanBeCasted()
                   && me.CanCast()
                   && !e.IsMagicImmune()
                   && me.HasModifier("modifier_crystal_maiden_freezing_field")
                   && Menus.Item("AutoAbility").GetValue<AbilityToggler>().IsEnabled("item_silver_edge")
                   && me.Distance2D(e) <= 900 
                   && Utils.SleepCheck("combosleep")
                   )
                {
                    shadB.UseAbility();
                    Utils.Sleep(150, "combosleep");
                }
                if (glimmer != null
                   && glimmer.CanBeCasted()
                   && me.Distance2D(e) <= 900
                   && Menus.Item("AutoAbility").GetValue<AbilityToggler>().IsEnabled(glimmer.Name)
                   && me.HasModifier("modifier_crystal_maiden_freezing_field")
                   && Utils.SleepCheck("combosleep")
                   )
                {
                    glimmer.UseAbility(me);
                    Utils.Sleep(150, "combosleep");
                }
                if (    Game.Ping <= 200
                        && R != null
                        && R.CanBeCasted()
                        && R.IsInAbilityPhase
                        && me.Distance2D(e) <= R.GetCastRange()
                        && Utils.SleepCheck("combosleep")
                       )
                    {
                        Utils.Sleep(150, "combosleep");
                        if (bkb != null
                            && bkb.CanBeCasted()
                            && v.Count(x => x.Distance2D(me) <= 650) >=
                            Menus.Item("Heel").GetValue<Slider>().Value
                            && Menus.Item("AutoAbility").GetValue<AbilityToggler>().IsEnabled(bkb.Name)
                        )
                        {
                            me.Stop();
                            bkb.UseAbility();
                            R.UseAbility();
                        }
                        if (ghost != null
                            && ghost.CanBeCasted()
                            && !me.IsMagicImmune()
                            && Menus.Item("AutoAbility").GetValue<AbilityToggler>().IsEnabled(ghost.Name)
                            && (bkb == null
                            || !bkb.CanBeCasted()
                            || v.Count(x => x.Distance2D(me) <= 650) <= Menus.Item("Heel").GetValue<Slider>().Value
                            || !Menus.Item("AutoAbility").GetValue<AbilityToggler>().IsEnabled(bkb.Name))
                            )
                        {
                            me.Stop();
                            ghost.UseAbility();
                            R.UseAbility();
                        }
                    }
                if (Active) return;
                if (R.IsInAbilityPhase || R.IsChanneling || me.IsChanneling())
                        return;

                var ecount = v.Count;
                if (ecount <= 0)
                {
                    foreach (var z in v)
                    {
                        var stayHere = z.HasModifiers(new[]
                                {
                            "modifier_rod_of_atos_debuff",
                            "modifier_crystal_maiden_frostbite"
                            }, false);
                        if (me.HasModifier("modifier_spirit_breaker_charge_of_darkness_vision"))
                        {
                            if (z.ClassId == ClassId.CDOTA_Unit_Hero_SpiritBreaker)
                            {
                               
                                if (W != null && W.CanBeCasted() && me.Distance2D(z) <= W.GetCastRange() + me.HullRadius
                                       && !z.IsMagicImmune()
                                       && !stayHere
                                       && Menus.Item("AutoAbility").GetValue<AbilityToggler>().IsEnabled(W.Name)
                                   )
                                {
                                    W.UseAbility(z);
                                }
                                else if (atos != null
                                        && atos.CanBeCasted()
                                        && me.Distance2D(z) <= W.GetCastRange() + me.HullRadius
                                        && !z.IsMagicImmune()
                                        && Menus.Item("AutoAbility").GetValue<AbilityToggler>().IsEnabled(atos.Name)
                                        )
                                {
                                    atos.UseAbility(z);
                                }
                            }
                        }
                        
                        if (W != null && W.CanBeCasted() && me.Distance2D(z) <= 500
                                && z.Distance2D(me) <= me.HullRadius + 50
                                && !stayHere
                                && z.NetworkActivity == NetworkActivity.Attack
                                && Menus.Item("AutoAbility").GetValue<AbilityToggler>().IsEnabled(W.Name)
                                && !z.IsMagicImmune()
                            )
                        {
                            W.UseAbility(e);
                        }
                        if (W != null && W.CanBeCasted() && me.Distance2D(z) <= W.GetCastRange() + me.HullRadius + 24
                            && !z.IsLinkensProtected()
                            &&
                            !z.HasModifiers(new[]
                            {
                            "modifier_rod_of_atos_debuff",
                            "modifier_shadow_shaman_shackles",
                            "modifier_winter_wyvern_cold_embrace",
                            "modifier_storm_spirit_electric_vortex_pull",
                            "modifier_rubick_telekinesis",
                            "modifier_bane_fiends_grip",
                            "modifier_axe_berserkers_call",
                            "modifier_crystal_maiden_crystal_nova",
                            "modifier_dark_troll_warlord_ensnare",
                            "modifier_ember_spirit_searing_chains",
                            "modifier_enigma_black_hole_pull",
                            "modifier_ice_blast",
                            "modifier_kunkka_torrent",
                            "modifier_legion_commander_duel",
                            "modifier_lone_druid_spirit_bear_entangle_effect",
                            "modifier_naga_siren_ensnare",
                            "modifier_pudge_dismember",
                            "modifier_meepo_earthbind"
                            }, false)
                            && (z.FindSpell("magnataur_reverse_polarity").IsInAbilityPhase
                            || z.FindItem("item_blink")?.Cooldown > 11
                            || z.FindSpell("queenofpain_blink").IsInAbilityPhase
                            || z.FindSpell("antimage_blink").IsInAbilityPhase
                            || z.FindSpell("antimage_mana_void").IsInAbilityPhase
                            || z.FindSpell("legion_commander_duel")?.Cooldown <= 0
                            || z.FindSpell("doom_bringer_doom").IsInAbilityPhase
                            || z.HasModifier("modifier_faceless_void_chronosphere_freeze")
                            || z.FindSpell("witch_doctor_death_ward").IsInAbilityPhase
                            || z.FindSpell("rattletrap_power_cogs").IsInAbilityPhase
                            || z.FindSpell("tidehunter_ravage").IsInAbilityPhase
                            || z.FindSpell("axe_berserkers_call").IsInAbilityPhase
                            || z.FindSpell("brewmaster_primal_split").IsInAbilityPhase
                            || z.FindSpell("omniknight_guardian_angel").IsInAbilityPhase
                            || z.FindSpell("queenofpain_sonic_wave").IsInAbilityPhase
                            || z.FindSpell("sandking_epicenter").IsInAbilityPhase
                            || z.FindSpell("slardar_slithereen_crush").IsInAbilityPhase
                            || z.FindSpell("tiny_toss").IsInAbilityPhase
                            || z.FindSpell("tusk_walrus_punch").IsInAbilityPhase
                            || z.FindSpell("faceless_void_time_walk").IsInAbilityPhase
                            || z.FindSpell("faceless_void_chronosphere").IsInAbilityPhase
                            || z.FindSpell("disruptor_static_storm")?.Cooldown <= 0
                            || z.FindSpell("lion_finger_of_death")?.Cooldown <= 0
                            || z.FindSpell("luna_eclipse")?.Cooldown <= 0
                            || z.FindSpell("lina_laguna_blade")?.Cooldown <= 0
                            || z.FindSpell("doom_bringer_doom")?.Cooldown <= 0
                            || z.FindSpell("life_stealer_rage")?.Cooldown <= 0
                            && me.Level >= 7
                            )
                            && (z.FindItem("item_manta")?.Cooldown > 0
                                || z.FindItem("item_manta") == null || z.IsStunned() || z.IsHexed() || z.IsRooted())
                            && !z.IsMagicImmune()
                            && Menus.Item("AutoAbility").GetValue<AbilityToggler>().IsEnabled(W.Name)
                            && !z.HasModifier("modifier_medusa_stone_gaze_stone")
                        )
                        {
                            W.UseAbility(z);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var st = new System.Diagnostics.StackTrace(ex, true);
                var line = st.GetFrame(0).GetFileLineNumber();
                Console.WriteLine("AutoAbilities exception at line: " + line);
            }
        }


        private static bool IsDisembodied(Unit target)
        {
            string[] modifs =
            {
                "modifier_item_ethereal_blade_ethereal",
                "modifier_pugna_decrepify"
            };

            return target.HasModifiers(modifs);
        }

        private static bool CanIncreaseMagicDmg(Hero source, Hero target)
        {
            //var orchid = source.FindItem("item_orchid") ?? source.FindItem("item_bloodthorn");
            var veil = source.FindItem("item_veil_of_discord");
            ethereal = source.FindItem("item_ethereal_blade");

            return (//(orchid != null && orchid.CanBeCasted() && !target.HasModifier("modifier_orchid_malevolence_debuff"))||
                  (veil != null && veil.CanBeCasted() && !target.HasModifier("modifier_item_veil_of_discord_debuff"))
                 || (ethereal != null && ethereal.CanBeCasted() && !IsDisembodied(target))
                 )
                 && source.CanUseItems();
        }

        private static void AutoSpells()
        {
            enemies = ObjectManager.GetEntities<Hero>()
                 .Where(x => x.IsVisible && x.IsAlive && x.Team != me.Team && !x.IsMagicImmune() && !x.IsMagicImmune() && !x.IsIllusion && !x.IsMagicImmune()).ToList();

            e = TargetSelector.ClosestToMouse(me,8000);

            if (e == null || !e.IsValid || !e.IsAlive) return;
            Q = me.Spellbook.SpellQ;
            W = me.Spellbook.SpellW;
            // Item
            ethereal = me.FindItem("item_ethereal_blade");
            vail = me.FindItem("item_veil_of_discord");
            orchid = me.FindItem("item_orchid") ?? me.FindItem("item_bloodthorn");
            atos = me.FindItem("item_rod_of_atos");
            blink = me.FindItem("item_blink");
            shiva = me.FindItem("item_shivas_guard");
            dagon = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));
            foreach (var v in enemies)
                {

                    if (me.IsInvisible()) return;
                    if (v.IsMagicImmune()) return;
                if (R.IsInAbilityPhase || R.IsChanneling || me.IsChanneling())
                    return;
                if (v.IsLinkensProtected() && Menus.Item("autoLink").IsActive())
                {
                    linken(v);
                }
                damage[v.Handle] = CalculateDamage(v);

                    var Range = me.HullRadius + Q?.GetCastRange()+200;
                var stayHere = v.HasModifiers(new[]
                            {
                            "modifier_rod_of_atos_debuff",
                            "modifier_crystal_maiden_frostbite"
                            }, false);
                float angle = me.FindAngleBetween(v.Position, true);
                    Vector3 pos = new Vector3((float)(v.Position.X - 200 * Math.Cos(angle)), (float)(v.Position.Y - 200 * Math.Sin(angle)), 0);
                    Vector3 posBlink = new Vector3((float)(v.Position.X - Range * Math.Cos(angle)), (float)(v.Position.Y - Range * Math.Sin(angle)), 0);
                if (Utils.SleepCheck("steal"))
                {
                    Utils.Sleep(250, "steal");
                    uint elsecount = 0;
                    elsecount += 1;
                    if (elsecount == 1
                           && enemies.Count(
                            x => x.Distance2D(v) <= 500) <= Menus.Item("solo_kill").GetValue<Slider>().Value
                           && blink != null
                           && blink.CanBeCasted()
                           && me.CanCast()
                           && me.Health >= (me.MaximumHealth / 100 * Menus.Item("minHealth").GetValue<Slider>().Value)
                           && v.Health <= damage[v.Handle]
                           && me.Distance2D(pos) <= 1180
                           && Menus.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(blink.Name)
                           && me.Distance2D(v) > 500
                           )
                        blink.UseAbility(pos);
                    if (v.Health <= damage[v.Handle] && me.Distance2D(v) <= Range)
                    {
                        elsecount += 1;
                        if (elsecount == 2
                            && orchid != null
                            && orchid.CanBeCasted()
                            && me.CanCast()
                            && !e.IsLinkensProtected()
                            && !e.IsMagicImmune()
                            && Menus.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled("item_bloodthorn")
                            )
                            orchid.UseAbility(e);
                        elsecount += 1;
                        if (elsecount == 3
                            && atos != null
                            && !stayHere
                            && atos.CanBeCasted()
                            && me.CanCast()
                            && !e.IsLinkensProtected()
                            && !e.IsMagicImmune()
                            && Menus.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(atos.Name)
                            && me.Distance2D(e) <= Menus.Item("atosRange").GetValue<Slider>().Value
                            )
                            atos.UseAbility(e);
                        else elsecount += 1;
                        if (elsecount == 4 
                            && vail != null
                            && vail.CanBeCasted()
                            && me.CanCast()
                            && Menus.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(vail.Name)
                            )
                            vail.UseAbility(v.Position);
                        else elsecount += 1;
                        if (elsecount == 5
                            && ethereal != null
                            && ethereal.CanBeCasted()
                            && me.CanCast()
                            && Menus.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(ethereal.Name)
                            )
                            ethereal.UseAbility(v);
                        else elsecount += 1;
                        if (elsecount == 6
                            && Q != null
                            && Q.CanBeCasted() 
                            && me.Distance2D(v) <= Q.GetCastRange() + me.HullRadius
                            && Menus.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(Q.Name))
                            Q.UseAbility(v.Position);
                        else elsecount += 1;
                        if (elsecount == 7
                            && Q != null
                            && Q.CanBeCasted()
                            && Menus.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(Q.Name))
                            Q.UseAbility(pos);
                        else elsecount += 1;
                        if (elsecount == 8
                            && W != null
                            && W.CanBeCasted()
                            && me.Distance2D(v) <= W.GetCastRange() + me.HullRadius
                            && Menus.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(W.Name))
                            W.UseAbility(v);
                        else elsecount += 1;
                        if (elsecount == 9
                           && dagon != null
                           && dagon.CanBeCasted()
                           && Menus.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled("item_dagon"))
                                dagon.UseAbility(v);
                        if (elsecount == 10
                            && shiva != null
                            && shiva.CanBeCasted()
                            && me.Distance2D(v) <= 600 + me.HullRadius
                            && Menus.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(shiva.Name))
                                shiva.UseAbility();
                            if (W != null && W.CanBeCasted() && me.Distance2D(v) >= W.GetCastRange() + me.HullRadius && me.Distance2D(v) <= W.GetCastRange() + me.HullRadius + 325 && Menus.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(W.Name) && Utils.SleepCheck("Move"))
                            {
                                me.Move(v.Position);
                                Utils.Sleep(250, "Move");
                            }
                    }
                }
            } // foreach::END
        } // AutoSpells::END
        private static void DrawUltiDamage(EventArgs args)
        {
            DrawingOnCore();
            //var Drawing = Ensage.Drawing;
            enemies = ObjectManager.GetEntities<Hero>()
                 .Where(x => x.IsVisible && x.IsAlive && x.Team != me.Team && !x.IsMagicImmune() && !x.IsIllusion).ToList();
            if (!Game.IsInGame || Game.IsPaused || Game.IsWatchingGame || enemies.Count == 0) return;

            if (Menus.Item("dmg").IsActive())
            {
                foreach (var v in enemies)
                {

                    damage[v.Handle] = CalculateDamage(v);
                    var screenPos = HUDInfo.GetHPbarPosition(v);
                    if (!OnScreen(v.Position)) continue;
                    var text = v.Health <= damage[v.Handle] ? "Yes: " + Math.Floor(damage[v.Handle]) : "No: " + Math.Floor(damage[v.Handle]);
                    var size = new Vector2(18, 18);
                    var textSize = Drawing.MeasureText(text, "Arial", size, FontFlags.AntiAlias);
                    var position = new Vector2(screenPos.X - textSize.X + 85, screenPos.Y + 62);

                    Drawing.DrawText(
                        text,
                        new Vector2(screenPos.X - textSize.X + 84, screenPos.Y + 63),
                        size,
                        (Color.White),
                        FontFlags.AntiAlias);
                    Drawing.DrawText(
                        text,
                        position,
                        size,
                        (v.Health <= damage[v.Handle] ? Color.LawnGreen : Color.Red),
                        FontFlags.AntiAlias);
                }
            }
        } // DrawUltiDamage::END
        private static double CalculateDamage(Hero victim)
        {
            double dmgResult = 0;

            var WPoint = me.FindSpell("special_bonus_unique_crystal_maiden_2");
            
            qDmg = WPoint != null && WPoint.Level > 0? Q.GetAbilityData("nova_damage")+250: Q.GetAbilityData("nova_damage");
            wDmg = W.GetAbilityData("hero_damage_tooltip");
            if (Q != null && Menus.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(Q.Name) && Q.CanBeCasted())
                dmgResult += qDmg;

            if (W != null && W.CanBeCasted() && Menus.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(W.Name))
                dmgResult += wDmg;

            if (victim.NetworkName == "CDOTA_Unit_Hero_SkeletonKing" && victim.Spellbook.SpellR.Cooldown <=0 && victim.Mana>140)
                dmgResult = 0;

            if (victim.HasModifier("modifier_kunkka_ghost_ship_damage_absorb"))
                dmgResult *= 0.5;

            if (victim.HasModifier("modifier_bloodseeker_bloodrage"))
            {
                var blood = ObjectManager.GetEntities<Hero>()
                    .FirstOrDefault(x => x.ClassId == ClassId.CDOTA_Unit_Hero_Bloodseeker);
                if (blood != null)
                    dmgResult *= bloodrage[blood.Spellbook.Spell1.Level];
                else
                    dmgResult *= 1.4;
            }

            if (victim.HasModifier("modifier_chen_penitence"))
            {
                var chen =
                    ObjectManager.GetEntities<Hero>()
                        .FirstOrDefault(x => x.Team == me.Team && x.ClassId == ClassId.CDOTA_Unit_Hero_Chen);
                if (chen != null)
                    dmgResult *= penitence[chen.Spellbook.Spell1.Level];
            }

            if (victim.HasModifier("modifier_shadow_demon_soul_catcher"))
            {
                var demon = ObjectManager.GetEntities<Hero>()
                    .FirstOrDefault(x => x.Team == me.Team && x.ClassId == ClassId.CDOTA_Unit_Hero_Shadow_Demon);
                if (demon != null)
                    dmgResult *= souls[demon.Spellbook.Spell2.Level];
            }

            if (victim.HasModifier("modifier_item_mask_of_madness_berserk"))
                dmgResult *= 1.3;

            vail = me.FindItem("item_veil_of_discord");
            if ((vail != null && vail.CanBeCasted() && Menus.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(vail.Name) 
                || victim.HasModifier("modifier_item_veil_of_discord_debuff"))
                )
            {
                dmgResult *= 1.25;
            }
            
            var spellamplymult = 1 + (me.TotalIntelligence / 16 / 100);
            dmgResult = dmgResult * spellamplymult;
            if (dagon != null && dagon.CanBeCasted() && victim.Handle == e?.Handle && Menus.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled("item_dagon"))
                dmgResult += dagonDmg[dagon.Level];
            shiva = me.FindItem("item_shivas_guard");
            if (shiva != null && shiva.CanBeCasted() && Menus.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(shiva.Name))
                dmgResult += 200;
            dmgResult *= 1 - victim.MagicDamageResist;
            int etherealdamage = (int)((me.TotalIntelligence * 2) + 75);

            if (ethereal != null && ethereal.CanBeCasted() && victim.Handle == e?.Handle 
                && Menus.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))
                dmgResult = dmgResult * 1.4 + etherealdamage;

            
            if (orchid != null && orchid.CanBeCasted() && Menus.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled("item_bloodthorn")|| victim.HasModifier("modifier_orchid_malevolence_debuff"))
                dmgResult *= 1.3;
            return dmgResult;
        } // GetDamageTaken::END
        public static bool OnScreen(Vector3 v)
        {
            return !(Drawing.WorldToScreen(v).X < 0 || Drawing.WorldToScreen(v).X > Drawing.Width
                  || Drawing.WorldToScreen(v).Y < 0 || Drawing.WorldToScreen(v).Y > Drawing.Height);
        }

        private static Hero GetClosestToTarget(List<Hero> units, Hero z)
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