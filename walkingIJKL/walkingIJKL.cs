using SharpPluginLoader.Core;
using SharpPluginLoader.Core.IO;
using SharpPluginLoader.Core.Memory;

namespace walkinGIJKL
{
    public class walkinGIJKL : IPlugin 
    {
        public string Name => "walkinGIJKL";
        public string Author => "seka";

        private Hook<InputUpdateDelegate> _inputUpdateHook = null!;
        private delegate void InputUpdateDelegate(nint pad);

        public void OnLoad()
        {
            _inputUpdateHook = Hook.Create<InputUpdateDelegate>(0x1422a5040, InputUpdateHook); // 0x141b15af0 in ver15.21
        }

        public void InputUpdateHook(nint pad)
        {
            _inputUpdateHook.Original(pad);
            ref var yAxis = ref MemoryUtil.GetRef<int>(pad + 0x1BC);
            ref var xAxis = ref MemoryUtil.GetRef<int>(pad + 0x1B8);

            if (Input.IsDown(Key.I) && Input.IsDown(Key.G))
                yAxis = 12288;
            else if (Input.IsDown(Key.K) && Input.IsDown(Key.G))
                yAxis = -12288;
            else if (!Input.IsDown(Key.K) && !Input.IsDown(Key.I))
                yAxis = 0;

            if (Input.IsDown(Key.L) && Input.IsDown(Key.G))
                xAxis = 12288;
            else if (Input.IsDown(Key.J) && Input.IsDown(Key.G))
                xAxis = -12288;
            else if (!Input.IsDown(Key.J) && !Input.IsDown(Key.L))
                xAxis = 0;
        }
    }
}
