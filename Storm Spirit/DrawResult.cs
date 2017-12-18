
using Ensage.SDK.Extensions;
using PlaySharp.Toolkit.Helper;

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
        private static void DrawUltiDamage(EventArgs args)
        {
            DrawingOnCore();
            enemies = ObjectManager.GetEntities<Hero>()
                .Where(x => x.IsVisible && x.IsAlive && x.Team != me.Team && !x.IsMagicImmune() && !x.IsIllusion).ToList();
            if (!Game.IsInGame || Game.IsPaused || Game.IsWatchingGame || enemies.Count == 0) return;

            if (Menu.Item("dmg").IsActive())
            {
                foreach (var v in enemies)
                {
                    damage[v.Handle] = (float)CalculateDamage(v);
                    var useMana = CalculateMana(v);
                    //Console.WriteLine("useMana" + useMana);
                    var screenPos = HUDInfo.GetHPbarPosition(v);

                    if (!OnScreen(v.Position)) continue;

                    var travelSpeed = R.GetAbilityData("ball_lightning_move_speed", R.Level);
                    var travelTime = me.Distance2D(v) / travelSpeed;
                    var distance = v.IsMoving ? me.Distance2D(v.Predict(travelTime)) : me.Distance2D(v);

                    var startManaCost = R.GetAbilityData("ball_lightning_initial_mana_base") +
                                        me.MaximumMana / 100 * R.GetAbilityData("ball_lightning_initial_mana_percentage");

                    var costPerUnit = (12 + me.MaximumMana * 0.007) / 100.0;
                    var calcEnemyHealth = v.Health<=0 ? 0 : v.Health - damage[v.Handle];
                    var calcMyMana = useMana >= me.Mana ? 0 : me.Mana - useMana;
                    var rManacost = startManaCost + costPerUnit * Math.Floor(distance / 100) * 100;
                    var text1 = v.Health <= damage[v.Handle] ? "✔ Damage:" + Math.Floor(damage[v.Handle]) + "(Easy Kill)"
                                                             : "✘ Damage:" + (int)Math.Floor(damage[v.Handle])+"("+ (int)calcEnemyHealth + ")";
                    var text2 = me.Mana >= useMana ? "✔ Mana:" + (int)Math.Floor(useMana)+"("+ (int)calcMyMana +")" : "✘ Mana:" + (int)Math.Floor(useMana)+"("+ (int)calcMyMana +")";
                    var text3 = me.Mana >= rManacost ? "✔ Distance:" + (int)me.Distance2D(v) : "✘ Distance:" + (int)me.Distance2D(v);
                    var size = new Vector2(15, 15);
                    var position1 = new Vector2(screenPos.X + 65, screenPos.Y + 12);
                    var position2 = new Vector2(screenPos.X + 65, screenPos.Y + 24);
                    var position3 = new Vector2(screenPos.X + 65, screenPos.Y + 36);


                    Drawing.DrawText(
                        text1,
                        new Vector2(screenPos.X + 64, screenPos.Y + 13),
                        size,
                        Color.Black,
                        FontFlags.DropShadow);
                    Drawing.DrawText(
                        text1,
                        position1,
                        size,
                        v.Health <= damage[v.Handle] ? Color.LawnGreen : Color.OrangeRed,
                        FontFlags.DropShadow);

                    Drawing.DrawText(
                        text2,
                        new Vector2(screenPos.X + 64, screenPos.Y + 25),
                        size,
                        Color.Black,
                        FontFlags.DropShadow);
                    Drawing.DrawText(
                        text2,
                        position2,
                        size,
                        me.Mana >= useMana ? Color.LawnGreen : Color.OrangeRed,
                        FontFlags.DropShadow);

                    Drawing.DrawText(
                        text3,
                        new Vector2(screenPos.X + 64, screenPos.Y + 37),
                        size,
                        Color.Black,
                        FontFlags.DropShadow);
                    Drawing.DrawText(
                        text3,
                        position3,
                        size,
                        me.Mana >= rManacost ? Color.LawnGreen : Color.OrangeRed,
                        FontFlags.DropShadow);
                }
            }
        } // DrawUltiDamage::END
    }
}