
using Ensage.SDK.Extensions;
namespace StormSpirit
{
    using System;
    using System.Collections.Generic;
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Menu;
    using SharpDX;
    using Ensage.Common.Extensions;
    using System.Linq;

    partial class StormSpirit
    {
        public static double CalculateDamage(Hero victim)
        {
            
            bool vailOn = false;
            bool orchidOn = false;
            double dmgResult = 0;
            double manacost = 0;
            me = ObjectManager.LocalHero;
            //var inUltimate = (me.HasModifier("modifier_storm_spirit_ball_lightning") || R.IsInAbilityPhase);
           // var inPassve = me.HasModifier("modifier_storm_spirit_overload");
            var rLevel = R.Level;
            var distance = me.Distance2D(victim);

            var travelSpeed = R.GetAbilityData("ball_lightning_move_speed", rLevel);
            var startManaCost = R.GetAbilityData("ball_lightning_initial_mana_base") +
                                me.MaximumMana / 100 * R.GetAbilityData("ball_lightning_initial_mana_percentage");
            //Console.WriteLine("startManaCost " + startManaCost);

            sheep = victim.ClassId == ClassId.CDOTA_Unit_Hero_Tidehunter ? null : me.FindItem("item_sheepstick");
            var costPerUnit = (12 + me.MaximumMana * 0.007) / 100.0;

            var rManacost = startManaCost + costPerUnit * Math.Floor(distance / 100) * 100;
            
            var travelTime = distance / travelSpeed;
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

            var qReady = Q != null && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
                         && Q.CanBeCasted();
            var wReady = W != null && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
                         && W.CanBeCasted();
            var rReady = R != null && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
                         && R.CanBeCasted();
            var eReady = E != null && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(E.Name)
                         && E.Level > 0;
            if (arcane != null && arcane.CanBeCasted()
                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(arcane.Name))
            {
                manacost -= 125;
            }
            if (soul != null && soul.CanBeCasted()
                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(soul.Name))
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
            if (orchid != null && orchid.CanBeCasted() && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_bloodthorn")
                && !victim.HasModifier("modifier_bloodthorn_debuff")
                && !victim.HasModifier("modifier_orchid_malevolence_debuff"))
            {
                if (manacost + orchid.ManaCost < me.Mana)
                {
                    manacost += orchid.ManaCost;
                    orchidOn = true;
                }
                else goto gotoDamage;
            }
            if (vail != null && vail.CanBeCasted()
                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(vail.Name)
                && !victim.HasModifier("modifier_item_veil_of_discord_debuff"))
            {
                if (manacost + vail.ManaCost < me.Mana)
                {
                    manacost += vail.ManaCost;
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
                if (manacost + W.ManaCost < me.Mana)
                {
                    dmgResult += eDmg * (1 - victim.MagicDamageResist);
                }
                else goto gotoDamage;
            }
            var spellamplymult = 1 + me.TotalIntelligence / 16 / 100;
            dmgResult = dmgResult * spellamplymult;
            if (dagon != null && dagon.CanBeCasted() && victim.Handle == e?.Handle && Menu.Item("Items")
                    .GetValue<AbilityToggler>()
                    .IsEnabled("item_dagon"))
            {
                if (manacost + dagon.ManaCost < me.Mana)
                {
                    manacost += dagon.ManaCost;
                    dmgResult += dagonDmg[dagon.Level] * (1 - victim.MagicDamageResist);
                }
                else goto gotoDamage;
            }
            if (shiva != null && shiva.CanBeCasted() && Menu.Item("Items")
                    .GetValue<AbilityToggler>()
                    .IsEnabled(shiva.Name))
            {
                if (manacost + shiva.ManaCost < me.Mana)
                {
                    manacost += shiva.ManaCost;
                    dmgResult += 200 * (1 - victim.MagicDamageResist);
                }
                else goto gotoDamage;
            }

            int etherealdamage = (int)(me.TotalIntelligence * 2 + 75);

            if (ethereal != null && ethereal.CanBeCasted() && victim.Handle == e?.Handle)
            {
                if (manacost + ethereal.ManaCost < me.Mana)
                {
                    manacost += ethereal.ManaCost;
                    dmgResult *= 1.4 + etherealdamage;
                }
                else goto gotoDamage;
            }
            gotoDamage:
            
            if (me.HasModifier("modifier_special_bonus_spell_amplify"))
            {
                dmgResult *= 1.10;
            }
            if (victim.HasModifier("modifier_item_veil_of_discord_debuff"))
            {
                dmgResult *= 1.25;
            }
            if (vail != null && vail.CanBeCasted()
                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(vail.Name)
                && !victim.HasModifier("modifier_item_veil_of_discord_debuff"))
            {
                if (vailOn)
                {
                    dmgResult *= 1.25;
                }
            }
            if (orchid != null && orchid.CanBeCasted() && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_bloodthorn")
                && !victim.HasModifier("modifier_bloodthorn_debuff")
                && !victim.HasModifier("modifier_orchid_malevolence_debuff"))
            {
                if (orchidOn)
                {
                    dmgResult *= 1.3;
                }
            }
            if (sheep != null && sheep.CanBeCasted() && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(sheep.Name))
            {
                if (manacost + 100 < me.Mana)
                {
                    dmgResult += sheepDmg;
                }
            }
            if (orchid != null && orchid.CanBeCasted() && orchid.Name == "item_bloodthorn"
                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_bloodthorn") && orchidOn || victim.HasModifier("modifier_bloodthorn_debuff") || victim.HasModifier("modifier_orchid_malevolence_debuff"))
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
            
            if (victim.HasModifier("modifier_orchid_malevolence_debuff") || victim.HasModifier("modifier_bloodthorn_debuff"))
            {
                dmgResult *= 1.3;
            }
            if (victim.HasModifier("modifier_kunkka_ghost_ship_damage_absorb"))
                dmgResult *= 0.5;

            if (victim.HasModifier("modifier_bloodseeker_bloodrage") || me.HasModifier("modifier_bloodseeker_bloodrage"))
            {
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

            if (victim.NetworkName == "CDOTA_Unit_Hero_SkeletonKing" && victim.Spellbook.SpellR.Cooldown <= 0 && victim.Mana > 140)
                dmgResult = 0;
            return dmgResult;
        } // GetDamageTaken::END
        public static double CalculateMana(Hero victim)
        {
            double manacost = 0;
            me = ObjectManager.LocalHero;

            var distance = me.Distance2D(victim);

            var startManaCost = R.GetAbilityData("ball_lightning_initial_mana_base") +
                                me.MaximumMana / 100 * R.GetAbilityData("ball_lightning_initial_mana_percentage");
            var costPerUnit = (12 + me.MaximumMana * 0.007) / 100.0;

            var rManacost = startManaCost + costPerUnit * Math.Floor(distance / 100) * 100;

            var qReady = Q != null && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
                         && Q.Cooldown <= 0;
            var wReady = W != null && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W?.Name)
                         && W.Cooldown <= 0;
            var rReady = R != null && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R?.Name)
                         && R.Cooldown <= 0;
            if (arcane != null && arcane.Cooldown <= 0
                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(arcane.Name))
            {
                manacost -= 125;
            }
            if (soul != null && soul.Cooldown <= 0
                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(soul.Name))
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
            if (vail != null && vail.Cooldown <= 0
                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(vail.Name)
                && !victim.HasModifier("modifier_item_veil_of_discord_debuff"))
            {
                manacost += vail.ManaCost;
            }
            if (wReady)
            {
                manacost += W.ManaCost;
            }
            if (orchid != null && orchid.Cooldown <= 0 && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_bloodthorn"))
            {
                manacost += orchid.ManaCost;
            }
            if (lotus != null && lotus.Cooldown <= 0 && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(lotus.Name))
            {
                manacost += lotus.ManaCost;
            }
            if (sheep != null && sheep.Cooldown <= 0 && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(sheep.Name))
            {
                manacost += sheep.ManaCost;
            }

            if (dagon != null && dagon.Cooldown <= 0 && victim.Handle == e?.Handle && Menu.Item("Items")
                    .GetValue<AbilityToggler>()
                    .IsEnabled("item_dagon"))
            {
                manacost += dagon.ManaCost;
            }
            if (shiva != null && shiva.Cooldown <= 0 && Menu.Item("Items")
                    .GetValue<AbilityToggler>()
                    .IsEnabled(shiva.Name))
            {
                manacost += shiva.ManaCost;
            }
            if (ethereal != null && ethereal.Cooldown <= 0 && victim.Handle == e?.Handle)
            {
                manacost += ethereal.ManaCost;
            }
            return manacost;
        } // GetDamageTaken::END
    }
}