using SharpPluginLoader.Core;
using SharpPluginLoader.Core.IO;
using SharpPluginLoader.Core.Memory;

namespace walkingIJKL
{
    public class walkingIJKL : IPlugin 
    {
        public string Name => "walkingIJKL";

        private Hook<InputUpdateDelegate> _inputUpdateHook = null!;
        private delegate void InputUpdateDelegate(nint pad);

        public void InputUpdateHook(nint pad)
        {
            _inputUpdateHook.Original(pad);
            ref var yAxis = ref MemoryUtil.GetRef<int>(pad + 0x1BC);
            ref var xAxis = ref MemoryUtil.GetRef<int>(pad + 0x1B8);

            if (Input.IsDown(Key.I))
                yAxis = 12288;
            else if (Input.IsDown(Key.K))
                yAxis = -12288;
            else if (!Input.IsDown(Key.K) && !Input.IsDown(Key.I))
                yAxis = 0;

            if (Input.IsDown(Key.L))
                xAxis = 12288;
            else if (Input.IsDown(Key.J))
                xAxis = -12288;
            else if (!Input.IsDown(Key.J) && !Input.IsDown(Key.L))
                xAxis = 0;
        }

        public PluginData Initialize()
        {
            _inputUpdateHook = Hook.Create<InputUpdateDelegate>(0x141b15af0, InputUpdateHook);
            return new PluginData();
        }
        public void OnLoad() { }

    }
}
