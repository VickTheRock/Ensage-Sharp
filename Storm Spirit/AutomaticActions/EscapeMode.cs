namespace StormSpirit
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Ensage.Common.Extensions;
    using Ensage.Common.Threading;
    using Ensage.SDK.Helpers;
    using SharpDX;
    using Ensage;
    using Ensage.SDK.Extensions;
    using ExUnit = Ensage.SDK.Extensions.UnitExtensions;

    partial class Combo
    {
        public virtual async Task AutoAbilities()
        {
            var e = TargetSelector.Active.GetTargets()
                .FirstOrDefault(x => !x.IsInvulnerable() && x.IsAlive);
            if (e == null) return;
            var f =
                EntityManager<Unit>.Entities.First(x => x.Team == me?.Team && x.ClassId == ClassId.CDOTA_Unit_Fountain);

            float angle = f.FindAngleBetween(me.Position, true);
            Vector3 pos = new Vector3((float) (me.Position.X - 1500 * Math.Cos(angle)),
                (float) (me.Position.Y - 1500 * Math.Sin(angle)), 0);

            var distance = me.Distance2D(e);
            if (me.IsAlive
                && distance <= 800
                && R != null
                && !R.IsInAbilityPhase
                && !R.IsChanneling
                && R.CanBeCasted()
                && Config.Escape.Value.IsEnabled(R.Name)
                && e.Health > me.Health
            )
            {
                R.UseAbility(pos);
                if (
                    travel != null
                    && travel.Item.IsValid
                    && travel.Item.CanBeCasted()
                    && !ExUnit.IsChanneling(me)
                    && Config.Escape.Value.IsEnabled("item_travel_boots_2")
                )
                    travel.UseAbility(f.Position);
                else if (
                    travel2 != null
                    && travel2.Item.IsValid
                    && travel2.Item.CanBeCasted()
                    && !ExUnit.IsChanneling(me)
                    && Config.Escape.Value.IsEnabled("item_travel_boots_2")
                )
                    travel2.UseAbility(f.Position);
                else if (
                    tp != null
                    && tp.Item.IsValid
                    && tp.Item.CanBeCasted()
                    && !ExUnit.IsChanneling(me)
                    && Config.Escape.Value.IsEnabled("item_travel_boots_2")
                )
                    tp.UseAbility(f.Position);
            }
            await Await.Delay(250);
        }
    }
}