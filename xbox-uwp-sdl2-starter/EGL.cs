using System;
using System.Runtime.InteropServices;

namespace xbox_uwp_sdl2_starter
{
    public static class EGL
    {
        public const int EGL_NONE = 0x3038;
        public const int EGL_RED_SIZE = 0x3024;
        public const int EGL_GREEN_SIZE = 0x3023;
        public const int EGL_BLUE_SIZE = 0x3022;
        public const int EGL_ALPHA_SIZE = 0x3021;
        public const int EGL_DEPTH_SIZE = 0x3025;
        public const int EGL_RENDERABLE_TYPE = 0x3040;
        public const int EGL_OPENGL_ES2_BIT = 0x0004;
        public const int EGL_OPENGL_ES3_BIT = 0x0040;
        public const int EGL_SURFACE_TYPE = 0x3033;
        public const int EGL_WINDOW_BIT = 0x0004;
        public const int EGL_CONTEXT_CLIENT_VERSION = 0x3098;
        public const int EGL_PBUFFER_BIT = 0x0001;
        public const int EGL_WIDTH = 0x3057;
        public const int EGL_HEIGHT = 0x3056;
        public const int EGL_SUCCESS = 0x3000;
        public const int EGL_BAD_DISPLAY = 0x3008;
        public const int EGL_BAD_CONFIG = 0x3005;
        public const int EGL_CONTEXT_LOST = 0x300E;

        [DllImport("libEGL.dll", EntryPoint = "eglGetDisplay")]
        public static extern IntPtr eglGetDisplay(IntPtr display_id);

        [DllImport("libEGL.dll", EntryPoint = "eglInitialize")]
        public static extern bool eglInitialize(IntPtr dpy, out int major, out int minor);

        [DllImport("libEGL.dll", EntryPoint = "eglChooseConfig")]
        public static extern bool eglChooseConfig(IntPtr dpy, int[] attrib_list, [Out] IntPtr[] configs, int config_size, out int num_config);

        [DllImport("libEGL.dll", EntryPoint = "eglCreateWindowSurface")]
        public static extern IntPtr eglCreateWindowSurface(IntPtr dpy, IntPtr config, IntPtr win, int[] attrib_list);

        [DllImport("libEGL.dll", EntryPoint = "eglCreatePbufferSurface")]
        public static extern IntPtr eglCreatePbufferSurface(IntPtr dpy, IntPtr config, int[] attrib_list);

        [DllImport("libEGL.dll", EntryPoint = "eglCreateContext")]
        public static extern IntPtr eglCreateContext(IntPtr dpy, IntPtr config, IntPtr share_context, int[] attrib_list);

        [DllImport("libEGL.dll", EntryPoint = "eglMakeCurrent")]
        public static extern bool eglMakeCurrent(IntPtr dpy, IntPtr draw, IntPtr read, IntPtr ctx);

        [DllImport("libEGL.dll", EntryPoint = "eglSwapBuffers")]
        public static extern bool eglSwapBuffers(IntPtr dpy, IntPtr surface);

        [DllImport("libEGL.dll", EntryPoint = "eglTerminate")]
        public static extern bool eglTerminate(IntPtr dpy);

        [DllImport("libEGL.dll", EntryPoint = "eglGetProcAddress")]
        public static extern IntPtr eglGetProcAddress(string procname);

        [DllImport("libEGL.dll", EntryPoint = "eglBindAPI")]
        public static extern bool eglBindAPI(uint api);

        [DllImport("libEGL.dll", EntryPoint = "eglQueryString")]
        public static extern IntPtr eglQueryString(IntPtr dpy, int name);

        [DllImport("libEGL.dll", EntryPoint = "eglGetError")]
        public static extern int eglGetError();

        [DllImport("libEGL.dll", EntryPoint = "eglDestroySurface")]
        public static extern bool eglDestroySurface(IntPtr dpy, IntPtr surface);

        [DllImport("libEGL.dll", EntryPoint = "eglDestroyContext")]
        public static extern bool eglDestroyContext(IntPtr dpy, IntPtr ctx);
    }
}
