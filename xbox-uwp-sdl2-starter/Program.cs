using System;
using System.Diagnostics;
using SDL2;

namespace xbox_uwp_sdl2_starter
{
    class Program
    {
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

            HelloWorldScene scene = new HelloWorldScene();
            scene.Initialize();

            GameLoop(window, scene);

            scene.Cleanup();
            SDL.SDL_GL_DeleteContext(glContext);
            SDL.SDL_DestroyWindow(window);
            SDL.SDL_Quit();

            return 0;
        }

        private static void GameLoop(IntPtr window, HelloWorldScene scene)
        {
            SDL.SDL_Event e;
            bool running = true;
            while (running)
            {
                while (SDL.SDL_PollEvent(out e) != 0)
                {
                    switch (e.type)
                    {
                        case SDL.SDL_EventType.SDL_QUIT:
                            running = false;
                            break;
                        case SDL.SDL_EventType.SDL_WINDOWEVENT:
                            if (e.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED)
                            {
                                SDL.SDL_GetWindowSize(window, out int width, out int height);
                            }
                            break;
                    }
                }

                scene.Render();
                SDL.SDL_GL_SwapWindow(window);
            }
        }
    }
}