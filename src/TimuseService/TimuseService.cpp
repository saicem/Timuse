using namespace Timuse;

TimuseErr InitializeUIAutomation(IUIAutomation** ppAutomation);
TimuseErr InitializeApplicationListener(void);
TimuseErr PreventMultiInstance();
void CreateConsoleWhileDebug(void);

int WINAPI WinMain(_In_ HINSTANCE hInstance, _In_opt_ HINSTANCE, _In_ LPSTR lpCmdLine, _In_ int nShowCmd)
{
    CreateConsoleWhileDebug();

    TimuseErr errCode = { };
    errCode = PreventMultiInstance();
    if (!IsSuccess(errCode))
    {
        std::cout << "timuse service already launched." << std::endl;
        return errCode;
    }

    errCode = InitializeApplicationListener();
    if (!IsSuccess(errCode)) return errCode;

    MSG msg = { };
    while (GetMessage(&msg, NULL, 0, 0))
    {
        TranslateMessage(&msg);
        DispatchMessage(&msg);
    }
    return 0;
}

TimuseErr InitializeApplicationListener()
{
    HRESULT hr;
    hr = CoInitializeEx(NULL, COINIT_APARTMENTTHREADED);
    if (FAILED(hr)) return TimuseErr::ComInitializeFailed;

    IUIAutomation* pUIAutomation;
    hr = InitializeUIAutomation(&pUIAutomation);
    if (FAILED(hr)) return TimuseErr::FailedToInitializeUIAutomation;

    IUIAutomationElement* pUIElementRoot;

    hr = pUIAutomation->GetRootElement(&pUIElementRoot);
    if (FAILED(hr)) return TimuseErr::FailedToGetRootElement;

    HMODULE hLibService = LoadLibrary(L"TimuseService.Lib.dll");
    if (hLibService == NULL) return TimuseErr::FailedToLoadLibrary;
    auto onSwitch = (FOREGROUNDAPPLICATIONSWITCHEDPROC)GetProcAddress(hLibService, "OnSwitch");
    auto initService = GetProcAddress(hLibService, "InitService");
    if (!initService || !onSwitch) return TimuseErr::FailedToGetProcAddress;

    initService();

    UIAutomationFocusChangedEventHandler* pFocusHandler = new UIAutomationFocusChangedEventHandler(onSwitch);
    if (!pFocusHandler)
    {
        return TimuseErr::OutOfMemory;
    }
    pUIAutomation->AddFocusChangedEventHandler(NULL, pFocusHandler);

    return TimuseErr::Success;
}

TimuseErr InitializeUIAutomation(IUIAutomation** ppAutomation)
{
    HRESULT hr = CoCreateInstance(CLSID_CUIAutomation, NULL,
        CLSCTX_INPROC_SERVER, IID_IUIAutomation,
        reinterpret_cast<void**>(ppAutomation));

    return FAILED(hr) ? TimuseErr::CoCreateInstanceFailed : TimuseErr::Success;
}

TimuseErr PreventMultiInstance()
{
    HANDLE hMutex = NULL;
    hMutex = OpenMutex(MUTEX_ALL_ACCESS, FALSE, L"MUTEXOFTIMEUSESERVICE");
    if (NULL == hMutex) hMutex = CreateMutex(0, FALSE, L"MUTEXOFTIMEUSESERVICE");
    else return TimuseErr::AlreadyLaunched;
    return TimuseErr::Success;
}

void CreateConsoleWhileDebug()
{
    int argCnt;
    LPWSTR* argv = CommandLineToArgvW(GetCommandLine(), &argCnt);
    if (argv != NULL && argCnt >= 2 && wcscmp(argv[1], L"debug") == 0)
    {
        AllocConsole();
        FILE* stream;
        freopen_s(&stream, "CON", "r", stdin);
        freopen_s(&stream, "CON", "w", stdout);
        SetConsoleTitle(L"Timuse Service Console");
    }
}
