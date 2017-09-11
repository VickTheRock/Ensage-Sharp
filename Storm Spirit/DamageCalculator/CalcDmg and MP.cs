namespace StormSpirit
{
    using System.Collections.Generic;
    using System;
    using System.Linq;
    using Ensage.Common.Extensions;
    using Ensage;
    using Ensage.SDK.Extensions;
    using ExUnit = Ensage.SDK.Extensions.UnitExtensions;

    partial class Combo
    {
        public static float qDmg, wDmg, eDmg, rDmg, sheepDmg;
        public static double[] penitence = { 0, 1.15, 1.2, 1.25, 1.3 };
        public static double[] bloodrage = { 0, 1.15, 1.2, 1.25, 1.3 };
        public static double[] souls = { 0, 1.2, 1.3, 1.4, 1.5 };
        public static readonly int[] dagonDmg = { 0, 400, 500, 600, 700, 800 };
        public static Dictionary<uint, float> damage = new Dictionary<uint, float>();
        public double CalculateDamage(Hero victim)
        {
            double dmgResult = 0;
            try
            {
                bool vailOn = false;
                bool orchidOn = false;
                double manacost = 0;
                var rLevel = R.Level;
                var distance = me.Distance2D(victim);

                var travelSpeed = R.GetAbilityData("ball_lightning_move_speed", rLevel);
                var travelTime = distance / travelSpeed;
                var startManaCost = R.GetAbilityData("ball_lightning_initial_mana_base") +
                                    me.MaximumMana / 100 * R.GetAbilityData("ball_lightning_initial_mana_percentage");
                //Console.WriteLine("startManaCost " + startManaCost);

                var costPerUnit = (12 + me.MaximumMana * 0.007) / 100.0;

                var rManacost = startManaCost + costPerUnit * Math.Floor(distance / 100) * 100;

                var e = TargetSelector.Active.GetTargets()
                    .FirstOrDefault(x => !x.IsInvulnerable() && x.IsAlive);
                var damageR = R.GetAbilityData("#AbilityDamage", rLevel);// / 100f;
                rDmg = (distance - 1) * (float)0.01 * damageR;
                Q = me.Spellbook.SpellQ;
                W = me.Spellbook.SpellW;
                E = me.Spellbook.SpellE;
                R = me.Spellbook.SpellR;
                qDmg = Q.GetAbilityData("static_remnant_damage");

                wDmg = (me.AttacksPerSecond * W.GetAbilityData("duration") - 1) * me.GetAttackDamage(victim);
                sheepDmg = (me.AttacksPerSecond * (float)3.5 - 1) * me.GetAttackDamage(victim);
                eDmg = E.GetAbilityData("#AbilityDamage", E.Level);

                var qReady = Q != null && Config.AbilityToggler.Value.IsEnabled(Q.Name)
                             && Q.CanBeCasted();
                var wReady = W != null && Config.AbilityToggler.Value.IsEnabled(W.Name)
                             && W.CanBeCasted();
                var rReady = R != null && Config.AbilityToggler.Value.IsEnabled(R.Name)
                             && R.CanBeCasted();
                var eReady = E != null && Config.AbilityToggler.Value.IsEnabled(E.Name)
                             && E.Level > 0;
                if (arcane != null
                    && arcane.Item.IsValid && arcane.Item.CanBeCasted()
                    && Config.ItemToggler.Value.IsEnabled(arcane.Item.Name))
                {
                    manacost -= 125;
                }
                if (soul != null
                    && soul.Item.IsValid && soul.Item.CanBeCasted()
                    && Config.ItemToggler.Value.IsEnabled(soul.Item.Name))
                {
                    manacost -= 150;
                }
                if (rReady)
                {
                    if (manacost + rManacost < me.Mana)
                    {
                        manacost += rManacost;
                        dmgResult += rDmg * (1 - victim.MagicDamageResist);
                        if (eReady)
                            dmgResult += eDmg * (1 - victim.MagicDamageResist);
                    }
                    else goto gotoDamage;
                }
                if (orchid != null
                    && orchid.Item.IsValid && orchid.Item.CanBeCasted() && Config.ItemToggler.Value.IsEnabled("item_bloodthorn")
                    && !ExUnit.HasModifier(victim, "modifier_bloodthorn_debuff")
                    && !ExUnit.HasModifier(victim, "modifier_orchid_malevolence_debuff"))
                {
                    if (manacost + orchid.Item.ManaCost < me.Mana)
                    {
                        manacost += orchid.Item.ManaCost;
                        orchidOn = true;
                    }
                    else goto gotoDamage;
                }
                if (bloodthorn != null
                    && bloodthorn.Item.IsValid && bloodthorn.Item.CanBeCasted() && Config.ItemToggler.Value.IsEnabled("item_bloodthorn")
                    && !ExUnit.HasModifier(victim, "modifier_bloodthorn_debuff")
                    && !ExUnit.HasModifier(victim, "modifier_orchid_malevolence_debuff"))
                {
                    if (manacost + bloodthorn.Item.ManaCost < me.Mana)
                    {
                        manacost += bloodthorn.Item.ManaCost;
                        orchidOn = true;
                    }
                    else goto gotoDamage;
                }
                if (vail != null
                    && vail.Item.IsValid && vail.Item.CanBeCasted()
                    && Config.ItemToggler.Value.IsEnabled(vail.Item.Name)
                    && !ExUnit.HasModifier(victim, "modifier_item_veil_of_discord_debuff"))
                {
                    if (manacost + vail.Item.ManaCost < me.Mana)
                    {
                        manacost += vail.Item.ManaCost;
                        vailOn = true;
                    }
                    else goto gotoDamage;
                }
                if (qReady)
                {
                    if (manacost + Q.ManaCost < me.Mana)
                    {

                        manacost += Q.ManaCost;
                        dmgResult += qDmg * (1 - victim.MagicDamageResist);
                        if (eReady)
                            dmgResult += eDmg * (1 - victim.MagicDamageResist);
                    }
                    else goto gotoDamage;
                }

                if (wReady && eReady)
                {
                    if (manacost + W.ManaCost < me.Mana && !Config.fastVortex.Value)
                    {
                        dmgResult += eDmg * (1 - victim.MagicDamageResist);
                    }
                    else goto gotoDamage;
                }
                var spellamplymult = 1 + me.TotalIntelligence / 16 / 100;
                dmgResult = dmgResult * spellamplymult;
                var dagon = me.GetDagon();
                if (dagon != null
                    && dagon.IsValid && dagon.CanBeCasted() && victim.Handle == e?.Handle && Config.ItemToggler.Value.IsEnabled("item_dagon_5"))
                {
                    if (manacost + dagon.ManaCost < me.Mana)
                    {
                        manacost += dagon.ManaCost;
                        dmgResult += dagonDmg[dagon.Level] * (1 - victim.MagicDamageResist);
                    }
                    else goto gotoDamage;
                }
                if (shiva != null
                    && shiva.Item.IsValid && shiva.Item.CanBeCasted() && Config.ItemToggler.Value.IsEnabled(shiva.Item.Name))
                {
                    if (manacost + shiva.Item.ManaCost < me.Mana)
                    {
                        manacost += shiva.Item.ManaCost;
                        dmgResult += 200 * (1 - victim.MagicDamageResist);
                    }
                    else goto gotoDamage;
                }

                int etherealdamage = (int)(me.TotalIntelligence * 2 + 75);

                if (ethereal != null
                    && ethereal.Item.IsValid && ethereal.Item.CanBeCasted() && victim.Handle == e?.Handle)
                {
                    if (manacost + ethereal.Item.ManaCost < me.Mana)
                    {
                        manacost += ethereal.Item.ManaCost;
                        dmgResult *= 1.4 + etherealdamage;
                    }
                    //else goto gotoDamage;
                }
                gotoDamage:

                if (ExUnit.HasModifier(me, "modifier_special_bonus_spell_amplify"))
                {
                    dmgResult *= 1.10;
                }
                if (ExUnit.HasModifier(victim, "modifier_item_veil_of_discord_debuff"))
                {
                    dmgResult *= 1.25;
                }
                if (vail != null
                    && vail.Item.IsValid && vail.Item.CanBeCasted()
                    && Config.ItemToggler.Value.IsEnabled(vail.Item.Name)
                    && !ExUnit.HasModifier(victim, "modifier_item_veil_of_discord_debuff"))
                {
                    if (vailOn)
                    {
                        dmgResult *= 1.25;
                    }
                }
                
                if (sheep != null
                    && sheep.Item.IsValid && sheep.Item.CanBeCasted() && Config.ItemToggler.Value.IsEnabled(sheep.Item.Name))
                {
                    if (manacost + 100 < me.Mana)
                    {
                        dmgResult += sheepDmg;
                    }
                }
                if (bloodthorn != null
                    && bloodthorn.Item.IsValid && bloodthorn.Item.CanBeCasted()
                    && Config.ItemToggler.Value.IsEnabled("item_bloodthorn") && orchidOn || ExUnit.HasModifier(victim, "modifier_bloodthorn_debuff"))
                {
                    if (qReady && eReady)
                        dmgResult += me.GetAttackDamage(victim) * 1.4;
                    if (rReady && eReady)
                        dmgResult += me.GetAttackDamage(victim) * 1.4;
                    if (wReady)
                    {
                        if (manacost + W.ManaCost < me.Mana)
                        {
                            dmgResult += wDmg * 1.4;
                        }
                    }
                }
                else
                {
                    if (qReady && eReady)
                        dmgResult += me.GetAttackDamage(victim);
                    if (rReady && eReady)
                        dmgResult += me.GetAttackDamage(victim);
                    if (wReady)
                    {
                        if (manacost + W.ManaCost < me.Mana)
                        {
                            dmgResult += wDmg;
                        }
                    }
                }
                if (ExUnit.HasModifier(victim, "modifier_chen_penitence"))
                {
                    var chen =
                        ObjectManager.GetEntities<Hero>()
                            .FirstOrDefault(x => x.Team == me.Team && x.ClassId == ClassId.CDOTA_Unit_Hero_Chen);
                    if (chen != null)
                        dmgResult *= penitence[chen.Spellbook.Spell1.Level];
                }

                if (ExUnit.HasModifier(victim, "modifier_shadow_demon_soul_catcher"))
                {
                    var demon = ObjectManager.GetEntities<Hero>()
                        .FirstOrDefault(x => x.Team == me.Team && x.ClassId == ClassId.CDOTA_Unit_Hero_Shadow_Demon);
                    if (demon != null)
                        dmgResult *= souls[demon.Spellbook.Spell2.Level];
                }
                if (orchid != null
                   && orchid.Item.IsValid && orchid.Item.CanBeCasted() && Config.ItemToggler.Value.IsEnabled("item_bloodthorn")
                   && !ExUnit.HasModifier(victim, "modifier_bloodthorn_debuff")
                   && !ExUnit.HasModifier(victim, "modifier_orchid_malevolence_debuff"))
                {
                    if (orchidOn)
                    {
                        dmgResult *= 1.3;
                    }
                }
                if (bloodthorn != null
                    && bloodthorn.Item.IsValid && bloodthorn.Item.CanBeCasted() && Config.ItemToggler.Value.IsEnabled("item_bloodthorn")
                    && !ExUnit.HasModifier(victim, "modifier_bloodthorn_debuff")
                    && !ExUnit.HasModifier(victim, "modifier_orchid_malevolence_debuff"))
                {
                    if (orchidOn)
                    {
                        dmgResult *= 1.3;
                    }
                }
                if (ExUnit.HasModifier(victim, "modifier_item_mask_of_madness_berserk"))
                    dmgResult *= 1.3;
                if (ExUnit.HasModifier(victim, "modifier_orchid_malevolence_debuff") || ExUnit.HasModifier(victim, "modifier_bloodthorn_debuff"))
                {
                    dmgResult *= 1.3;
                }
                if (ExUnit.HasModifier(victim, "modifier_kunkka_ghost_ship_damage_absorb"))
                    dmgResult *= 0.5;

                if (ExUnit.HasModifier(victim, "modifier_bloodseeker_bloodrage") || ExUnit.HasModifier(me, "modifier_bloodseeker_bloodrage"))
                {
                    dmgResult *= 1.4;
                }
                if (victim.NetworkName == "CDOTA_Unit_Hero_SkeletonKing" && victim.Spellbook.SpellR.Cooldown <= 0 && victim.Mana > 140)
                    dmgResult = 0;
            }
            catch (Exception)
            {
            }

            return dmgResult;
        } // GetDamageTaken::END
        public double CalculateMana(Hero victim)
        {
            double manacost = 0;
            try
            {

                var distance = me.Distance2D(victim);

                var startManaCost = R.GetAbilityData("ball_lightning_initial_mana_base") +
                                    me.MaximumMana / 100 * R.GetAbilityData("ball_lightning_initial_mana_percentage");
                var costPerUnit = (12 + me.MaximumMana * 0.007) / 100.0;

                var rManacost = startManaCost + costPerUnit * Math.Floor(distance / 100) * 100;

                var qReady = Q != null && Config.AbilityToggler.Value.IsEnabled(Q?.Name)
                             && Q.Cooldown <= 0;
                var wReady = W != null && Config.AbilityToggler.Value.IsEnabled(W?.Name)
                             && W.Cooldown <= 0;
                var rReady = R != null && Config.AbilityToggler.Value.IsEnabled(R?.Name)
                             && R.Cooldown <= 0;
                if (arcane != null && arcane.Item.Cooldown <= 0
                    && Config.ItemToggler.Value.IsEnabled(arcane.Item.Name))
                {
                    manacost -= 125;
                }
                if (soul != null && soul.Item.Cooldown <= 0
                    && Config.ItemToggler.Value.IsEnabled(soul.Item.Name))
                {
                    manacost -= 150;
                }
                if (rReady)
                {
                    manacost += rManacost;
                }
                if (qReady)
                {
                    manacost += Q.ManaCost;
                }
                if (vail != null && vail.Item.Cooldown <= 0
                    && Config.ItemToggler.Value.IsEnabled(vail.Item.Name)
                    && !ExUnit.HasModifier(victim, "modifier_item_veil_of_discord_debuff"))
                {
                    manacost += vail.Item.ManaCost;
                }
                if (wReady)
                {
                    manacost += W.ManaCost;
                }
                if (orchid != null
                    && orchid.Item.IsValid && orchid.Item.Cooldown <= 0 && Config.ItemToggler.Value.IsEnabled("item_bloodthorn"))
                {
                    manacost += orchid.Item.ManaCost;
                }
                if (bloodthorn != null
                    && bloodthorn.Item.IsValid  && bloodthorn.Item.Cooldown <= 0 && Config.ItemToggler.Value.IsEnabled("item_bloodthorn"))
                {
                    manacost += bloodthorn.Item.ManaCost;
                }
                if (lotus != null
                    && lotus.Item.IsValid && lotus.Item.Cooldown <= 0 && Config.ItemToggler.Value.IsEnabled(lotus.Item.Name))
                {
                    manacost += lotus.Item.ManaCost;
                }
                if (sheep != null
                    && sheep.Item.IsValid && sheep.Item.Cooldown <= 0 && Config.ItemToggler.Value.IsEnabled(sheep.Item.Name))
                {
                    manacost += sheep.Item.ManaCost;
                }

                var dagon = me.GetDagon();
                if (dagon != null
                    && dagon.IsValid && dagon.Cooldown <= 0 && Config.ItemToggler.Value.IsEnabled("item_dagon_5"))
                {
                    manacost += dagon.ManaCost;
                }
                if (shiva != null
                    && shiva.Item.IsValid && shiva.Item.Cooldown <= 0 && Config.ItemToggler.Value.IsEnabled(shiva.Item.Name))
                {
                    manacost += shiva.Item.ManaCost;
                }
                if (ethereal != null
                    && ethereal.Item.IsValid && ethereal.Item.Cooldown <= 0)
                {
                    manacost += ethereal.Item.ManaCost;
                }
            }
            catch (Exception)
            {
            }

            return manacost;
        } // GetDamageTaken::END

    }
}