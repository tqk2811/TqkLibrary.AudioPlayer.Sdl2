namespace TqkLibrary.AudioPlayer.Sdl2.Enums
{
    /// <summary>
    /// SDL Audio Format values matching SDL_AudioFormat (Uint16)
    /// </summary>
    public enum SdlAudioFormat : ushort
    {
        AUDIO_U8 = 0x0008,
        AUDIO_S8 = 0x8008,
        AUDIO_U16LSB = 0x0010,
        AUDIO_S16LSB = 0x8010,
        AUDIO_U16MSB = 0x1010,
        AUDIO_S16MSB = 0x9010,
        AUDIO_S32LSB = 0x8020,
        AUDIO_S32MSB = 0x9020,
        AUDIO_F32LSB = 0x8120,
        AUDIO_F32MSB = 0x9120,

        AUDIO_U16 = AUDIO_U16LSB,
        AUDIO_S16 = AUDIO_S16LSB,
        AUDIO_S32 = AUDIO_S32LSB,
        AUDIO_F32 = AUDIO_F32LSB,
    }
}
