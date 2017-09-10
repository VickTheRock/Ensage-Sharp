namespace StormSpirit
{
    using System.Linq;
    using ExUnit = Ensage.SDK.Extensions.UnitExtensions;
    using System.Threading.Tasks;
    using Ensage.Common.Threading;
    using SharpDX;
    using Ensage;
    
    partial class Combo
    {
        public virtual async Task DrawingTargetDisplay()
        {
            var e = TargetSelector.Active.GetTargets()
                .FirstOrDefault(x => !ExUnit.IsInvulnerable(x) && x.IsAlive);
            if (e == null && Effect != null)
            {
                Effect.Dispose();
                Effect = null;
                await Await.Delay(100);
            }
            if (e == null || !e.IsValid || !e.IsAlive) return;
            if (Effect == null || !Effect.IsValid)
            {
                Effect = new ParticleEffect(@"particles\ui_mouseactions\range_finder_tower_aoe.vpcf", e);
                Effect.SetControlPoint(2, new Vector3(me.Position.X, me.Position.Y, me.Position.Z));
                Effect.SetControlPoint(6, new Vector3(1, 0, 0));
                Effect.SetControlPoint(7, new Vector3(e.Position.X, e.Position.Y, e.Position.Z));
                await Await.Delay(100);
            }
            else
            {
                Effect.SetControlPoint(2, new Vector3(me.Position.X, me.Position.Y, me.Position.Z));
                Effect.SetControlPoint(6, new Vector3(1, 0, 0));
                Effect.SetControlPoint(7, new Vector3(e.Position.X, e.Position.Y, e.Position.Z));
                await Await.Delay(100);
            }
            await Await.Delay(100);
        }
    }
}