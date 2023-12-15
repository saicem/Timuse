#pragma once
#include <winrt/windows.media.control.h>
#include <winrt/windows.foundation.h>

class MediaListener
{
public:
	MediaListener() 
	{
		Initialize().get();
	}
	~MediaListener() = default;
	winrt::Windows::Foundation::IAsyncAction Initialize();

private:
	winrt::Windows::Media::Control::GlobalSystemMediaTransportControlsSessionManager _sessionManager = nullptr;
};