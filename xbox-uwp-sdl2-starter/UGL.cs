using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.Gaming.Input;
using Windows.UI.Core;

namespace xbox_uwp_sdl2_starter
{
    /// <summary>
    /// UGL provides an SDL–like API that wraps raw EGL, GLES, and (optionally) Windows.Gaming.Input for controller support.
    /// It handles window events (using the CoreWindow), the render loop, gamepad button checking,
    /// and exposes common GL functions.
    /// 
    /// Version 1.3 adds:
    /// - GetGamingDevice() and the Device enums to know what console are we currently in
    /// - SDL_GamepadVibrate() to control the gamepad vibration
    /// </summary>
    public static class UGL
    {
        #region Public Properties

        public static IntPtr Display { get; private set; }
        public static IntPtr Surface { get; private set; }
        public static IntPtr Context { get; private set; }
        public static IntPtr Config { get; private set; }
        public static CoreWindow Window { get; private set; }

        public static int RequestedGLESMajor { get; private set; } = 2;
        public static int RequestedGLESMinor { get; private set; } = 0;

        #endregion

        #region Private Fields

        private static Gamepad _activeGamepad;
        private static bool _isRunning;
        private static readonly System.Collections.Generic.List<Gamepad> _connectedGamepads =
            new System.Collections.Generic.List<Gamepad>();

        private static bool _handleBackButton = false;

        #endregion

        #region Public SDL-like Enums and Flags

        [Flags]
        public enum WindowFlags : uint
        {
            SDL_WINDOW_OPENGL = 0x00000002,
            SDL_WINDOW_SHOWN = 0x00000004,
            SDL_WINDOW_FULLSCREEN = 0x00000008
        }

        [Flags]
        public enum Buttons
        {
            None = 0,
            A,
            B,
            X,
            Y,
            L1,
            R1,
            L2,
            R2,
            DPadUp,
            DPadDown,
            DPadLeft,
            DPadRight
        }

        #endregion

        #region SDL-like Methods

        /// <summary>
        /// Mimics SDL_Init.
        /// </summary>
        public static void SDL_Init()
        {
            SetupGamepadEvents();
        }

        /// <summary>
        /// Sets a hint value. Currently supports:
        /// "UGL_WINRT_HANDLE_BACK_BUTTON" with a value of "1" to install a back button handler.
        /// </summary>
        public static void SetHint(string hint, string value)
        {
            if (hint == "UGL_WINRT_HANDLE_BACK_BUTTON" && value == "1")
            {
                _handleBackButton = true;
                CoreWindow currentWindow = CoreWindow.GetForCurrentThread();
                if (currentWindow != null)
                {
                    try
                    {
                        Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Error registering BackRequested event: " + ex.Message);
                    }
                }
                else
                {
                    Debug.WriteLine("CoreWindow not available yet; back button handler will be registered later.");
                }
            }
        }

        /// <summary>
        /// The back button event handler. Simply marks the event as handled.
        /// </summary>
        private static void App_BackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Mimics SDL_CreateWindow.
        /// </summary>
        public static IntPtr SDL_CreateWindow(
            string title,
            int x,
            int y,
            int w,
            int h,
            WindowFlags flags)
        {
            Window = CoreWindow.GetForCurrentThread();
            if (Window == null)
            {
                throw new Exception("Could not retrieve CoreWindow. Make sure you're on the UI thread.");
            }

            if (flags.HasFlag(WindowFlags.SDL_WINDOW_FULLSCREEN))
            {
                var view = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
                view.TryEnterFullScreenMode();
                //Debug.WriteLine(view.IsFullScreenMode);
            }

            try
            {
                // UWP does not allow setting Window.Title, so this is commented out.
                // Window.Title = title;
            }
            catch
            {
                Debug.WriteLine("Unable to set window title in UWP environment.");
            }

            // If the hint was set but we haven't registered the back button event, do it now.
            if (_handleBackButton)
            {
                try
                {
                    SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error registering BackRequested event in SDL_CreateWindow: " + ex.Message);
                }
            }

            IntPtr nativeWindow = Marshal.GetIUnknownForObject(Window);
            Marshal.Release(nativeWindow);
            return nativeWindow;
        }

        /// <summary>
        /// Returns the current window handle.
        /// </summary>
        public static IntPtr SDL_GetWindow()
        {
            if (Window == null)
            {
                throw new Exception("No CoreWindow has been created yet.");
            }
            return Marshal.GetIUnknownForObject(Window);
        }

        /// <summary>
        /// Emulates processing pending events.
        /// </summary>
        public static void SDL_PollEvents()
        {
            Window?.Dispatcher?.ProcessEvents(CoreProcessEventsOption.ProcessAllIfPresent);
        }

        /// <summary>
        /// Mimics SDL_GL_CreateContext to set up EGL and GLES on the current window.
        /// </summary>
        public static void SDL_GL_CreateContext(IntPtr windowHandle, int glesMajor = 2, int glesMinor = 0)
        {
            RequestedGLESMajor = glesMajor;
            RequestedGLESMinor = glesMinor;

            InitEGLCore(Window, glesMajor, glesMinor);
            GLES.Initialize();
        }

        /// <summary>
        /// Returns the current OpenGL ES context.
        /// </summary>
        public static IntPtr SDL_GL_GetContext()
        {
            if (Context == IntPtr.Zero)
            {
                throw new Exception("No OpenGLES context exists.");
            }
            return Context;
        }

        /// <summary>
        /// Mimics SDL_GL_SwapWindow.
        /// </summary>
        public static void SDL_GL_SwapWindow(IntPtr windowHandle)
        {
            if (!EGL.eglSwapBuffers(Display, Surface))
                throw new Exception("eglSwapBuffers failed.");
        }

        /// <summary>
        /// A simple main loop that calls the provided render callback each frame.
        /// </summary>
        public static void SDL_MainLoop(Action renderCallback)
        {
            _isRunning = true;
            while (_isRunning)
            {
                SDL_PollEvents();
                renderCallback();
                SDL_GL_SwapWindow(IntPtr.Zero);
            }
        }

        /// <summary>
        /// Stops the main loop and cleans up EGL.
        /// </summary>
        public static void SDL_Quit()
        {
            _isRunning = false;
            TerminateEGL();
        }

        #endregion

        #region Controller / GamePad Handling

        /// <summary>
        /// Returns true if the specified controller button is pressed.
        /// </summary>
        public static bool GetButton(Buttons button)
        {
            if (_activeGamepad == null)
                return false;

            var reading = _activeGamepad.GetCurrentReading();
            switch (button)
            {
                case Buttons.A:
                    return reading.Buttons.HasFlag(GamepadButtons.A);
                case Buttons.B:
                    return reading.Buttons.HasFlag(GamepadButtons.B);
                case Buttons.X:
                    return reading.Buttons.HasFlag(GamepadButtons.X);
                case Buttons.Y:
                    return reading.Buttons.HasFlag(GamepadButtons.Y);
                case Buttons.L1:
                    return reading.Buttons.HasFlag(GamepadButtons.LeftShoulder);
                case Buttons.R1:
                    return reading.Buttons.HasFlag(GamepadButtons.RightShoulder);
                case Buttons.L2:
                    return reading.LeftTrigger > 0.5;
                case Buttons.R2:
                    return reading.RightTrigger > 0.5;
                case Buttons.DPadUp:
                    return reading.Buttons.HasFlag(GamepadButtons.DPadUp);
                case Buttons.DPadDown:
                    return reading.Buttons.HasFlag(GamepadButtons.DPadDown);
                case Buttons.DPadLeft:
                    return reading.Buttons.HasFlag(GamepadButtons.DPadLeft);
                case Buttons.DPadRight:
                    return reading.Buttons.HasFlag(GamepadButtons.DPadRight);
                default:
                    return false;
            }
        }


        private static void SetupGamepadEvents()
        {
            _connectedGamepads.Clear();

            Gamepad.GamepadAdded += (sender, args) =>
            {
                if (!_connectedGamepads.Contains(args))
                    _connectedGamepads.Add(args);

                if (_activeGamepad == null)
                    _activeGamepad = args;
            };

            Gamepad.GamepadRemoved += (sender, args) =>
            {
                if (_connectedGamepads.Contains(args))
                    _connectedGamepads.Remove(args);

                if (_activeGamepad == args)
                {
                    _activeGamepad = null;
                    if (_connectedGamepads.Count > 0)
                        _activeGamepad = _connectedGamepads[0];
                }
            };

            foreach (var gp in Gamepad.Gamepads)
            {
                if (!_connectedGamepads.Contains(gp))
                    _connectedGamepads.Add(gp);

                if (_activeGamepad == null)
                    _activeGamepad = gp;
            }
        }

        public static void SDL_GamepadVibrate(double leftMotor, double rightMotor, double leftTrigger = 0, double rightTrigger = 0)
        {
            if (_activeGamepad != null)
            {
                var vibration = new GamepadVibration()
                {
                    LeftMotor = leftMotor,
                    RightMotor = rightMotor,
                    LeftTrigger = leftTrigger,
                    RightTrigger = rightTrigger
                };
                _activeGamepad.Vibration = vibration;
            }
        }

        #endregion

        #region EGL / GL Setup Internals

        /// <summary>
        /// Sets up EGL on the given CoreWindow, requesting the specified GLES version.
        /// </summary>
        private static void InitEGLCore(CoreWindow window, int glesMajorVersion, int glesMinorVersion)
        {
            Window = window;

            Display = EGL.eglGetDisplay(IntPtr.Zero);
            if (Display == IntPtr.Zero)
                throw new Exception("Failed to get EGL display.");

            if (!EGL.eglInitialize(Display, out int eglMajor, out int eglMinor))
                throw new Exception("Failed to initialize EGL.");

            if (!EGL.eglBindAPI(0x30A0))
                throw new Exception("Failed to bind OpenGL ES API.");

            bool requestES3OrHigher = (glesMajorVersion > 2);
            int renderableType = requestES3OrHigher ? EGL.EGL_OPENGL_ES3_BIT : EGL.EGL_OPENGL_ES2_BIT;

            int[] configAttribs = new int[]
            {
                EGL.EGL_RED_SIZE, 8,
                EGL.EGL_GREEN_SIZE, 8,
                EGL.EGL_BLUE_SIZE, 8,
                EGL.EGL_ALPHA_SIZE, 8,
                EGL.EGL_DEPTH_SIZE, 16,
                EGL.EGL_RENDERABLE_TYPE, renderableType,
                EGL.EGL_SURFACE_TYPE, EGL.EGL_WINDOW_BIT,
                EGL.EGL_NONE
            };

            IntPtr[] configs = new IntPtr[1];
            if (!EGL.eglChooseConfig(Display, configAttribs, configs, 1, out int numConfigs) || numConfigs < 1)
                throw new Exception("Failed to choose EGL config.");
            Config = configs[0];

            IntPtr nativeWindow = Marshal.GetIUnknownForObject(window);
            if (nativeWindow == IntPtr.Zero)
                throw new Exception("Failed to get native window pointer.");

            int[] surfaceAttribs = new int[] { EGL.EGL_NONE };
            Surface = EGL.eglCreateWindowSurface(Display, Config, nativeWindow, surfaceAttribs);
            Marshal.Release(nativeWindow);

            if (Surface == IntPtr.Zero)
                throw new Exception("Failed to create EGL window surface.");

            const int EGL_CONTEXT_MAJOR_VERSION = 0x3098;
            const int EGL_CONTEXT_MINOR_VERSION = 0x30FB;
            int[] contextAttribs;

            if (glesMajorVersion >= 3 && glesMinorVersion > 0)
            {
                contextAttribs = new int[]
                {
                    EGL_CONTEXT_MAJOR_VERSION, glesMajorVersion,
                    EGL_CONTEXT_MINOR_VERSION, glesMinorVersion,
                    EGL.EGL_NONE
                };
            }
            else
            {
                contextAttribs = new int[]
                {
                    EGL.EGL_CONTEXT_CLIENT_VERSION, glesMajorVersion,
                    EGL.EGL_NONE
                };
            }

            Context = EGL.eglCreateContext(Display, Config, IntPtr.Zero, contextAttribs);
            if (Context == IntPtr.Zero)
            {
                if (glesMajorVersion >= 3 && glesMinorVersion > 0)
                {
                    Debug.WriteLine($"Failed to create EGL context with minor={glesMinorVersion}, retrying without minor...");
                    contextAttribs = new int[]
                    {
                        EGL.EGL_CONTEXT_CLIENT_VERSION, glesMajorVersion,
                        EGL.EGL_NONE
                    };
                    Context = EGL.eglCreateContext(Display, Config, IntPtr.Zero, contextAttribs);
                }
                if (Context == IntPtr.Zero)
                    throw new Exception("Failed to create EGL context.");
            }

            if (!EGL.eglMakeCurrent(Display, Surface, Surface, Context))
                throw new Exception("Failed to make EGL context current.");
        }

        /// <summary>
        /// Cleans up the EGL context, surface, and display.
        /// </summary>
        private static void TerminateEGL()
        {
            if (Display != IntPtr.Zero)
            {
                EGL.eglMakeCurrent(Display, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                if (Context != IntPtr.Zero)
                {
                    EGL.eglDestroyContext(Display, Context);
                    Context = IntPtr.Zero;
                }
                if (Surface != IntPtr.Zero)
                {
                    EGL.eglDestroySurface(Display, Surface);
                    Surface = IntPtr.Zero;
                }
                EGL.eglTerminate(Display);
                Display = IntPtr.Zero;
            }
        }

        #endregion

        #region Utility / Common GL-like Methods

        /// <summary>
        /// Equivalent to glViewport.
        /// </summary>
        public static void Viewport(int x, int y, int width, int height)
        {
            GLES.glViewport(x, y, width, height);
        }

        #endregion

        #region External / UGL.h / UGL.cpp support

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void PollEventsDelegate();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int GetButtonDelegate(int button);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr GetContextDelegate();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int MainLoopDelegate(RenderCallbackDelegate renderCallback);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int RenderCallbackDelegate();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GamepadVibrateDelegate(double leftMotor, double rightMotor, double leftTrigger, double rightTrigger);

        public static void PollEventsWrapper()
        {
            SDL_PollEvents();
        }

        public static int GetButtonWrapper(int button)
        {
            return GetButton((Buttons)button) ? 1 : 0;
        }

        public static IntPtr GetContextWrapper()
        {
            return SDL_GL_GetContext();
        }

        public static int MainLoopWrapper(RenderCallbackDelegate renderCallback)
        {
            Action action = () => { renderCallback(); };
            SDL_MainLoop(action);
            return 0;
        }

        public static void GamepadVibrateWrapper(double leftMotor, double rightMotor, double leftTrigger, double rightTrigger)
        {
            SDL_GamepadVibrate(leftMotor, rightMotor, leftTrigger, rightTrigger);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetFunctionPointersDelegate(
            PollEventsDelegate pollEvents,
            GetButtonDelegate getButton,
            GetContextDelegate getContext,
            MainLoopDelegate mainLoop,
            GamepadVibrateDelegate gamepadVibrate);


        private static class NativeMethods32
        {
            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
            public static extern IntPtr LoadLibrary(string lpFileName);

            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
            public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        }

        /// <summary>
        /// This method is generic. It loads the native module with the given name,
        /// looks for the exported UGL_SetFunctionPointers, and if found, calls it
        /// passing our managed wrapper delegates.
        /// </summary>
        /// <param name="moduleName">The name of the native DLL (e.g. "snake_angle.dll")</param>
        public static void InitializeNativeExportsForModule(string moduleName)
        {
            IntPtr moduleHandle = NativeMethods32.LoadLibrary(moduleName);
            if (moduleHandle == IntPtr.Zero)
            {
                Debug.WriteLine("Failed to load module: " + moduleName);
                return;
            }

            IntPtr procAddress = NativeMethods32.GetProcAddress(moduleHandle, "UGL_SetFunctionPointers");
            if (procAddress == IntPtr.Zero)
            {
                Debug.WriteLine("Module " + moduleName + " does not export UGL_SetFunctionPointers.");
                return;
            }

            SetFunctionPointersDelegate setFP = (SetFunctionPointersDelegate)Marshal.GetDelegateForFunctionPointer(
                procAddress, typeof(SetFunctionPointersDelegate));

            setFP(
                new PollEventsDelegate(PollEventsWrapper),
                new GetButtonDelegate(GetButtonWrapper),
                new GetContextDelegate(GetContextWrapper),
                new MainLoopDelegate(MainLoopWrapper),
                 new GamepadVibrateDelegate(GamepadVibrateWrapper)
            );

            Debug.WriteLine("Registered UGL exports for module: " + moduleName);
        }

        #endregion

        #region Gaming Device Information
        public static class Device
        {
            public const uint NONE = 0;
            public const uint XBOX_ONE = 0x768BAE26;
            public const uint XBOX_ONE_S = 0x2A7361D9;
            public const uint XBOX_ONE_X = 0x5AD617C7;
            public const uint XBOX_ONE_X_DEVKIT = 0x10F7CDE3;
            public const uint XBOX_SERIES_S = 0x1D27FABB;
            public const uint XBOX_SERIES_X = 0x2F7A3DFF;
            public const uint XBOX_SERIES_X_DEVKIT = 0xDE8A5661;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct GAMING_DEVICE_MODEL_INFORMATION
        {
            public uint vendorId;
            public uint deviceId;
        }

        [DllImport("api-ms-win-gaming-deviceinformation-l1-1-0.dll", EntryPoint = "GetGamingDeviceModelInformation")]
        private static extern int GetGamingDeviceModelInformation(ref GAMING_DEVICE_MODEL_INFORMATION info);

        public static uint GetGamingDevice()
        {
            GAMING_DEVICE_MODEL_INFORMATION info = new GAMING_DEVICE_MODEL_INFORMATION();
            try
            {
                int hr = GetGamingDeviceModelInformation(ref info);
                if (hr != 0)
                {
                    Debug.WriteLine("Failed to retrieve gaming device information. HRESULT: " + hr);
                    return Device.NONE;
                }

                return info.deviceId;
            }
            catch (DllNotFoundException ex)
            {
                Debug.WriteLine("DLL not found: " + ex.Message);
                return Device.NONE;
            }
        }
        #endregion
    }
}
