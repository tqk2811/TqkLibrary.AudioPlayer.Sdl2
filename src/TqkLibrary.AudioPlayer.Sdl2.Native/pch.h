#ifndef PCH_H
#define PCH_H

#ifdef TQKLIBRARYAUDIOPLAYERSDL2NATIVE_EXPORTS
#define TQKLIBRARYAUDIOPLAYERSDL2NATIVE__EXPORTS extern "C" __declspec( dllexport )
#else
#define TQKLIBRARYAUDIOPLAYERSDL2NATIVE__EXPORTS extern "C" __declspec( dllimport )
#endif

#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include <assert.h>
#include <SDL.h>
#include "libav.h"

typedef class SwrConvert SwrConvert;
typedef enum SdlSourceQueueResult SdlSourceQueueResult;
#include "SdlDevice.h"
#include "SwrConvert.h"

#include "Exports.h"


#endif //PCH_H
