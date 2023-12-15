#pragma once

namespace Timuse
{
	enum TimuseErr : long
	{
		Success = 0,
		AlreadyLaunched = -1,
		FailedToInitializeUIAutomation = -2,
		FailedToGetRootElement = -3,
		FailedToLoadLibrary = -4,
		FailedToGetProcAddress = -5,
		ComInitializeFailed = -6,
		OutOfMemory = -7,
		CoCreateInstanceFailed = -8,
	};

	inline bool IsSuccess(const TimuseErr& err)
	{
		return err == TimuseErr::Success;
	}
}