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
            
            if ((cyclone != null && cyclone.CanBeCasted() || force != null && force.CanBeCasted()
                 || sheep != null && sheep.CanBeCasted() || atos != null && atos.CanBeCasted() || W != null && W.CanBeCasted())
                && me.Distance2D(e) <= 900)
            {
                if (cyclone != null && cyclone.CanBeCasted()
                    && Config.Link.Value.IsEnabled(cyclone.Name))
                    cyclone.UseAbility(e);
                else if (force != null && force.CanBeCasted() &&
                         Config.Link.Value.IsEnabled(force.Name))
                    force.UseAbility(e);
                else if (atos != null && atos.CanBeCasted()
                         && Config.Link.Value.IsEnabled(atos.Name))
                    atos.UseAbility(e);
                else if (dagon != null && dagon.CanBeCasted()
                         && Config.Link.Value.IsEnabled("item_dagon_5"))
                    dagon.UseAbility(e);
                else if (W != null && W.CanBeCasted() &&
                         Config.Link.Value.IsEnabled(W.Name)
                         && !ExUnit.IsMagicImmune(e))
                    W.UseAbility(e);
                else if (sheep != null && sheep.CanBeCasted()
                         && Config.Link.Value.IsEnabled(sheep.Name))
                    sheep.UseAbility(e);
            }
            await Await.Delay(250);
        }


    }
}