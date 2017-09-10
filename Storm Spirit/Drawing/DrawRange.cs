namespace StormSpirit
{
    using System.Threading.Tasks;
    using Ensage.Common.Extensions;
    using Ensage.SDK.Extensions;
    using Ensage.Common.Threading;
    using SharpDX;
    partial class Combo
    {
        public float lastblinkRange, lastqRange, lastwRange, lastrRange;
        public float blinkRange, qRange, wRange, rRange;
        public virtual async Task DrawingRangeDisplay()
        {
            if (Config.RangeStaticRemnant.Value && Q != null && Q.Level > 0)
            {
                qRange = Q.GetAbilityData("static_remnant_radius");
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
            if (Config.RangeElectricVortex.Value && W != null && W.Level > 0)
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
            await Await.Delay(500);
        }
        
    }
}