namespace StormSpirit
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using SharpDX;
    using PlaySharp.Toolkit.Helper.Annotations;
    using AbilityId = Ensage.AbilityId;
    using ExUnit = Ensage.SDK.Extensions.UnitExtensions;
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;
    using Ensage.Common.Threading;
    using Ensage.SDK.Orbwalker.Modes;
    using Ensage.SDK.Prediction;
    using Ensage.SDK.TargetSelector;
    using Ensage.SDK.Abilities.Items;
    using Ensage.SDK.Inventory.Metadata;
    using Ensage.SDK.Service;
    using Ensage.SDK.Extensions;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Inventory;

    [PublicAPI]
    partial class Combo : KeyPressOrbwalkingModeAsync
    {
        public ConfigInit Config { get; }
        private IPrediction Prediction { get; }
        private ITargetSelectorManager TargetSelector { get; }
        protected Hero me;
        private readonly IServiceContext context;
        private bool cooldown;
        public Combo(
            Key key,
            ConfigInit config,
            IServiceContext context)
            : base(context, key)
        {

            this.context = context;
            this.Config = config;
            TargetSelector = context.TargetSelector;
            Inventory = context.Inventory;
            Prediction = context.Prediction;

        }



        private Ability W { get; set; }

        private Ability E { get; set; }

        private Ability R { get; set; }

        private Ability Q { get; set; }

        private IInventoryManager Inventory { get; }

        [ItemBinding]

        public item_ethereal_blade ethereal { get; private set; }
        [ItemBinding]
        public item_mjollnir mjollnir { get; private set; }
        [ItemBinding]
        public item_force_staff force { get; private set; }
        [ItemBinding]
        public item_urn_of_shadows urn { get; private set; }
        [ItemBinding]
        public item_black_king_bar bkb { get; private set; }
        [ItemBinding]
        public item_lotus_orb lotus { get; private set; }
        [ItemBinding]
        public item_veil_of_discord vail { get; private set; }
        [ItemBinding]
        public item_cheese cheese { get; private set; }
        [ItemBinding]
        public item_orchid orchid { get; private set; }
        [ItemBinding]
        public item_shivas_guard shiva { get; private set; }
        [ItemBinding]
        public item_bloodthorn bloodthorn { get; private set; }
        [ItemBinding]
        public item_rod_of_atos atos { get; private set; }
        [ItemBinding]
        public item_soul_ring soul { get; private set; }
        [ItemBinding]
        public item_arcane_boots arcane { get; private set; }

        [ItemBinding]
        public item_sheepstick sheep { get; private set; }
        [ItemBinding]
        public item_travel_boots travel { get; private set; }
        [ItemBinding]

        public item_cyclone cyclone { get; private set; }
        [ItemBinding]
        public item_travel_boots_2 travel2 { get; private set; }
        [ItemBinding]
        public item_tpscroll tp { get; private set; }
        public ParticleEffect BlinkRange { get; set; }
        public ParticleEffect QRange { get; set; }
        public ParticleEffect WRange { get; set; }
        public ParticleEffect RRange { get; set; }
        public ParticleEffect Effect;

        public override async Task ExecuteAsync(CancellationToken token)
        {
            var e = TargetSelector.Active.GetTargets()
                .FirstOrDefault(x => !x.IsInvulnerable() && x.IsAlive);
            if (e == null) return;
            var debuff = e.FindModifier("modifier_storm_spirit_electric_vortex_pull") ?? e.FindModifier("modifier_sheepstick_debuff") ?? e.FindModifier("modifier_rod_of_atos_debuff");


            var buff = e.HasModifiers(new[]
            {
                "modifier_storm_spirit_electric_vortex_pull",
                "modifier_sheepstick_debuff",
                "modifier_rod_of_atos_debuff"
            }, false) || ExUnit.IsStunned(e) || ExUnit.IsHexed(e) || ExUnit.IsRooted(e);
            var checkTimeModif = debuff?.RemainingTime <= 0.2;

            var inUltBall = ExUnit.HasModifier(me, "modifier_storm_spirit_ball_lightning") ||
                            R.IsInAbilityPhase;
            var inOverload = ExUnit.HasModifier(me, "modifier_storm_spirit_overload");
            var lotusBuff = ExUnit.HasModifier(e, "modifier_item_lotus_orb_active");
            var inVortex = ExUnit.HasModifier(e, "modifier_storm_spirit_electric_vortex_pull");

            var travelSpeed = R.GetAbilityData("ball_lightning_move_speed", R.Level);
            var distance = me.Distance2D(e);
            var travelTime = distance / travelSpeed;
            var startManaCost = R.GetAbilityData("ball_lightning_initial_mana_base") +
                                me.MaximumMana / 100 * R.GetAbilityData("ball_lightning_initial_mana_percentage");
            var costPerUnit = (12 + me.MaximumMana * 0.007) / 100.0;
            var rManacost = startManaCost + costPerUnit * Math.Floor(distance / 100) * 100;
            var dagon = me.GetDagon();
            var enemies = EntityManager<Hero>.Entities.Count(
                              x => x.IsValid && x.IsAlive && !x.IsIllusion && x.Team != me.Team && x.Distance2D(me) <= 700) >= Config.Heel.Item.GetValue<Slider>().Value;
            if (!lotusBuff)
            {
                if (
                    // cheese
                    cheese != null
                    && cheese.Item.IsValid
                    && cheese.Item.CanBeCasted()
                    && (me.Health <= me.MaximumHealth * 0.3
                        || me.Mana <= me.MaximumMana * 0.2)
                    && distance <= 700
                    && Config.ItemToggler.Value.IsEnabled(cheese.Item.Name)
                )
                {
                    cheese.UseAbility();
                    await Await.Delay(GetItemDelay(e), token);
                }
                if ( // SoulRing Item 
                    soul != null
                    && soul.Item.IsValid
                    && soul.Item.CanBeCasted()
                    && Config.ItemToggler.Value.IsEnabled(soul.Item.Name)
                    && me.CanCast()
                    && me.Health >= me.MaximumHealth * 0.4
                    && me.Mana <= me.MaximumMana * 0.4
                )
                {
                    soul.UseAbility();
                    await Await.Delay(GetItemDelay(e), token);
                }
                if ( // Arcane Boots Item
                    arcane != null
                    && arcane.Item.IsValid
                    && arcane.Item.CanBeCasted()
                    && Config.ItemToggler.Value.IsEnabled(arcane.Item.Name)
                    && me.CanCast()
                    && me.Mana <= me.MaximumMana * 0.4
                )
                {
                    arcane.UseAbility();
                    await Await.Delay(GetItemDelay(e), token);
                }

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
                    && !ExUnit.IsMagicImmune(e)
                    && Config.AbilityToggler.Value.IsEnabled(R.Name)
                )
                {
                    if (e.NetworkActivity == NetworkActivity.Move)
                        R.CastSkillShot(e);
                    else
                        R.UseAbility(e.Position);
                    await Await.Delay(GetAbilityDelay(me, R) + (int)Math.Floor(travelTime * 1000), token);
                }
                else elsecount += 1;
                if (vail != null
                    && vail.Item.IsValid
                    && vail.Item.CanBeCasted()
                    && Config.ItemToggler.Value.IsEnabled(vail.Item.Name)
                    && me.CanCast()
                    && !ExUnit.IsMagicImmune(e)
                )
                {
                    vail.UseAbility(e.Position);
                    await Await.Delay(GetItemDelay(e), token);
                }
                if (orchid != null
                    && orchid.Item.IsValid
                    && orchid.Item.CanBeCasted()
                    && me.CanCast()
                    && !ExUnit.IsLinkensProtected(e)
                    && !ExUnit.IsMagicImmune(e)
                    && Config.ItemToggler.Value.IsEnabled("item_bloodthorn")
                )
                {
                    orchid.UseAbility(e);
                    await Await.Delay(GetItemDelay(e), token);
                }
                if (bloodthorn != null
                    && bloodthorn.Item.IsValid
                    && bloodthorn.Item.CanBeCasted()
                    && me.CanCast()
                    && !ExUnit.IsLinkensProtected(e)
                    && !ExUnit.IsMagicImmune(e)
                    && Config.ItemToggler.Value.IsEnabled("item_bloodthorn")
                )
                {
                    bloodthorn.UseAbility(e);
                    await Await.Delay(GetItemDelay(e), token);
                }
                if (elsecount == 2
                    && sheep != null
                    && sheep.Item.IsValid
                    && sheep.Item.CanBeCasted()
                    && me.CanCast()
                    && (checkTimeModif || !buff)
                    && !ExUnit.IsLinkensProtected(e)
                    && !ExUnit.IsMagicImmune(e)
                    && e.ClassId != ClassId.CDOTA_Unit_Hero_Tidehunter
                    && Config.ItemToggler.Value.IsEnabled(sheep.Item.Name)
                )
                {
                    sheep.UseAbility(e);
                    await Await.Delay(GetItemDelay(e), token);
                }
                else elsecount += 1;
                if (elsecount == 3
                    && W != null
                    && W.CanBeCasted()
                    && (Config.fastVortex.Value || !inOverload)
                    && (checkTimeModif || !buff)
                    && !me.IsAttacking()
                    && me.CanCast()
                    && !ExUnit.IsMagicImmune(e)
                    && Config.AbilityToggler.Value.IsEnabled(W.Name)
                )
                {
                    W.UseAbility(e);
                    await Await.Delay(GetAbilityDelay(me, W) + 50, token);
                }
                else elsecount += 1;
                if (elsecount == 4
                    && ExUnit.CanAttack(me)
                    && !ExUnit.HasModifier(e, "modifier_item_ethereal_blade_slow")
                    && inOverload
                    && !ExUnit.IsAttackImmune(e)
                    && distance <= me.GetAttackRange()
                    && !ExUnit.IsAttackImmune(e)
                )
                {
                    me.Attack(e);
                    await Await.Delay(250, token);
                }
                else elsecount += 1;
                if (elsecount == 5
                    && W != null
                    && W.CanBeCasted()
                    && !inOverload
                    && me.AghanimState()
                    && (checkTimeModif || !buff)
                    && !me.IsAttacking()
                    && me.CanCast()
                    && !ExUnit.IsMagicImmune(e)
                    && Config.AbilityToggler.Value.IsEnabled(W.Name)
                )
                {
                    W.UseAbility();
                    await Await.Delay(GetAbilityDelay(me, W), token);
                }
                else elsecount += 1;
                if (elsecount == 6
                    && atos != null
                    && atos.Item.IsValid
                    && atos.Item.CanBeCasted()
                    && me.CanCast()
                    && (checkTimeModif || !buff)
                    && !ExUnit.IsLinkensProtected(e)
                    && !ExUnit.IsMagicImmune(e)
                    && Config.ItemToggler.Value.IsEnabled(atos.Item.Name)
                )
                {
                    atos.UseAbility(e);
                    await Await.Delay(GetItemDelay(e) + 100, token);
                }
                else elsecount += 1;
                if (elsecount == 7
                    && ethereal != null
                    && ethereal.Item.IsValid
                    && ethereal.Item.CanBeCasted()
                    && me.CanCast()
                    && !ExUnit.IsLinkensProtected(e)
                    && !ExUnit.IsMagicImmune(e)
                    && Config.ItemToggler.Value.IsEnabled(ethereal.Item.Name)
                )
                {
                    if (Config.debuff.Value)
                    {
                        var speed = ethereal.Item.GetAbilityData("projectile_speed");
                        var time = e.Distance2D(me) / speed;
                        ethereal.UseAbility(e);
                        await Await.Delay((int)(time * 1000.0f + Game.Ping) + 30, token);
                    }
                    else
                    {
                        ethereal.UseAbility(e);
                        await Await.Delay(GetItemDelay(e), token);
                    }
                }
                else elsecount += 1;
                if (elsecount == 8
                    && dagon != null
                    && dagon.IsValid
                    && dagon.CanBeCasted()
                    && Config.ItemToggler.Value.IsEnabled("item_dagon_5")
                    && me.CanCast()
                    && (ethereal == null
                        || ExUnit.HasModifier(e, "modifier_item_ethereal_blade_slow")
                        || ethereal.Item.Cooldown < 17)
                    && !ExUnit.IsLinkensProtected(e)
                    && !ExUnit.IsMagicImmune(e)
                )
                {
                    dagon.UseAbility(e);
                    await Await.Delay(GetItemDelay(e), token);
                }
                else elsecount += 1;
                if (elsecount == 9
                    && Q != null
                    && Q.CanBeCasted()
                    && (W == null || !W.CanBeCasted() || !Config.AbilityToggler.Value.IsEnabled(W.Name))
                    && (!inOverload || inOverload && ExUnit.IsAttackImmune(e))
                    && distance <= Q.GetAbilityData("static_remnant_radius") + me.HullRadius
                    && Config.AbilityToggler.Value.IsEnabled(Q.Name)
                    && me.CanCast()
                    && !ExUnit.IsMagicImmune(e)
                )
                {
                    Q.UseAbility();
                    await Await.Delay(GetAbilityDelay(me, Q), token);
                }
                else elsecount += 1;
                if (elsecount == 10
                    && urn != null
                    && urn.Item.IsValid && urn.Item.CanBeCasted() && urn.Item.CurrentCharges > 0
                    && distance <= urn.Item.GetCastRange()
                    && Config.ItemToggler.Value.IsEnabled(urn.Item.Name)
                )
                {
                    urn.UseAbility(e);
                    await Await.Delay(GetItemDelay(e), token);
                }
                else elsecount += 1;
                if (elsecount == 11
                    && bkb != null
                    && bkb.Item.IsValid
                    && bkb.Item.CanBeCasted()
                    && enemies
                    && Config.ItemToggler.Value.IsEnabled(bkb.Item.Name)
                )
                {
                    bkb.UseAbility();
                    await Await.Delay(GetItemDelay(e), token);
                }
                else elsecount += 1;
                if (elsecount == 12
                    && lotus != null
                    && lotus.Item.IsValid
                    && lotus.Item.CanBeCasted()
                    && enemies
                    && Config.ItemToggler.Value.IsEnabled(lotus.Item.Name)
                )
                {
                    lotus.UseAbility(me);
                    await Await.Delay(GetItemDelay(e), token);
                }
                else elsecount += 1;
                if (elsecount == 13
                    && shiva != null
                    && shiva.Item.IsValid
                    && shiva.Item.CanBeCasted()
                    && me.CanCast()
                    && !ExUnit.IsMagicImmune(e)
                    && Config.ItemToggler.Value.IsEnabled(shiva.Item.Name)
                    && distance <= 600
                )
                {
                    shiva.UseAbility();
                    await Await.Delay(GetItemDelay(e), token);
                }
                else elsecount += 1;
                if (elsecount == 14
                   && mjollnir != null
                    && mjollnir.Item.IsValid
                   && mjollnir.Item.CanBeCasted()
                   && me.CanCast()
                   && !ExUnit.IsMagicImmune(e)
                   && Config.ItemToggler.Value.IsEnabled(mjollnir.Item.Name)
                   && me.Distance2D(e) <= 600
                   )
                {
                    mjollnir.UseAbility(me);
                    await Await.Delay(GetItemDelay(e), token);
                }

            }
            else
            {
                if (Q != null
                    && Q.CanBeCasted()
                    && !inOverload
                    && (W == null || !W.CanBeCasted() || !Config.AbilityToggler.Value.IsEnabled(W.Name))
                    && distance <= Q.GetAbilityData("static_remnant_radius") + me.HullRadius
                    && Config.AbilityToggler.Value.IsEnabled(Q.Name)
                    && me.CanCast()
                    && !ExUnit.IsMagicImmune(e)
                )
                {
                    Q.UseAbility();
                    await Await.Delay(GetAbilityDelay(me, Q), token);
                }

                if (shiva != null
                    && shiva.Item.IsValid
                    && shiva.Item.CanBeCasted()
                    && me.CanCast()
                    && !ExUnit.IsMagicImmune(e)
                    && Config.ItemToggler.Value.IsEnabled(shiva.Item.Name)
                    && distance <= 600
                )
                {
                    shiva.UseAbility();
                    await Await.Delay(GetItemDelay(e), token);
                }
                if (W != null
                    && W.CanBeCasted()
                    && !inOverload
                    && me.AghanimState()
                    && (checkTimeModif || !buff)
                    && !me.IsAttacking()
                    && me.CanCast()
                    && !ExUnit.IsMagicImmune(e)
                    && Config.AbilityToggler.Value.IsEnabled(W.Name)
                )
                {
                    W.UseAbility();
                    await Await.Delay(GetItemDelay(e), token);
                }
            }
            Vector3 start = e.NetworkActivity == NetworkActivity.Move ? new Vector3((float)((R.GetCastDelay(me, me, true) + 0.3) * Math.Cos(e.RotationRad) * e.MovementSpeed + e.Position.X),
                                               (float)((R.GetCastDelay(me, me, true) + 0.5) * Math.Sin(e.RotationRad) * e.MovementSpeed + e.NetworkPosition.Y), e.NetworkPosition.Z) : e.NetworkPosition;
            if (R != null
                && R.CanBeCasted()
                && !inUltBall
                && !R.IsInAbilityPhase
                && !R.IsChanneling
                && (W == null || !W.CanBeCasted() || !Config.AbilityToggler.Value.IsEnabled(W.Name))
                && !ExUnit.IsChanneling(me)
                && (Config.AutoOverload.Value
                    && buff
                    || !buff)
                && !inOverload
                && me.Position.Distance2D(start) <= me.AttackRange
                && !ExUnit.IsMagicImmune(e)
                && (Config.savemanamode.Value
                && me.AttackBackswing() > 0.3 - me.TurnTime(e.NetworkPosition)
                && !cooldown
                    || !Config.savemanamode.Value)
                && Config.AbilityToggler.Value.IsEnabled(R.Name)
            )
            {
                me.Stop();
                float angle = e.FindAngleBetween(me.Position, true);
                Vector3 pos = new Vector3((float)(me.Position.X - 15 * Math.Cos(angle)),
                    (float)(me.Position.Y - 15 * Math.Sin(angle)), me.Position.Z);
                R.UseAbility(pos);
                await Await.Delay(GetAbilityDelay(me, R) + 100, token);
            }
            if (!ExUnit.IsAttackImmune(e) && !ExUnit.IsDisarmed(me) && !ExUnit.HasModifier(e, "modifier_item_ethereal_blade_slow"))
            {
                if (me.Distance2D(e) <= me.GetAttackRange() / 2 && ExUnit.CanAttack(me) && !me.IsAttacking())
                {
                    me.Attack(e);
                    await Await.Delay((int)(me.SecondsPerAttack * 100), token);
                }
                else
            if (Orbwalker.OrbwalkTo(e))
                {
                    return;
                }
            }
            await Await.Delay(5, token);
        }


        protected int GetAbilityDelay(Unit unit, Ability ability)
        {
            return (int)((ability.FindCastPoint() + me.GetTurnTime(unit)) * 1000.0);
        }

        protected int GetItemDelay(Unit unit)
        {
            return (int)(me.GetTurnTime(unit) * 1000);
        }

        protected int GetItemDelay(Vector3 pos)
        {
            return (int)(me.GetTurnTime(pos) * 1000);
        }

        private void GameDispatcher_OnIngameUpdate(EventArgs args)
        {
            if (Game.IsPaused || !me.IsAlive) return;
            var e = TargetSelector.Active.GetTargets()
                .FirstOrDefault(x => !x.IsInvulnerable() && x.IsAlive);
            try
            {
                cooldown = Orbwalking.AttackOnCooldown();
            }
            catch (Exception)
            {
            }
            if (e != null && ExUnit.IsLinkensProtected(e))
            {
                Await.Block("linken", linken);
            }
            if (e != null && me.Health <= me.MaximumHealth / 100 * Config.RHealh.Item.GetValue<Slider>().Value)
            {
                Await.Block("AutoAbilities", AutoAbilities);
            }
            Await.Block("DrawingTargetDisplay", DrawingTargetDisplay);
            Await.Block("DrawingRangeDisplay", DrawingRangeDisplay);

        }

        protected override void OnActivate()
        {
            me = Context.Owner as Hero;
            GameDispatcher.OnIngameUpdate += GameDispatcher_OnIngameUpdate;

            Q = ExUnit.GetAbilityById(me, AbilityId.storm_spirit_static_remnant);
            W = ExUnit.GetAbilityById(me, AbilityId.storm_spirit_electric_vortex);
            R = ExUnit.GetAbilityById(me, AbilityId.storm_spirit_ball_lightning);
            E = ExUnit.GetAbilityById(me, AbilityId.storm_spirit_overload);

            if (Config.DrawingDamageEnabled.Value)
            {
                Drawing.OnDraw += DrawingDamagePanel;
            }
            context.Inventory.Attach(this);

            base.OnActivate();
        }

        protected override void OnDeactivate()
        {
            Drawing.OnDraw -= DrawingDamagePanel;
            base.OnDeactivate();
            context.Inventory.Detach(this);
        }


    }
}
