#ifndef _H_SdlDevice_H_
#define _H_SdlDevice_H_
enum SdlSourceQueueResult : INT32
{
	SdlSourceQueue_Failed = -1,
	SdlSourceQueue_Success = 0,
	SdlSourceQueue_QueueFailed = 1,
};

class SdlDevice
{
public:
	SdlDevice();
	~SdlDevice();

	BOOL Init(const char* deviceName, int freq, Uint8 channels, SDL_AudioFormat format);

	SdlSourceQueueResult QueueAudio(const Uint8* data, Uint32 len);
	VOID Pause(INT32 flag);
	SDL_AudioStatus GetStatus();
	UINT32 GetQueuedAudioSize();
	VOID ClearQueuedAudio();

	FLOAT GetVolume();
	VOID SetVolume(FLOAT volume);

private:
	SDL_AudioDeviceID _deviceId{ 0 };
	FLOAT _volume{ 1.0f };
	SDL_AudioFormat _format{ 0 };
};
#endif
