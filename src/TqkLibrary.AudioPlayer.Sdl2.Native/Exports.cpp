#include "pch.h"
#include "Exports.h"
SdlDevice* SdlDevice_Alloc(const AVFrame* pFrame)
{
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
	if (!pSdlDevice)
		return SdlSourceQueueResult::SdlSourceQueue_Failed;
	return pSdlDevice->QueueAudio(pFrame);
}