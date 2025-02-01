// // Barebones Example of how to use UWP SDL2 (C#) with a ported app (DLL) that uses OpenGL written in Rust.
// // You can take this as an example and go see the rust source code for more info.*
// As an added bonus, this project also includes a GL.cs file that contains the OpenGL bindings for SDL2.


using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using SDL2; 

namespace xbox_uwp_sdl2_starter
{
    class Program
    {
        [DllImport("extern_entry.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int external_entry();

        static void Main(string[] args)
        {
            SDL.SDL_SetHint("SDL_WINRT_HANDLE_BACK_BUTTON", "1");

            SDL.SDL_main_func mainFunction = SDLMain;
            SDL.SDL_WinRTRunApp(mainFunction, IntPtr.Zero);
        }

        private static int SDLMain(int argc, IntPtr argv)
        {
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO | SDL.SDL_INIT_AUDIO) != 0)
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


            int ret = external_entry();
            Debug.WriteLine($"external_entry returned: {ret}");

            SDL.SDL_GL_DeleteContext(glContext);
            SDL.SDL_DestroyWindow(window);
            SDL.SDL_Quit();

            return ret;
        }
    }
}
