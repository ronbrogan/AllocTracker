#include "Common.h"
#include "Logger.h"
#include "OS.h"
#include "CoreProfilerFactory.h"

extern "C" BOOL __stdcall DllMain(HINSTANCE hInstDll, DWORD reason, PVOID) {
	switch (reason) {
		case DLL_PROCESS_ATTACH:
			Logger::Info("Profiler DLL loaded into PID %d", OS::GetPid());
			break;

		case DLL_PROCESS_DETACH:
			Logger::Info("Profiler DLL unloaded from PID %d", OS::GetPid());
			Logger::Shutdown();
			break;
	}
	return TRUE;
}

class __declspec(uuid("768edc28-f285-4a4b-a5da-9a2de8fe4f92")) CoreProfiler;

extern "C" HRESULT __stdcall DllGetClassObject(REFCLSID rclsid, REFIID riid, void** ppv) {
	Logger::Debug(__FUNCTION__);

	if (rclsid == __uuidof(CoreProfiler)) {
		static CoreProfilerFactory factory;
		return factory.QueryInterface(riid, ppv);
	}
	return CLASS_E_CLASSNOTAVAILABLE;
}
