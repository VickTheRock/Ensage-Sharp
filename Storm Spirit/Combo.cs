namespace StormSpirit
{
    using System;
    using System.Linq;
    using Ensage;
    using SharpDX;
    using Ensage.Common.Extensions;
    using Ensage.Common;
    using Ensage.Common.Menu;

    partial class StormSpirit
    {
        private static void Main()
        {
            Events.OnLoad += (sender, args) =>
            {
                if (ObjectManager.LocalHero.ClassId != ClassId.CDOTA_Unit_Hero_StormSpirit) return;
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
                Menu.RemoveFromMainMenu();
                Game.OnUpdate -= Action;
            };
        }



        private static void Action(EventArgs args)
        {
            Q = me.Spellbook.SpellQ;
            W = me.Spellbook.SpellW;
            E = me.Spellbook.SpellE;
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
            travel = me.FindItem("item_travel_boots") ?? me.FindItem("item_travel_boots_2") ?? me.FindItem("item_tpscroll");
            e = TargetSelector.ClosestToMouse(me, 5000);
            enemies = ObjectManager.GetEntities<Hero>()
                .Where(x => x.IsAlive && x.Team != me.Team && !x.IsIllusion && x.Distance2D(me) <= 4000
                            && (!x.HasModifier("modifier_abaddon_borrowed_time")
                                || !x.HasModifier("modifier_item_blade_mail_reflect")))
                .ToList();
            if (e == null) return;
            if (e.HasModifier("modifier_abaddon_borrowed_time")
                || e.HasModifier("modifier_item_blade_mail_reflect"))
            {

                e = GetClosestToTarget(enemies, e) ?? null;
                if (Utils.SleepCheck("spam"))
                {
                    Utils.Sleep(5000, "spam");
                }
            }
            if (e == null && Effect != null)
            {
                Effect.Dispose();
                Effect = null;
            }
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
            Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key);
            var debuff =  (e.HasModifiers(new[]
            {
                "modifier_storm_spirit_electric_vortex_pull",
                "modifier_sheepstick_debuff",
                "modifier_rod_of_atos_debuff"
            }, false) || e.IsStunned() || e.IsHexed() || e.IsRooted());
            if (me.IsAlive)
            {
                AutoAbilities();
            }
            /*if (Menu.Item("AutoUsage").IsActive())
            {
                // AutoSpells();
            }*/
            var inUltBall = me.HasModifier("modifier_storm_spirit_ball_lightning");
            var inOverload = me.HasModifier("modifier_storm_spirit_overload");
            var lotusBuff = e.HasModifier("modifier_item_lotus_orb_active");
            var inVortex = e.HasModifier("modifier_storm_spirit_electric_vortex_pull");

            var startManaCost = R.GetAbilityData("ball_lightning_initial_mana_base") +
                                me.MaximumMana / 100 * R.GetAbilityData("ball_lightning_initial_mana_percentage");
            var costPerUnit = (12 + me.MaximumMana * 0.007) / 100.0;
            var distance = me.Distance2D(e);
            var rManacost = startManaCost + costPerUnit * Math.Floor(distance / 100) * 100;
            if (Active && Utils.SleepCheck("combosleep") && me.IsAlive)
            {
                Utils.Sleep(250, "combosleep");
                /*try
                {*/
                //spell
                sheep = e.ClassId == ClassId.CDOTA_Unit_Hero_Tidehunter ? null : me.FindItem("item_sheepstick");

                //var noBlade = e.HasModifier("modifier_item_blade_mail_reflect");

                if (!lotusBuff)
                {
                    if (e.IsLinkensProtected())
                        linken(e);
                    if (
                        // cheese
                        cheese != null
                        && cheese.CanBeCasted()
                        && (me.Health <= me.MaximumHealth * 0.3
                            || me.Mana <= me.MaximumMana * 0.2)
                        && distance <= 700
                        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(cheese.Name)
                    )
                        cheese.UseAbility();
                    if ( // SoulRing Item 
                        soul != null
                        && soul.CanBeCasted()
                        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(soul.Name)
                        && me.CanCast()
                        && me.Health >= me.MaximumHealth * 0.4
                        && me.Mana <= me.MaximumMana * 0.2
                    )
                        soul.UseAbility();
                    if ( // Arcane Boots Item
                        arcane != null
                        && arcane.CanBeCasted()
                        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(arcane.Name)
                        && me.CanCast()
                        && me.Mana <= me.MaximumMana * 0.4
                    )
                        arcane.UseAbility();
                   
                    uint elsecount = 0;
                    elsecount += 1;
                    if (elsecount == 1
                        && R != null
                        && R.CanBeCasted()
                        && !R.IsChanneling
                        && !R.IsInAbilityPhase
                        && me.Mana >= rManacost
                        && !inUltBall
                        && distance >= me.GetAttackRange()
                        && !e.IsMagicImmune()
                        && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
                    )
                    {
                        if (e.NetworkActivity == NetworkActivity.Move)
                            R.CastSkillShot(e);
                        else
                            R.UseAbility(e.Position);
                    }
                    else elsecount += 1;
                    if (vail != null
                        && vail.CanBeCasted()
                        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(vail.Name)
                        && me.CanCast()
                        && !e.IsMagicImmune()
                    )
                        vail.UseAbility(e.Position);
                    if (orchid != null
                        && orchid.CanBeCasted()
                        && me.CanCast()
                        && !e.IsLinkensProtected()
                        && !e.IsMagicImmune()
                        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_bloodthorn")
                    )
                        orchid.UseAbility(e);
                    if (elsecount == 2
                        && me.CanAttack()
                        && inOverload
                        && distance <= me.GetAttackRange()
                        && !e.IsAttackImmune()
                    )
                    {
                        me.Attack(e);
                    }
                    else elsecount += 1;
                    if (elsecount == 3
                        && dagon != null
                        && dagon.CanBeCasted()
                        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon")
                        && me.CanCast()
                        && (ethereal == null
                            || e.HasModifier("modifier_item_ethereal_blade_slow")
                            || ethereal.Cooldown < 17)
                        && !e.IsLinkensProtected()
                        && !e.IsMagicImmune()
                    )
                        dagon.UseAbility(e);
                    else elsecount += 1;
                    if (elsecount == 4
                        && sheep != null
                        && sheep.CanBeCasted()
                        && me.CanCast()
                        && !debuff
                        && !e.IsLinkensProtected()
                        && !e.IsMagicImmune()
                        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(sheep.Name)
                    )
                        sheep.UseAbility(e);
                    else elsecount += 1;
                    if (elsecount == 5
                        && urn != null && urn.CanBeCasted() && urn.CurrentCharges > 0
                        && distance <= urn.GetCastRange()
                        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(urn.Name)
                    )
                        urn.UseAbility(e);
                    else elsecount += 1;
                    if (elsecount == 6
                        && bkb != null
                        && bkb.CanBeCasted()
                        && enemies.Count(x => x.Distance2D(me) <= 700) >=
                        Menu.Item("Heel").GetValue<Slider>().Value
                        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(bkb.Name)
                    )
                        bkb.UseAbility();
                    else elsecount += 1;
                    if (elsecount == 7
                        && lotus != null
                        && lotus.CanBeCasted()
                        && enemies.Count(x => x.Distance2D(me) <= 700) >= 1
                        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(lotus.Name)
                    )
                        lotus.UseAbility(me);
                    else elsecount += 1;
                    if (elsecount == 8
                        && ethereal != null
                        && ethereal.CanBeCasted()
                        && me.CanCast()
                        && !e.IsLinkensProtected()
                        && !e.IsMagicImmune()
                        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name)
                    )
                    {
                        if (Menu.Item("debuff").GetValue<bool>())
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
                    if (elsecount == 9
                        && W != null
                        && W.CanBeCasted()
                        && !inOverload
                        && !debuff
                        && !me.IsAttacking()
                        && me.CanCast()
                        && !e.IsMagicImmune()
                        && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
                    )
                        W.UseAbility(e);
                    else elsecount += 1;
                    if (elsecount == 10
                        && Q != null
                        && Q.CanBeCasted()
                        && !inOverload
                        && distance <= Q.GetAbilityData("static_remnant_radius") + me.HullRadius
                        && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
                        && me.CanCast()
                        && !e.IsMagicImmune()
                    )
                        Q.UseAbility();
                    else elsecount += 1;
                    if (elsecount == 11
                        && atos != null
                        && atos.CanBeCasted()
                        && me.CanCast()
                        && !debuff
                        && !e.IsLinkensProtected()
                        && !e.IsMagicImmune()
                        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(atos.Name)
                    )
                        atos.UseAbility(e);
                    else elsecount += 1;
                    if (elsecount == 12
                        && shiva != null
                        && shiva.CanBeCasted()
                        && me.CanCast()
                        && !e.IsMagicImmune()
                        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(shiva.Name)
                        && distance <= 600
                    )
                        shiva.UseAbility();
                    else elsecount += 1;
                    if (elsecount == 13
                        && R != null
                        && R.CanBeCasted()
                        && !inUltBall
                        && !R.IsInAbilityPhase
                        && !R.IsChanneling
                        && !me.IsChanneling()
                        && (Menu.Item("AutoOverload").GetValue<bool>() && inVortex || !inVortex)
                        && !inOverload
                        && distance <= me.AttackRange
                        && !e.IsMagicImmune()
                        && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
                    )
                        R.UseAbility(me.Position);
                }
                if (Menu.Item("orbwalk").GetValue<bool>())
                {
                    var modifInv = me.IsInvisible();
                    var range = modifInv
                        ? distance <= me.GetAttackRange() / 100 * Menu.Item("range").GetValue<Slider>().Value
                        : distance >= me.GetAttackRange() / 100 * Menu.Item("range").GetValue<Slider>().Value;

                    if (!e.IsAttackImmune() && range)
                    {
                        Orbwalking.Orbwalk(e, 10, 1600, false, true);
                    }
                    else if (!e.IsAttackImmune() && distance <= me.GetAttackRange() / 100 * Menu.Item("range").GetValue<Slider>().Value && !e.IsAttackImmune())
                    {
                        Orbwalking.Orbwalk(e, 10, 1600, false, false);
                    }
                }
                /*}
                catch (Exception ex)
                {
                    var st = new System.Diagnostics.StackTrace(ex, true);
                    var line = st.GetFrame(0).GetFileLineNumber();

                    Console.WriteLine("Combo exception at line: " + line);
                }*/
            }
        }
        private static void linken(Hero z)
        {
            W = me.Spellbook.SpellW;

            cyclone = me.FindItem("item_cyclone");
            force = me.FindItem("item_force_staff");
            atos = me.FindItem("item_rod_of_atos");
            sheep = me.FindItem("item_sheepstick");

            dagon = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));

            if ((cyclone.CanBeCasted() || force.CanBeCasted()
                 || sheep.CanBeCasted() || atos.CanBeCasted() || W.CanBeCasted())
                && me.Distance2D(z) <= 900 && Utils.SleepCheck("Combo2"))
            {
                Utils.Sleep(200, "Combo2");
                if (cyclone != null && cyclone.CanBeCasted()
                    && Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(cyclone.Name))
                    cyclone.UseAbility(z);
                else if (force != null && force.CanBeCasted() &&
                         Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(force.Name))
                    force.UseAbility(z);
                else if (W != null && W.CanBeCasted() &&
                         Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(W.Name)
                         && !me.IsMagicImmune())
                    W.UseAbility(z);
                else if (atos != null && atos.CanBeCasted()
                         && Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(atos.Name))
                    atos.UseAbility(z);
                else if (sheep != null && sheep.CanBeCasted()
                         && Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(sheep.Name))
                    sheep.UseAbility(z);
            }
        }

        private static void AutoAbilities()
        {

            R = me.Spellbook.SpellR;
            Unit f = ObjectManager.GetEntities<Unit>().First(x => x.Team == me.Team && x.ClassId == ClassId.CDOTA_Unit_Fountain);
            float angle = f.FindAngleBetween(me.Position, true);
            Vector3 pos = new Vector3((float)(me.Position.X - 1500 * Math.Cos(angle)),
                (float)(me.Position.Y - 1500 * Math.Sin(angle)), 0);
            if (me.Health <= me.MaximumHealth / 100 * Menu.Item("RHealh").GetValue<Slider>().Value && Utils.SleepCheck("escape*"))
            {
                var distance = me.Distance2D(e);
                Utils.Sleep(350, "escape*");
                if (me.IsAlive
                    && distance <= 800
                    && R != null
					&& !R.IsInAbilityPhase
                    && !R.IsChanneling
                    && R.CanBeCasted()
					&& Menu.Item("BallLightningAbility").GetValue<AbilityToggler>().IsEnabled(R.Name)
                    && e.Health > me.Health
                )
				{
                    R.UseAbility(pos);
					if (
                    travel != null
                    && travel.CanBeCasted()
                    && Menu.Item("BallLightningAbility").GetValue<AbilityToggler>().IsEnabled("item_travel_boots_2")
                )
                    travel.UseAbility(f.Position);
				}
                
            }
        }


    }
}