#ifndef _H_TqkLibraryAudioPlayerSdl2Native_H_
#define _H_TqkLibraryAudioPlayerSdl2Native_H_
TQKLIBRARYAUDIOPLAYERSDL2NATIVE__EXPORTS SdlDevice* SdlDevice_Alloc(int freq, Uint8 channels, SDL_AudioFormat format);
TQKLIBRARYAUDIOPLAYERSDL2NATIVE__EXPORTS void SdlDevice_Free(SdlDevice** ppSdlDevice);
TQKLIBRARYAUDIOPLAYERSDL2NATIVE__EXPORTS SdlSourceQueueResult SdlDevice_QueueAudio(SdlDevice* pSdlDevice, const Uint8* data, Uint32 len);
TQKLIBRARYAUDIOPLAYERSDL2NATIVE__EXPORTS VOID SdlDevice_Pause(SdlDevice* pSdlDevice, INT32 flag);
TQKLIBRARYAUDIOPLAYERSDL2NATIVE__EXPORTS SDL_AudioStatus SdlDevice_GetStatus(SdlDevice* pSdlDevice);
TQKLIBRARYAUDIOPLAYERSDL2NATIVE__EXPORTS UINT32 SdlDevice_GetQueuedAudioSize(SdlDevice* pSdlDevice);
TQKLIBRARYAUDIOPLAYERSDL2NATIVE__EXPORTS VOID SdlDevice_ClearQueuedAudio(SdlDevice* pSdlDevice);


#endif // !TqkLibraryAudioPlayerSdl2Native_H
