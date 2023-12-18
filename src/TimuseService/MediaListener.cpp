#include "MediaListener.h"

using namespace winrt::Windows::Media::Control;

winrt::Windows::Foundation::IAsyncAction MediaListener::Initialize()
{
    auto sessionManager = co_await GlobalSystemMediaTransportControlsSessionManager::RequestAsync();
    this->_sessionManager = sessionManager;

    sessionManager.SessionsChanged(
        [](GlobalSystemMediaTransportControlsSessionManager manager, SessionsChangedEventArgs args)
        {
            auto sessions = manager.GetSessions();
            for (auto session : sessions)
            {
                auto props = session.TryGetMediaPropertiesAsync().get();
                auto title = props.Title();
                auto artist = props.Artist();

                std::wcout << title.c_str() << " - " << artist.c_str() << std::endl;
            }
        });
}