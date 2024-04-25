#include "pch.h"
#include "SdlDevice.h"


SdlDevice::SdlDevice() {

}
SdlDevice::~SdlDevice() {

}

BOOL SdlDevice::Init(const AVFrame* pFrame) {
	SDL_AudioSpec want, have;
	SDL_zero(want);
	SDL_zero(have);
	want.freq = pFrame->sample_rate;
	want.channels = pFrame->ch_layout.nb_channels;
	switch ((AVSampleFormat)pFrame->format)
	{
	case AVSampleFormat::AV_SAMPLE_FMT_U8:
		want.format = AUDIO_U8;
		break;
	case AVSampleFormat::AV_SAMPLE_FMT_S16:
		want.format = AUDIO_S16;
		break;
	case AVSampleFormat::AV_SAMPLE_FMT_S32:
		want.format = AUDIO_S32;
		break;
	case AVSampleFormat::AV_SAMPLE_FMT_FLT:
		want.format = AUDIO_F32;
		break;

	case AVSampleFormat::AV_SAMPLE_FMT_U8P:
	case AVSampleFormat::AV_SAMPLE_FMT_S16P:
	case AVSampleFormat::AV_SAMPLE_FMT_S32P:
	case AVSampleFormat::AV_SAMPLE_FMT_FLTP:
	{
		switch ((AVSampleFormat)pFrame->format)
		{
		case AVSampleFormat::AV_SAMPLE_FMT_U8P:
			_outFormat = AVSampleFormat::AV_SAMPLE_FMT_U8;
			want.format = AUDIO_U8;
			break;
		case AVSampleFormat::AV_SAMPLE_FMT_S16P:
			_outFormat = AVSampleFormat::AV_SAMPLE_FMT_S16;
			want.format = AUDIO_S16;
			break;
		case AVSampleFormat::AV_SAMPLE_FMT_S32P:
			_outFormat = AVSampleFormat::AV_SAMPLE_FMT_S32;
			want.format = AUDIO_S32;
			break;
		case AVSampleFormat::AV_SAMPLE_FMT_FLTP:
			_outFormat = AVSampleFormat::AV_SAMPLE_FMT_FLT;
			want.format = AUDIO_F32;
			break;
		}

		_outChlayout.nb_channels = pFrame->ch_layout.nb_channels;
		_outChlayout.order = pFrame->ch_layout.order;
		if (pFrame->ch_layout.order == AV_CHANNEL_ORDER_NATIVE)
			_outChlayout.u.mask = pFrame->ch_layout.u.mask;

		this->_swrConvert = new SwrConvert();
		if (!this->_swrConvert->Init(
			(AVChannelLayout*)&_outChlayout, _outFormat, pFrame->sample_rate,
			(AVChannelLayout*)&pFrame->ch_layout, (AVSampleFormat)pFrame->format, pFrame->sample_rate
		))
		{
			return FALSE;
		}
		break;
	}

	default:
		return FALSE;
	}
	this->_deviceId = SDL_OpenAudioDevice(NULL, 0, &want, &have, 0);
	SDL_PauseAudioDevice(this->_deviceId, 0);
	return this->_deviceId != 0;
}

SdlSourceQueueResult SdlDevice::QueueAudio(AVFrame* pFrame) {
	if (this->_deviceId == 0)
		return SdlSourceQueueResult::SdlSourceQueue_Failed;

	AVFrame* newFrame = av_frame_alloc();
	assert(newFrame);
	if (this->_swrConvert)
	{
		newFrame->ch_layout.nb_channels = pFrame->ch_layout.nb_channels;
		newFrame->ch_layout.order = pFrame->ch_layout.order;
		newFrame->ch_layout.opaque = nullptr;
		if (pFrame->ch_layout.order == AV_CHANNEL_ORDER_NATIVE)
			newFrame->ch_layout.u.mask = pFrame->ch_layout.u.mask;
		newFrame->format = this->_outFormat;
		newFrame->sample_rate = pFrame->sample_rate;
		if (!this->_swrConvert->Convert(pFrame, newFrame))
		{
			av_frame_free(&newFrame);
			return SdlSourceQueueResult::SdlSourceQueue_Failed;
		}
	}
	else
	{
		av_frame_copy_props(newFrame, pFrame);
		av_frame_ref(newFrame, pFrame);
	}

	int AudioBytes = av_samples_get_buffer_size(
		newFrame->linesize,
		newFrame->ch_layout.nb_channels,
		newFrame->nb_samples,
		(AVSampleFormat)newFrame->format,
		0
	);
	int queueResult = SDL_QueueAudio(this->_deviceId, newFrame->data[0], AudioBytes);
	av_frame_free(&newFrame);
	if (queueResult != 0)
	{
		SetLastError(queueResult);

#if _DEBUG
		auto msg = SDL_GetError();
#endif

		return SdlSourceQueueResult::SdlSourceQueue_Failed;
	}
	return SdlSourceQueueResult::SdlSourceQueue_Success;
}