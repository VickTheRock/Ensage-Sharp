namespace StormSpirit
{
    using System.Threading.Tasks;
    using Ensage.Common.Threading;
    using System.Linq;
    using Ensage.Common.Extensions;
    using Ensage.SDK.Extensions;
    using ExUnit = Ensage.SDK.Extensions.UnitExtensions;

    partial class Combo
    {
        public virtual async Task linken()
        {
            var e = TargetSelector.Active.GetTargets()
                .FirstOrDefault(x => !x.IsInvulnerable() && x.IsAlive);
            if (ExUnit.IsInvisible(me)) return;

            var dagon = me.GetDagon();
            if ((cyclone != null && cyclone.Item.CanBeCasted() || force != null && force.Item.CanBeCasted()
                 || sheep != null && sheep.Item.CanBeCasted() || atos != null && atos.Item.CanBeCasted() || W != null && W.CanBeCasted())
                && me.Distance2D(e) <= 900)
            {
                if (cyclone != null && cyclone.Item.CanBeCasted()
                    && Config.Link.Value.IsEnabled(cyclone.Item.Name))
                    cyclone.UseAbility(e);
                else if (force != null && force.Item.CanBeCasted() &&
                         Config.Link.Value.IsEnabled(force.Item.Name))
                    force.UseAbility(e);
                else if (atos != null && atos.Item.CanBeCasted()
                         && Config.Link.Value.IsEnabled(atos.Item.Name))
                    atos.UseAbility(e);
                else if (dagon != null && dagon.CanBeCasted()
                         && Config.Link.Value.IsEnabled("item_dagon_5"))
                    dagon.UseAbility(e);
                else if (W != null && W.CanBeCasted() &&
                         Config.Link.Value.IsEnabled(W.Name)
                         && !ExUnit.IsMagicImmune(e))
                    W.UseAbility(e);
                else if (sheep != null && sheep.Item.CanBeCasted()
                         && Config.Link.Value.IsEnabled(sheep.Item.Name))
                    sheep.UseAbility(e);
            }
            await Await.Delay(250);
        }


    }
}