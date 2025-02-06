using System;
using System.Text;
using System.Diagnostics;

namespace xbox_uwp_sdl2_starter
{
    public class HelloWorldSceneANGLE
    {
        private const string VertexShaderSrc = @"
            attribute vec2 aPosition;
            void main()
            {
                gl_Position = vec4(aPosition, 0.0, 1.0);
            }
        ";

        private const string FragmentShaderSrc = @"
            precision mediump float;
            void main()
            {
                gl_FragColor = vec4(1.0, 0.5, 0.2, 1.0);
            }
        ";

        private uint _shaderProgram;
        private uint _vao;
        private uint _vbo;

        private float[] _triangleVertices = new float[]
        {
            //  X,    Y
             0.0f,   0.5f,   // Top
            -0.5f,  -0.5f,   // Left
             0.5f,  -0.5f,   // Right
        };

        public void Initialize()
        {
            _shaderProgram = CreateShaderProgram(VertexShaderSrc, FragmentShaderSrc);

            GLES.glGenVertexArrays(1, out _vao);
            GLES.glBindVertexArray(_vao);

            GLES.glGenBuffers(1, out _vbo);
            GLES.glBindBuffer(GLES.GL_ARRAY_BUFFER, _vbo);
            var sizeInBytes = (IntPtr)(sizeof(float) * _triangleVertices.Length);
            GLES.glBufferData(GLES.GL_ARRAY_BUFFER, sizeInBytes, _triangleVertices, GLES.GL_STATIC_DRAW);

            uint location = 0;
            GLES.glEnableVertexAttribArray(location);
            GLES.glVertexAttribPointer(location, 2, GLES.GL_FLOAT, false, 2 * sizeof(float), IntPtr.Zero);

            GLES.glBindBuffer(GLES.GL_ARRAY_BUFFER, 0);
            GLES.glBindVertexArray(0);
        }

        public void Render()
        {
            GLES.glClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GLES.glClear(GLES.GL_COLOR_BUFFER_BIT);

            GLES.glUseProgram(_shaderProgram);

            GLES.glBindVertexArray(_vao);
            GLES.glDrawArrays(GLES.GL_TRIANGLES, 0, 3);

            GLES.glBindVertexArray(0);
        }

        public void Cleanup()
        {
            if (_vbo != 0)
            {
                GLES.glDeleteBuffers(1, out _vbo);
                _vbo = 0;
            }
            if (_vao != 0)
            {
                GLES.glDeleteVertexArrays(1, out _vao);
                _vao = 0;
            }
            if (_shaderProgram != 0)
            {
                GLES.glDeleteProgram(_shaderProgram);
                _shaderProgram = 0;
            }
        }

        private uint CreateShaderProgram(string vertexSrc, string fragmentSrc)
        {
            uint vertexShader = GLES.glCreateShader(GLES.GL_VERTEX_SHADER);
            GLES.glShaderSource(vertexShader, 1, new[] { vertexSrc }, null);
            GLES.glCompileShader(vertexShader);
            CheckCompileErrors(vertexShader, "VERTEX");

            uint fragmentShader = GLES.glCreateShader(GLES.GL_FRAGMENT_SHADER);
            GLES.glShaderSource(fragmentShader, 1, new[] { fragmentSrc }, null);
            GLES.glCompileShader(fragmentShader);
            CheckCompileErrors(fragmentShader, "FRAGMENT");

            uint program = GLES.glCreateProgram();
            GLES.glAttachShader(program, vertexShader);
            GLES.glAttachShader(program, fragmentShader);
            GLES.glLinkProgram(program);
            CheckLinkErrors(program);

            GLES.glDeleteShader(vertexShader);
            GLES.glDeleteShader(fragmentShader);

            return program;
        }

        private void CheckCompileErrors(uint shader, string type)
        {
            GLES.glGetShaderiv(shader, GLES.GL_COMPILE_STATUS, out int success);
            if (success == 0)
            {
                StringBuilder infoLog = new StringBuilder(512);
                GLES.glGetShaderInfoLog(shader, infoLog.Capacity, out _, infoLog);
                Debug.WriteLine($"ERROR::SHADER_COMPILATION_ERROR of type: {type}\n{infoLog}\n");
            }
        }

        private void CheckLinkErrors(uint program)
        {
            GLES.glGetProgramiv(program, GLES.GL_LINK_STATUS, out int success);
            if (success == 0)
            {
                StringBuilder infoLog = new StringBuilder(512);
                GLES.glGetProgramInfoLog(program, infoLog.Capacity, out _, infoLog);
                Debug.WriteLine($"ERROR::PROGRAM_LINKING_ERROR\n{infoLog}\n");
            }
        }
    }
}
