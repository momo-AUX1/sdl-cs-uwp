#include "UGL.h"

PFN_UGL_SDL_PollEvents     g_pfnUGL_SDL_PollEvents     = 0;
PFN_UGL_GetButton          g_pfnUGL_GetButton          = 0;
PFN_UGL_SDL_GL_GetContext  g_pfnUGL_SDL_GL_GetContext  = 0;
PFN_UGL_SDL_MainLoop       g_pfnUGL_SDL_MainLoop       = 0;
PFN_UGL_SDL_GamepadVibrate g_pfnUGL_SDL_GamepadVibrate = 0;

// -------------------------------------------------------------------------
// UGL_SetFunctionPointers: Called by the host to pass our managed delegates.
// -------------------------------------------------------------------------
__declspec(dllexport) void UGL_SetFunctionPointers(
    PFN_UGL_SDL_PollEvents pfnPollEvents,
    PFN_UGL_GetButton pfnGetButton,
    PFN_UGL_SDL_GL_GetContext pfnGetContext,
    PFN_UGL_SDL_MainLoop pfnMainLoop,
    PFN_UGL_SDL_GamepadVibrate pfnGamepadVibrate)
{
    g_pfnUGL_SDL_PollEvents    = pfnPollEvents;
    g_pfnUGL_GetButton         = pfnGetButton;
    g_pfnUGL_SDL_GL_GetContext = pfnGetContext;
    g_pfnUGL_SDL_MainLoop      = pfnMainLoop;
    g_pfnUGL_SDL_GamepadVibrate = pfnGamepadVibrate;
}

// -------------------------------------------------------------------------
// Stub implementations for functions that the DLL calls but which are
// actually implemented on the host.
// -------------------------------------------------------------------------
__declspec(dllexport) void UGL_SDL_Init(void) { }
__declspec(dllexport) void* UGL_SDL_CreateWindow(const char* title,
                                                   int x, int y,
                                                   int width, int height,
                                                   unsigned int flags)
{
    return 0;
}
__declspec(dllexport) void UGL_SDL_GL_CreateContext(void* window,
                                                      int glesMajor,
                                                      int glesMinor)
{ }
__declspec(dllexport) void* UGL_SDL_GetWindow(void) { return 0; }
__declspec(dllexport) void UGL_SDL_GL_SwapWindow(void* window) { }
__declspec(dllexport) void UGL_SDL_PollEvents(void)
{
    if (g_pfnUGL_SDL_PollEvents)
        g_pfnUGL_SDL_PollEvents();
}
__declspec(dllexport) int UGL_GetButton(int button)
{
    if (g_pfnUGL_GetButton)
        return g_pfnUGL_GetButton(button);
    return 0;
}
__declspec(dllexport) void* UGL_SDL_GL_GetContext(void)
{
    if (g_pfnUGL_SDL_GL_GetContext)
        return g_pfnUGL_SDL_GL_GetContext();
    return 0;
}
__declspec(dllexport) int UGL_SDL_MainLoop(void (*renderCallback)())
{
    if (g_pfnUGL_SDL_MainLoop)
        return g_pfnUGL_SDL_MainLoop(renderCallback);
    return 0;
}
__declspec(dllexport) void UGL_SDL_GamepadVibrate(double leftMotor, double rightMotor, double leftTrigger, double rightTrigger)
{
    if (g_pfnUGL_SDL_GamepadVibrate)
        g_pfnUGL_SDL_GamepadVibrate(leftMotor, rightMotor, leftTrigger, rightTrigger);
}
