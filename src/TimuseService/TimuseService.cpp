#include "MediaListener.h"
#include "AppRecorder.h"
#include "BinaryHelper.h"

using namespace Timuse;

TimuseErr InitializeUIAutomation(IUIAutomation** ppAutomation);
TimuseErr InitializeApplicationListener(void);
TimuseErr PreventMultiInstance();
void CreateConsoleWhileDebug(void);

std::shared_ptr<AppRecorder> spAppRecorder = nullptr;

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

    winrt::init_apartment();

    // MediaListener mediaListener;
    spAppRecorder = std::make_shared<AppRecorder>();    
    
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

void HandleFocusChangedEvent(BSTR lpName, BSTR lpPath)
{
    if (!lpName || !lpPath) return;
    spAppRecorder->Switch(lpName, lpPath);
}

TimuseErr InitializeApplicationListener()
{
    HRESULT hr = 0;

    IUIAutomation* pUIAutomation;
    hr = InitializeUIAutomation(&pUIAutomation);
    if (FAILED(hr)) return TimuseErr::FailedToInitializeUIAutomation;

    IUIAutomationElement* pUIElementRoot;

    hr = pUIAutomation->GetRootElement(&pUIElementRoot);
    if (FAILED(hr)) return TimuseErr::FailedToGetRootElement;

    UIAutomationFocusChangedEventHandler* pFocusHandler = new UIAutomationFocusChangedEventHandler(HandleFocusChangedEvent);

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
    hMutex = OpenMutex(MUTEX_ALL_ACCESS, FALSE, TEXT("MUTEXOFTIMEUSESERVICE"));
    if (NULL == hMutex) hMutex = CreateMutex(0, FALSE, TEXT("MUTEXOFTIMEUSESERVICE"));
    else return TimuseErr::AlreadyLaunched;
    return TimuseErr::Success;
}

void CreateConsoleWhileDebug()
{
    int argCnt;
    LPWSTR* argv = CommandLineToArgvW(GetCommandLine(), &argCnt);
    if (argv != NULL && argCnt >= 2 && wcscmp(argv[1], TEXT("debug")) == 0)
    {
        AllocConsole();
        FILE* stream;
        freopen_s(&stream, "CON", "r", stdin);
        freopen_s(&stream, "CON", "w", stdout);
        SetConsoleTitle(TEXT("Timuse Service Console"));
        std::wcout.imbue(std::locale(""));
    }
}
