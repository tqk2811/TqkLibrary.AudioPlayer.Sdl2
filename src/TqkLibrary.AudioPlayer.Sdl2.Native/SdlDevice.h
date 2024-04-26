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

	BOOL Init(const AVFrame* pFrame);

	SdlSourceQueueResult QueueAudio(AVFrame* pFrame);
	VOID Pause(INT32 flag);
	SDL_AudioStatus GetStatus();
	UINT32 GetQueuedAudioSize();

private:
	AVSampleFormat _outFormat{ AV_SAMPLE_FMT_NONE };
	AVChannelLayout _outChlayout{  };
	SDL_AudioDeviceID _deviceId{ 0 };
	SwrConvert* _swrConvert{ nullptr };
};
#endif
