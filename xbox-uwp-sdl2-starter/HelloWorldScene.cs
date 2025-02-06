using System;
using System.Diagnostics;
using System.Text;

namespace xbox_uwp_sdl2_starter
{
    class HelloWorldScene
    {
        private uint shaderProgram;
        private uint VAO;
        private uint VBO;

        public void Initialize()
        {
            GL.Initialize();

            string vertexShaderSource = @"
                #version 430 core
                layout(location = 0) in vec3 aPos;
                void main()
                {
                    gl_Position = vec4(aPos, 1.0);
                }
            ";

            string fragmentShaderSource = @"
                 #version 430 core
                out vec4 FragColor;
                void main()
                {
                    FragColor = vec4(0.2, 0.1, 1.0, 1.0);
                }
            ";

            uint vertexShader = CompileShader(GL.GL_VERTEX_SHADER, vertexShaderSource);
            uint fragmentShader = CompileShader(GL.GL_FRAGMENT_SHADER, fragmentShaderSource);
            shaderProgram = LinkProgram(vertexShader, fragmentShader);

            GL.glDeleteShader(vertexShader);
            GL.glDeleteShader(fragmentShader);

            float[] vertices = {  
                 0.0f,  0.5f, 0.0f,  
                 0.5f, -0.5f, 0.0f,  
                -0.5f, -0.5f, 0.0f   
            };

            GL.glGenVertexArrays(1, out VAO);
            GL.glBindVertexArray(VAO);

            GL.glGenBuffers(1, out VBO);
            GL.glBindBuffer(GL.GL_ARRAY_BUFFER, VBO);
            GL.glBufferData(GL.GL_ARRAY_BUFFER, (IntPtr)(vertices.Length * sizeof(float)), vertices, GL.GL_STATIC_DRAW);

            GL.glVertexAttribPointer(0, 3, GL.GL_FLOAT, false, 3 * sizeof(float), IntPtr.Zero);
            GL.glEnableVertexAttribArray(0);

            GL.glBindBuffer(GL.GL_ARRAY_BUFFER, 0);
            GL.glBindVertexArray(0);
        }

        public void Render()
        {
            GL.glClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.glClear(GL.GL_COLOR_BUFFER_BIT);

            GL.glUseProgram(shaderProgram);

            GL.glBindVertexArray(VAO);

            GL.glDrawArrays(GL.GL_TRIANGLES, 0, 3);

            GL.glBindVertexArray(0);
        }

        public void Cleanup()
        {
            GL.glDeleteProgram(shaderProgram);
            GL.glDeleteBuffers(1, out VBO);
            GL.glDeleteVertexArrays(1, out VAO);
        }

        private uint CompileShader(uint type, string source)
        {
            uint shader = GL.glCreateShader(type);
            string[] sources = new string[] { source };
            int[] lengths = new int[] { source.Length };
            GL.glShaderSource(shader, sources.Length, sources, lengths);
            GL.glCompileShader(shader);

            GL.glGetShaderiv(shader, GL.GL_COMPILE_STATUS, out int success);
            if (success == 0)
            {
                StringBuilder infoLog = new StringBuilder(512);
                GL.glGetShaderInfoLog(shader, 512, out int logLength, infoLog);
                Debug.WriteLine($"ERROR::SHADER::COMPILATION_FAILED\n{infoLog}");
            }

            return shader;
        }

        private uint LinkProgram(uint vertexShader, uint fragmentShader)
        {
            uint program = GL.glCreateProgram();
            GL.glAttachShader(program, vertexShader);
            GL.glAttachShader(program, fragmentShader);
            GL.glLinkProgram(program);

            GL.glGetProgramiv(program, GL.GL_LINK_STATUS, out int success);
            if (success == 0)
            {
                StringBuilder infoLog = new StringBuilder(512);
                GL.glGetProgramInfoLog(program, 512, out int logLength, infoLog);
                Debug.WriteLine($"ERROR::PROGRAM::LINKING_FAILED\n{infoLog}");
            }

            return program;
        }
    }
}
