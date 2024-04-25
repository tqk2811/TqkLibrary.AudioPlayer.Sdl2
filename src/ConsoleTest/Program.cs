using TqkLibrary.AudioPlayer.FFmpegAudioReader;
using TqkLibrary.AudioPlayer.Sdl2;

string filePath = ".\\test-tube-194556.mp3";
while (true)
{
    using AudioSource audioSource = new AudioSource(filePath);
    using AudioSource.AVFrame aVFrame = new AudioSource.AVFrame();
    if (!audioSource.ReadFrame(aVFrame))
    {
        throw new ApplicationException();
    }
    using SdlDevice sdlDevice = new SdlDevice(aVFrame.Handle);

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
    } while (audioSource.ReadFrame(aVFrame));
    Console.ReadLine();
}
