using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;

namespace xbox_uwp_sdl2_starter
{
    public static class GLES
    {
        public const uint GL_VERTEX_SHADER = 0x8B31;
        public const uint GL_FRAGMENT_SHADER = 0x8B30;
        public const uint GL_COMPILE_STATUS = 0x8B81;
        public const uint GL_LINK_STATUS = 0x8B82;
        public const uint GL_ARRAY_BUFFER = 0x8892;
        public const uint GL_STATIC_DRAW = 0x88E4;
        public const uint GL_COLOR_BUFFER_BIT = 0x00004000;
        public const uint GL_TRIANGLES = 0x0004;
        public const uint GL_FLOAT = 0x1406;
        public const uint GL_TRUE = 1;
        public const uint GL_FALSE = 0;

        public const uint GL_DEPTH_BUFFER_BIT = 0x00000100;
        public const uint GL_DEPTH_TEST = 0x0B71;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate uint glCreateShaderDelegate(uint type);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void glShaderSourceDelegate(uint shader, int count, string[] stringPtr, int[] length);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void glCompileShaderDelegate(uint shader);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void glGetShaderivDelegate(uint shader, uint pname, out int param);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void glGetShaderInfoLogDelegate(uint shader, int maxLength, out int length, StringBuilder infoLog);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate uint glCreateProgramDelegate();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void glAttachShaderDelegate(uint program, uint shader);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void glLinkProgramDelegate(uint program);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void glGetProgramivDelegate(uint program, uint pname, out int param);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void glGetProgramInfoLogDelegate(uint program, int maxLength, out int length, StringBuilder infoLog);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void glUseProgramDelegate(uint program);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void glDeleteShaderDelegate(uint shader);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void glDeleteProgramDelegate(uint program);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void glGenVertexArraysDelegate(int n, out uint arrays);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void glBindVertexArrayDelegate(uint array);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void glGenBuffersDelegate(int n, out uint buffers);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void glBindBufferDelegate(uint target, uint buffer);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void glBufferDataDelegate(uint target, IntPtr size, float[] data, uint usage);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void glEnableVertexAttribArrayDelegate(uint index);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void glVertexAttribPointerDelegate(uint index, int size, uint type, bool normalized, int stride, IntPtr pointer);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void glDrawArraysDelegate(uint mode, int first, int count);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate uint glGetErrorDelegate();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void glClearColorDelegate(float red, float green, float blue, float alpha);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void glClearDelegate(uint mask);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void glViewportDelegate(int x, int y, int width, int height);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr glGetStringDelegate(uint name);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void glDeleteBuffersDelegate(int n, out uint buffers);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void glDeleteVertexArraysDelegate(int n, out uint arrays);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void glEnableDelegate(uint cap);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void glUniformMatrix4fvDelegate(int location, int count, bool transpose, float[] value);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int glGetUniformLocationDelegate(uint program, string name);

        public static glCreateShaderDelegate glCreateShader;
        public static glShaderSourceDelegate glShaderSource;
        public static glCompileShaderDelegate glCompileShader;
        public static glGetShaderivDelegate glGetShaderiv;
        public static glGetShaderInfoLogDelegate glGetShaderInfoLog;
        public static glCreateProgramDelegate glCreateProgram;
        public static glAttachShaderDelegate glAttachShader;
        public static glLinkProgramDelegate glLinkProgram;
        public static glGetProgramivDelegate glGetProgramiv;
        public static glGetProgramInfoLogDelegate glGetProgramInfoLog;
        public static glUseProgramDelegate glUseProgram;
        public static glDeleteShaderDelegate glDeleteShader;
        public static glDeleteProgramDelegate glDeleteProgram;
        public static glGenVertexArraysDelegate glGenVertexArrays;
        public static glBindVertexArrayDelegate glBindVertexArray;
        public static glGenBuffersDelegate glGenBuffers;
        public static glBindBufferDelegate glBindBuffer;
        public static glBufferDataDelegate glBufferData;
        public static glEnableVertexAttribArrayDelegate glEnableVertexAttribArray;
        public static glVertexAttribPointerDelegate glVertexAttribPointer;
        public static glDrawArraysDelegate glDrawArrays;
        public static glGetErrorDelegate glGetError;
        public static glClearColorDelegate glClearColor;
        public static glClearDelegate glClear;
        public static glViewportDelegate glViewport;
        public static glGetStringDelegate glGetString;
        public static glDeleteBuffersDelegate glDeleteBuffers;
        public static glDeleteVertexArraysDelegate glDeleteVertexArrays;

        public static glEnableDelegate glEnable;
        public static glUniformMatrix4fvDelegate glUniformMatrix4fv;
        public static glGetUniformLocationDelegate glGetUniformLocation;

        private static T LoadFunction<T>(string name) where T : Delegate
        {
            IntPtr proc = EGL.eglGetProcAddress(name);
            if (proc == IntPtr.Zero)
            {
                Debug.WriteLine($"Unable to load OpenGL ES function '{name}'.");
                throw new Exception($"Unable to load OpenGL ES function '{name}'.");
            }
            return Marshal.GetDelegateForFunctionPointer<T>(proc);
        }

        public static void Initialize()
        {
            glCreateShader = LoadFunction<glCreateShaderDelegate>("glCreateShader");
            glShaderSource = LoadFunction<glShaderSourceDelegate>("glShaderSource");
            glCompileShader = LoadFunction<glCompileShaderDelegate>("glCompileShader");
            glGetShaderiv = LoadFunction<glGetShaderivDelegate>("glGetShaderiv");
            glGetShaderInfoLog = LoadFunction<glGetShaderInfoLogDelegate>("glGetShaderInfoLog");
            glCreateProgram = LoadFunction<glCreateProgramDelegate>("glCreateProgram");
            glAttachShader = LoadFunction<glAttachShaderDelegate>("glAttachShader");
            glLinkProgram = LoadFunction<glLinkProgramDelegate>("glLinkProgram");
            glGetProgramiv = LoadFunction<glGetProgramivDelegate>("glGetProgramiv");
            glGetProgramInfoLog = LoadFunction<glGetProgramInfoLogDelegate>("glGetProgramInfoLog");
            glUseProgram = LoadFunction<glUseProgramDelegate>("glUseProgram");
            glDeleteShader = LoadFunction<glDeleteShaderDelegate>("glDeleteShader");
            glDeleteProgram = LoadFunction<glDeleteProgramDelegate>("glDeleteProgram");
            glGenVertexArrays = LoadFunction<glGenVertexArraysDelegate>("glGenVertexArrays");
            glBindVertexArray = LoadFunction<glBindVertexArrayDelegate>("glBindVertexArray");
            glGenBuffers = LoadFunction<glGenBuffersDelegate>("glGenBuffers");
            glBindBuffer = LoadFunction<glBindBufferDelegate>("glBindBuffer");
            glBufferData = LoadFunction<glBufferDataDelegate>("glBufferData");
            glEnableVertexAttribArray = LoadFunction<glEnableVertexAttribArrayDelegate>("glEnableVertexAttribArray");
            glVertexAttribPointer = LoadFunction<glVertexAttribPointerDelegate>("glVertexAttribPointer");
            glDrawArrays = LoadFunction<glDrawArraysDelegate>("glDrawArrays");
            glGetError = LoadFunction<glGetErrorDelegate>("glGetError");
            glClearColor = LoadFunction<glClearColorDelegate>("glClearColor");
            glClear = LoadFunction<glClearDelegate>("glClear");
            glViewport = LoadFunction<glViewportDelegate>("glViewport");
            glGetString = LoadFunction<glGetStringDelegate>("glGetString");
            glDeleteBuffers = LoadFunction<glDeleteBuffersDelegate>("glDeleteBuffers");
            glDeleteVertexArrays = LoadFunction<glDeleteVertexArraysDelegate>("glDeleteVertexArrays");
            glEnable = LoadFunction<glEnableDelegate>("glEnable");
            glUniformMatrix4fv = LoadFunction<glUniformMatrix4fvDelegate>("glUniformMatrix4fv");
            glGetUniformLocation = LoadFunction<glGetUniformLocationDelegate>("glGetUniformLocation");
        }
    }
}
