// // Barebones Example of how to use UWP SDL2 (C#) with a ported app (DLL) that uses OpenGL written in Rust.
// // You can take this as an example and go see the rust source code for more info.*
// As an added bonus, this project also includes a GL.cs file that contains the OpenGL bindings for SDL2.


using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using SDL2;
using Windows.Gaming.Input;
using Windows.System;
using Windows.UI.Core;
using Windows.Foundation;

namespace xbox_uwp_sdl2_starter
{
    class Program
    {

        [StructLayout(LayoutKind.Sequential)]
        public struct ControllerInput
        {
            public float left_x;
            public float left_y;
            public float right_x;
            public float right_y;
            [MarshalAs(UnmanagedType.I1)]
            public bool button_a;
            [MarshalAs(UnmanagedType.I1)]
            public bool button_b;
        }

        [DllImport("extern_entry_cube.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int external_entry();

        [DllImport("extern_entry_cube.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void update_controller_input(ControllerInput input);

        static void Main(string[] args)
        {
            SDL.SDL_SetHint("SDL_WINRT_HANDLE_BACK_BUTTON", "1");

            SDL.SDL_main_func mainFunction = SDLMain;
            SDL.SDL_WinRTRunApp(mainFunction, IntPtr.Zero);
        }

        private static ControllerInput PollThirdPartyController(CoreWindow window)
        {
            ControllerInput input = new ControllerInput();
            VirtualKey leftUp = (VirtualKey)211;
            VirtualKey leftDown = (VirtualKey)212;
            VirtualKey leftLeft = (VirtualKey)214;
            VirtualKey leftRight = (VirtualKey)213;

            var stateUpL = window.GetKeyState(leftUp);
            var stateDownL = window.GetKeyState(leftDown);
            var stateLeftL = window.GetKeyState(leftLeft);
            var stateRightL = window.GetKeyState(leftRight);

            input.left_y = 0.0f;
            if ((stateUpL & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down)
                input.left_y = 1.0f;
            if ((stateDownL & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down)
                input.left_y = -1.0f;

            input.left_x = 0.0f;
            if ((stateLeftL & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down)
                input.left_x = -1.0f;
            if ((stateRightL & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down)
                input.left_x = 1.0f;

            VirtualKey rightUp = (VirtualKey)215;
            VirtualKey rightDown = (VirtualKey)216;
            VirtualKey rightLeft = (VirtualKey)218;
            VirtualKey rightRight = (VirtualKey)217;

            var stateUpR = window.GetKeyState(rightUp);
            var stateDownR = window.GetKeyState(rightDown);
            var stateLeftR = window.GetKeyState(rightLeft);
            var stateRightR = window.GetKeyState(rightRight);

            input.right_y = 0.0f;
            if ((stateUpR & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down)
                input.right_y = 1.0f;
            if ((stateDownR & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down)
                input.right_y = -1.0f;

            input.right_x = 0.0f;
            if ((stateLeftR & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down)
                input.right_x = -1.0f;
            if ((stateRightR & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down)
                input.right_x = 1.0f;

            VirtualKey buttonA = (VirtualKey)195;
            VirtualKey buttonB = (VirtualKey)196;

            var stateA = window.GetKeyState(buttonA);
            var stateB = window.GetKeyState(buttonB);

            input.button_a = (stateA & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
            input.button_b = (stateB & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;

            return input;
        }

        private static ControllerInput GetInputFromUI(CoreWindow window)
        {
            ControllerInput result = new ControllerInput();
            var op = window.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                result = PollThirdPartyController(window);
            });
            while (op.Status == AsyncStatus.Started)
            {
                Thread.Sleep(1);
            }
            return result;
        }

        private static void ControllerPollingLoop(CoreWindow window)
        {
            while (true)
            {
                ControllerInput input = GetInputFromUI(window);
                update_controller_input(input);
                Thread.Sleep(16); 
            }
        }

        private static int SDLMain(int argc, IntPtr argv)
        {
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO | SDL.SDL_INIT_AUDIO | SDL.SDL_INIT_GAMECONTROLLER | SDL.SDL_INIT_JOYSTICK) != 0)
            {
                Debug.WriteLine($"SDL Initialization failed: {SDL.SDL_GetError()}");
                return -1;
            }

            if (SDL.SDL_GetCurrentDisplayMode(0, out SDL.SDL_DisplayMode displayMode) != 0)
            {
                Debug.WriteLine($"Failed to get display mode: {SDL.SDL_GetError()}");
                SDL.SDL_Quit();
                return -1;
            }

            IntPtr window = SDL.SDL_CreateWindow(
                "XBOX UWP SDL2 Starter",
                SDL.SDL_WINDOWPOS_CENTERED,
                SDL.SDL_WINDOWPOS_CENTERED,
                displayMode.w,
                displayMode.h,
                SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN
            );
            if (window == IntPtr.Zero)
            {
                Debug.WriteLine($"Window creation failed: {SDL.SDL_GetError()}");
                SDL.SDL_Quit();
                return -1;
            }

            IntPtr glContext = SDL.SDL_GL_CreateContext(window);
            if (glContext == IntPtr.Zero)
            {
                Debug.WriteLine($"OpenGL context creation failed: {SDL.SDL_GetError()}");
                SDL.SDL_DestroyWindow(window);
                SDL.SDL_Quit();
                return -1;
            }

            if (SDL.SDL_GL_SetSwapInterval(1) != 0)
            {
                Debug.WriteLine($"Warning: Unable to set VSync: {SDL.SDL_GetError()}");
            }



            var inputThread = new Thread(() =>
            {
                while (true)
                {
                    var gamepads = Gamepad.Gamepads;
                    if (gamepads.Count > 0)
                    {
                        var gamepad = gamepads[0];
                        var reading = gamepad.GetCurrentReading();

                        ControllerInput input = new ControllerInput
                        {
                            left_x = (float)reading.LeftThumbstickX,
                            left_y = (float)reading.LeftThumbstickY,
                            right_x = (float)reading.RightThumbstickX,
                            right_y = (float)reading.RightThumbstickY,
                            button_a = (reading.Buttons & GamepadButtons.A) != 0,
                            button_b = (reading.Buttons & GamepadButtons.B) != 0
                        };

                        update_controller_input(input);
                    }
                    Thread.Sleep(16); 
                }
            });
            inputThread.IsBackground = true;
            inputThread.Start();


            CoreWindow coreWindow = CoreWindow.GetForCurrentThread();
            var thirdinputThread = new Thread(() => ControllerPollingLoop(coreWindow));
            thirdinputThread.IsBackground = true;
            thirdinputThread.Start();

            int ret = external_entry();
            Debug.WriteLine($"external_entry returned: {ret}");

            SDL.SDL_GL_DeleteContext(glContext);
            SDL.SDL_DestroyWindow(window);
            SDL.SDL_Quit();

            return ret;
        }
    }
}
