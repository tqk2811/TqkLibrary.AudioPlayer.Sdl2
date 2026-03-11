#include "pch.h"
#include "SdlDevice.h"


SdlDevice::SdlDevice() {

}
SdlDevice::~SdlDevice() {
	SDL_CloseAudioDevice(this->_deviceId);
	this->_deviceId = 0;
}

BOOL SdlDevice::Init(const char* deviceName, int freq, Uint8 channels, SDL_AudioFormat format) {
	SDL_AudioSpec want, have;
	SDL_zero(want);
	SDL_zero(have);
	want.freq = freq;
	want.channels = channels;
	want.format = format;
	this->_deviceId = SDL_OpenAudioDevice(deviceName, 0, &want, &have, 0);
	SDL_PauseAudioDevice(this->_deviceId, 0);
	return this->_deviceId != 0;
}

SdlSourceQueueResult SdlDevice::QueueAudio(const Uint8* data, Uint32 len) {
	if (this->_deviceId == 0)
		return SdlSourceQueueResult::SdlSourceQueue_Failed;

	int queueResult = SDL_QueueAudio(this->_deviceId, data, len);
	if (queueResult != 0)
	{
		SetLastError(queueResult);

#if _DEBUG
		auto msg = SDL_GetError();
#endif

		return SdlSourceQueueResult::SdlSourceQueue_QueueFailed;
	}
	return SdlSourceQueueResult::SdlSourceQueue_Success;
}


VOID SdlDevice::Pause(INT32 flag)
{
	SDL_PauseAudioDevice(this->_deviceId, flag);
}

SDL_AudioStatus SdlDevice::GetStatus()
{
	return SDL_GetAudioDeviceStatus(this->_deviceId);
}

UINT32 SdlDevice::GetQueuedAudioSize() {
	return SDL_GetQueuedAudioSize(this->_deviceId);
}

VOID SdlDevice::ClearQueuedAudio()
{
	SDL_ClearQueuedAudio(this->_deviceId);
}