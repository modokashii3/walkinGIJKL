using SharpPluginLoader.Core;
using SharpPluginLoader.Core.Entities;
using SharpPluginLoader.Core.IO;
using SharpPluginLoader.Core.Memory;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KeyboardWalk
{
    public class KeyboardWalk : IPlugin
    {
        public string Name => "KeyboardWalk";

        private readonly Patch _inputPatch = new((nint)0x140485c14, [0x90, 0x90, 0x90, 0x90, 0x90]);
        public void OnUpdate(float deltaTime)
        {
            if (Input.IsPressed(Key.LeftControl))
            {
                _inputPatch.Enable();
            }
            else if (Input.IsReleased(Key.LeftControl))
            {
                _inputPatch.Disable();
            }
        }

        public unsafe void SetKeysReleased(Key[] keys)
        {
            var keyboard = MemoryUtil.Read<nint>(0x1451c3170);
            var state = MemoryUtil.AsSpan<uint>(keyboard + 0x138, 8);
            var vkTable = MemoryUtil.AsSpan<byte>(keyboard + 0x38, 256);

            foreach (var key in keys)
            {
                var vk = vkTable[(int)key];
                state[vk >> 5] &= ~(1u << (vk & 0x1F));
            }
        }

        private delegate void PadUpdate(nint pad);
        private Hook<PadUpdate> _padUpdateHook = null!;

        public PluginData OnLoad()
        {
            _padUpdateHook = Hook.Create<PadUpdate>(PadUpdateHook, 0x141b15af0);
            return new PluginData()
            {
                OnUpdate = true
            };
        }


        public void PadUpdateHook(nint pad)
        {
            var sMhArea = MemoryUtil.Read<nint>(0x1451c2078);
            var stageId = MemoryUtil.Read<int>(sMhArea + 0xD328);

            if (Area.CurrentStage == Stage.SelianaHub || Area.CurrentStage == Stage.Seliana || Area.CurrentStage == Stage.Astera || Area.CurrentStage == Stage.AsteraHub)
            
            _padUpdateHook.Original(pad);
            ref var xAxis = ref MemoryUtil.GetRef<int>(pad + 0x1B8);
            ref var yAxis = ref MemoryUtil.GetRef<int>(pad + 0x1BC);

            if (Input.IsDown(Key.LeftControl))
            {
                if (Input.IsDown(Key.IW))
                {
                    yAxis = 12288;
                }
                else if (Input.IsDown(Key.K))
                {
                    yAxis = -12288;
                }
                else
                {
                    yAxis = 0;
                }

                if (Input.IsDown(Key.L))
                {
                    xAxis = 12288;
                }
                else if (Input.IsDown(Key.J))
                {
                    xAxis = -12288;
                }
                else
                {
                    xAxis = 0;
                }

                SetKeysReleased([Key.W, Key.A, Key.S, Key.D]);
            }
            else
            {
                yAxis = xAxis = 0;
            }
        }
    }
}
