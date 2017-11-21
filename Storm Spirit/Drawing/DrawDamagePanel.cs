namespace StormSpirit
{
    using System;
    using System.Linq;
    using Ensage.Common.Extensions;
    using Ensage;
    using Ensage.Common;
    using SharpDX;
    using Ensage.SDK.Extensions;
    using ExUnit = Ensage.SDK.Extensions.UnitExtensions;
    using Ensage.Common.Menu;
    partial class Combo
    {
        public static bool OnScreen(Vector3 v)
        {
            return Drawing.WorldToScreen(v) != Vector2.Zero;
        }
        private void DrawingDamagePanel(EventArgs args)
        {
            me = Context.Owner as Hero;
            var enemies = ObjectManager.GetEntities<Hero>()
                .Where(x => x.IsVisible && x.IsAlive && x.Team != me.Team && !ExUnit.IsMagicImmune(x) && !x.IsIllusion).ToList();
            if (!Game.IsInGame || Game.IsPaused || Game.IsWatchingGame || enemies.Count == 0 || !Config.DrawingDamageEnabled.Value) return;
            
            foreach (var v in enemies)
            {
                damage[v.Handle] = (float)CalculateDamage(v);
                var useMana = CalculateMana(v);
                //Console.WriteLine("useMana" + useMana);
                var screenPos = HUDInfo.GetHPbarPosition(v);

                if (!OnScreen(v.Position)) continue;
                var travelSpeed = R.GetAbilityData("ball_lightning_move_speed", R.Level);
                //var travelTime = me.Distance2D(v) / travelSpeed;
                var distance = me.Distance2D(v);

                var startManaCost = R.GetAbilityData("ball_lightning_initial_mana_base") +
                                    me.MaximumMana / 100 * R.GetAbilityData("ball_lightning_initial_mana_percentage");

                var costPerUnit = (12 + me.MaximumMana * 0.007) / 100.0;
                var calcEnemyHealth = v.Health <= 0 ? 0 : v.Health - damage[v.Handle];
                var calcMyMana = useMana >= me.Mana ? 0 : me.Mana - useMana;
                var rManacost = startManaCost + costPerUnit * Math.Floor(distance / 100) * 100;
                var text1 = v.Health <= damage[v.Handle] ? "✔ Damage:" + Math.Floor(damage[v.Handle]) + "(Easy Kill)"
                    : "✘ Damage:" + (int)Math.Floor(damage[v.Handle]) + "(" + (int)calcEnemyHealth + ")";
                var text2 = me.Mana >= useMana ? "✔ Mana:" + (int)Math.Floor(useMana) + "(" + (int)calcMyMana + ")" : "✘ Mana:" + (int)Math.Floor(useMana) + "(" + (int)calcMyMana + ")";
                var text3 = me.Mana >= rManacost ? "✔ Distance:" + (int)me.Distance2D(v) : "✘ Distance:" + (int)me.Distance2D(v);
                var size = new Vector2(Config.DrawingDamageSize.Item.GetValue<Slider>().Value, Config.DrawingDamageSize.Item.GetValue<Slider>().Value);
                var position1 = new Vector2(screenPos.X + 65, screenPos.Y + 12);
                var position2 = new Vector2(screenPos.X + 65, screenPos.Y + 24);
                var position3 = new Vector2(screenPos.X + 65, screenPos.Y + 36);
                var fountCount = Config.WeatherItem.Value.SelectedIndex;

                var fountName = Config.WeatherItem.Value.SList;
                
                if (Drawing.Direct3DDevice9 == null) return;
                Drawing.DrawText(
                    text1, fountName[fountCount],
                    new Vector2(screenPos.X + 64, screenPos.Y + 13),
                    size,
                    Color.Black,
                    FontFlags.GaussianBlur);
                Drawing.DrawText(
                    text1, fountName[fountCount],
                    position1,
                    size,
                    v.Health <= damage[v.Handle] ? Color.LawnGreen : Color.OrangeRed,
                    FontFlags.GaussianBlur);

                Drawing.DrawText(
                    text2, fountName[fountCount],
                    new Vector2(screenPos.X + 64, screenPos.Y + 25),
                    size,
                    Color.Black,
                    FontFlags.GaussianBlur);
                Drawing.DrawText( 
                    text2, fountName[fountCount],
                    position2,
                    size,
                    me.Mana >= useMana ? Color.LawnGreen : Color.OrangeRed,
                    FontFlags.GaussianBlur);

                Drawing.DrawText(
                    text3, fountName[fountCount],
                    new Vector2(screenPos.X + 64, screenPos.Y + 37),
                    size,
                    Color.Black,
                    FontFlags.GaussianBlur);
                Drawing.DrawText(
                    text3, fountName[fountCount],
                    position3,
                    size,
                    me.Mana >= rManacost ? Color.LawnGreen : Color.OrangeRed,
                    FontFlags.GaussianBlur);
            }
        }

    }
}