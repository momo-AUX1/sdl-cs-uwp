using System;
using System.Diagnostics;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace xbox_uwp_sdl2_starter
{
    public sealed class AppView : IFrameworkView
    {
        private IntPtr _windowHandle;
        private HelloWorldSceneANGLE _scene;

        public void Initialize(CoreApplicationView applicationView)
        {
            UGL.SetHint("UGL_WINRT_HANDLE_BACK_BUTTON", "1");
        }

        public void SetWindow(CoreWindow window)
        {
            _windowHandle = UGL.SDL_CreateWindow("XBOX UWP UGL Starter", 0, 0,
                                                  (int)window.Bounds.Width,
                                                  (int)window.Bounds.Height,
                                                  UGL.WindowFlags.SDL_WINDOW_OPENGL | UGL.WindowFlags.SDL_WINDOW_SHOWN);
            window.Activate();
        }

        public void Load(string entryPoint)
        {
            // Here we request an OpenGL ES 3.0 context (supports 2.0 and 3.0).
            UGL.SDL_Init();
            UGL.SDL_GL_CreateContext(_windowHandle, 3, 0);
            _scene = new HelloWorldSceneANGLE();
            _scene.Initialize();
        }

        public void Run()
        {
            UGL.SDL_MainLoop(() =>
            {
                // Minimal example of getting button presses
                if (UGL.GetButton(UGL.Buttons.A))
                    Debug.WriteLine("Button A pressed");
                if (UGL.GetButton(UGL.Buttons.B))
                    Debug.WriteLine("Button B pressed");
                if (UGL.GetButton(UGL.Buttons.X))
                    Debug.WriteLine("Button X pressed");
                if (UGL.GetButton(UGL.Buttons.Y))
                    Debug.WriteLine("Button Y pressed");
                if (UGL.GetButton(UGL.Buttons.L1))
                    Debug.WriteLine("Button L1 pressed");
                if (UGL.GetButton(UGL.Buttons.R1))
                    Debug.WriteLine("Button R1 pressed");
                if (UGL.GetButton(UGL.Buttons.L2))
                    Debug.WriteLine("Button L2 pressed");
                if (UGL.GetButton(UGL.Buttons.R2))
                    Debug.WriteLine("Button R2 pressed");


                _scene.Render();
                UGL.SDL_GL_SwapWindow(_windowHandle);
            });
        }

        public void Uninitialize()
        {
            _scene?.Cleanup();
            UGL.SDL_Quit();
        }
    }

    // The IFrameworkViewSource that creates our view.
    public sealed class AppViewSource : IFrameworkViewSource
    {
        public IFrameworkView CreateView()
        {
            return new AppView();
        }
    }

    // The static Main entry point.
    static class ProgramAngle
    {
        [MTAThread]
        static void MainAngle(string[] args)
        {
            CoreApplication.Run(new AppViewSource());  // if using ANGLE you should use MainAngle as the entry point or replace Program.cs with this file and replace MainAngle with Main.
        }
    }
}
