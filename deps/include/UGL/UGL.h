#ifndef UGL_H
#define UGL_H

#ifdef __cplusplus
extern "C" {
#endif

// -------------------------------------------------------------------------
// Function pointer types for the UGL functions implemented on the managed side.
// -------------------------------------------------------------------------
typedef void (*PFN_UGL_SDL_PollEvents)();
typedef int  (*PFN_UGL_GetButton)(int button);
typedef void* (*PFN_UGL_SDL_GL_GetContext)();
typedef int  (*PFN_UGL_SDL_MainLoop)(void (*renderCallback)());

// Global function pointers that will be set by the host.
extern PFN_UGL_SDL_PollEvents     g_pfnUGL_SDL_PollEvents;
extern PFN_UGL_GetButton          g_pfnUGL_GetButton;
extern PFN_UGL_SDL_GL_GetContext  g_pfnUGL_SDL_GL_GetContext;
extern PFN_UGL_SDL_MainLoop       g_pfnUGL_SDL_MainLoop;

// -------------------------------------------------------------------------
// Exported function to let the host push its delegates into our module.
// -------------------------------------------------------------------------
__declspec(dllexport) void UGL_SetFunctionPointers(
    PFN_UGL_SDL_PollEvents pfnPollEvents,
    PFN_UGL_GetButton pfnGetButton,
    PFN_UGL_SDL_GL_GetContext pfnGetContext,
    PFN_UGL_SDL_MainLoop pfnMainLoop);

// -------------------------------------------------------------------------
// Declarations for the stub functions (defined only in UGL.cpp)
// -------------------------------------------------------------------------
__declspec(dllexport) void UGL_SDL_Init(void);
__declspec(dllexport) void* UGL_SDL_CreateWindow(const char* title,
                                                   int x, int y,
                                                   int width, int height,
                                                   unsigned int flags);
__declspec(dllexport) void UGL_SDL_GL_CreateContext(void* window,
                                                      int glesMajor,
                                                      int glesMinor);
__declspec(dllexport) void* UGL_SDL_GetWindow(void);
__declspec(dllexport) void UGL_SDL_GL_SwapWindow(void* window);
__declspec(dllexport) void UGL_SDL_PollEvents(void);
__declspec(dllexport) int UGL_GetButton(int button);
__declspec(dllexport) void* UGL_SDL_GL_GetContext(void);
__declspec(dllexport) int UGL_SDL_MainLoop(void (*renderCallback)());

// -------------------------------------------------------------------------
// UGL Window Flags and Button definitions
// -------------------------------------------------------------------------
#define UGL_WINDOW_OPENGL      0x00000002
#define UGL_WINDOW_SHOWN       0x00000004
#define UGL_WINDOW_FULLSCREEN  0x00000008

// managed enum equivalent from the host side:
#define UGL_BUTTON_A          1
#define UGL_BUTTON_B          2
#define UGL_BUTTON_X          3
#define UGL_BUTTON_Y          4
#define UGL_BUTTON_L1         5
#define UGL_BUTTON_R1         6
#define UGL_BUTTON_L2         7
#define UGL_BUTTON_R2         8
#define UGL_BUTTON_DPAD_UP    9
#define UGL_BUTTON_DPAD_DOWN  10
#define UGL_BUTTON_DPAD_LEFT  11
#define UGL_BUTTON_DPAD_RIGHT 12

#ifdef __cplusplus
}
#endif

#endif 
