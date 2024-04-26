#include "pch.h"
#include "Exports.h"
SdlDevice* SdlDevice_Alloc(const AVFrame* pFrame)
{
	SetLastError(0);
	SdlDevice* pdevice = new SdlDevice();
	if (pdevice->Init(pFrame))
	{
		return pdevice;
	}
	else
	{
		return nullptr;
	}
}
void SdlDevice_Free(SdlDevice** ppSdlDevice)
{
	if (ppSdlDevice)
	{
		SdlDevice* pdevice = *ppSdlDevice;
		if (pdevice)
		{
			delete pdevice;
			*ppSdlDevice = nullptr;
		}
	}
}
SdlSourceQueueResult SdlDevice_QueueAudio(SdlDevice* pSdlDevice, AVFrame* pFrame)
{
	SetLastError(0);
	if (!pSdlDevice)
		return SdlSourceQueueResult::SdlSourceQueue_Failed;
	return pSdlDevice->QueueAudio(pFrame);
}

VOID SdlDevice_Pause(SdlDevice* pSdlDevice, INT32 flag)
{
	SetLastError(0);
	if (pSdlDevice)
	{
		pSdlDevice->Pause(flag);
	}
}
SDL_AudioStatus SdlDevice_GetStatus(SdlDevice* pSdlDevice)
{
	SetLastError(0);
	if (pSdlDevice)
	{
		return pSdlDevice->GetStatus();
	}
	return (SDL_AudioStatus)-1;
}
UINT32 SdlDevice_GetQueuedAudioSize(SdlDevice* pSdlDevice)
{
	SetLastError(0);
	if (pSdlDevice)
	{
		return pSdlDevice->GetQueuedAudioSize();
	}
	return 0;
}
VOID SdlDevice_ClearQueuedAudio(SdlDevice* pSdlDevice)
{
	SetLastError(0);
	if (pSdlDevice)
	{
		pSdlDevice->ClearQueuedAudio();
	}
}