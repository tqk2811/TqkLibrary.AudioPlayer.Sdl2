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
	this->_format = format;
	this->_deviceId = SDL_OpenAudioDevice(deviceName, 0, &want, &have, 0);
	SDL_PauseAudioDevice(this->_deviceId, 0);
	return this->_deviceId != 0;
}

SdlSourceQueueResult SdlDevice::QueueAudio(const Uint8* data, Uint32 len, FLOAT volume) {
	if (this->_deviceId == 0)
		return SdlSourceQueueResult::SdlSourceQueue_Failed;

	if (volume != 1.0f) {
		Uint8* mixed = (Uint8*)SDL_malloc(len);
		if (mixed) {
			SDL_memset(mixed, this->_format == AUDIO_U8 ? 128 : 0, len);
			int sdl_volume = (int)(volume * SDL_MIX_MAXVOLUME);
			if (sdl_volume < 0) sdl_volume = 0;
			if (sdl_volume > SDL_MIX_MAXVOLUME) sdl_volume = SDL_MIX_MAXVOLUME;

			SDL_MixAudioFormat(mixed, data, this->_format, len, sdl_volume);
			int queueResult = SDL_QueueAudio(this->_deviceId, mixed, len);
			SDL_free(mixed);

			if (queueResult != 0) {
				SetLastError(queueResult);
#if _DEBUG
				auto msg = SDL_GetError();
#endif
				return SdlSourceQueueResult::SdlSourceQueue_QueueFailed;
			}
			return SdlSourceQueueResult::SdlSourceQueue_Success;
		}
	}

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

UINT32 SdlDevice::GetQueuedAudioSize() 
{
	return SDL_GetQueuedAudioSize(this->_deviceId);
}

VOID SdlDevice::ClearQueuedAudio()
{
	SDL_ClearQueuedAudio(this->_deviceId);
}