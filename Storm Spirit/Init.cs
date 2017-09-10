namespace StormSpirit
{
    using System.ComponentModel.Composition;
    using System.Windows.Input;

    using Ensage;
    using Ensage.Common.Menu;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    [ExportPlugin("StormSpirit by Vick", HeroId.npc_dota_hero_storm_spirit)]
    partial class Init : Plugin
    {
        private readonly IServiceContext context;

        [ImportingConstructor]
        public Init([Import] IServiceContext context)
        {
            this.context = context;
        }

        public ConfigInit Config { get; private set; }

        public Combo OrbwalkerMode { get; private set; }

        protected override void OnActivate()
        {
            Config = new ConfigInit();
            var key = KeyInterop.KeyFromVirtualKey((int)Config.Key.Value.Key);
            Config.Key.Item.ValueChanged += HotkeyChanged;

            OrbwalkerMode = new Combo(key, Config, context);


            context.Orbwalker.RegisterMode(OrbwalkerMode);
        }

        protected override void OnDeactivate()
        {
            context.Orbwalker.UnregisterMode(OrbwalkerMode);
            Config.Key.Item.ValueChanged -= HotkeyChanged;
            Config.Dispose();
        }

        private void HotkeyChanged(object sender, OnValueChangeEventArgs e)
        {
            var keyCode = e.GetNewValue<KeyBind>().Key;
            if (keyCode == e.GetOldValue<KeyBind>().Key)
            {
                return;
            }

            var key = KeyInterop.KeyFromVirtualKey((int)keyCode);
            OrbwalkerMode.Key = key;
        }
    }
}