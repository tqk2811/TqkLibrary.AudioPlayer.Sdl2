#ifndef _H_TqkLibraryAudioPlayerSdl2Native_H_
#define _H_TqkLibraryAudioPlayerSdl2Native_H_
TQKLIBRARYAUDIOPLAYERSDL2NATIVE__EXPORTS SdlDevice* SdlDevice_Alloc(const AVFrame* pFrame);
TQKLIBRARYAUDIOPLAYERSDL2NATIVE__EXPORTS void SdlDevice_Free(SdlDevice** ppSdlDevice);
TQKLIBRARYAUDIOPLAYERSDL2NATIVE__EXPORTS SdlSourceQueueResult SdlDevice_QueueAudio(SdlDevice* pSdlDevice, AVFrame* pFrame);



#endif // !ScrcpyNativeExports_H
