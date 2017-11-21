using Ensage.Common.Extensions;

namespace StormSpirit
{
    using System;
    using System.Collections.Generic;
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Menu;
    using SharpDX;
    using Ensage.SDK.Extensions;

    partial class StormSpirit
    {
        public static void DrawingOnCore()
        {
            if (Menu.Item("Range Blink").GetValue<bool>() && blink != null)
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
            if (Menu.Item("Range Static Remnant").GetValue<bool>() && Q != null && Q.Level > 0)
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
            if (Menu.Item("Range Electric Vortex").GetValue<bool>() && W != null && W.Level > 0)
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
        }
    }
}