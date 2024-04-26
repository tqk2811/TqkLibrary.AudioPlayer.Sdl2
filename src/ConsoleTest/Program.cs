using TqkLibrary.AudioPlayer.FFmpegAudioReader;
using TqkLibrary.AudioPlayer.Sdl2;

string filePath = ".\\test-tube-194556.mp3";
SdlDevice? sdlDevice = null;
int count = 0;
try
{
    while (true)
    {
        using AudioSource audioSource = new AudioSource(filePath);
        using AudioSource.AVFrame aVFrame = new AudioSource.AVFrame();
        if (!audioSource.ReadFrame(aVFrame))
        {
            throw new ApplicationException();
        }
        if (sdlDevice is null)
            sdlDevice = new SdlDevice(aVFrame.Handle);

        Console.WriteLine($"Start {count}");
        do
        {
            while (true)
            {
                SdlSourceQueueResult result = sdlDevice.QueueAudio(aVFrame.Handle);
                if (result == SdlSourceQueueResult.Success)
                    break;//read next
                else if (result == SdlSourceQueueResult.Failed)
                    return;
                await Task.Delay(100);//requeue
            }
            while (sdlDevice.GetQueuedAudioSize() > 320 * 1024)
            {
                await Task.Delay(10);
            }
        } while (audioSource.ReadFrame(aVFrame));

        await sdlDevice.WaitUntilPlayCompleteAsync();
        Console.WriteLine($"End {count++}");
    }
}
finally
{
    sdlDevice?.Dispose();
}