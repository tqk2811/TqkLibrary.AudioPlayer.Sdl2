using TqkLibrary.AudioPlayer.Sdl2;

// Generate a simple sine wave tone for testing
int sampleRate = 44100;
byte channels = 1;
SdlAudioFormat format = SdlAudioFormat.AUDIO_S16;
int durationSeconds = 2;
double frequency = 440.0; // A4 note

int totalSamples = sampleRate * durationSeconds;
byte[] audioData = new byte[totalSamples * channels * 2]; // 2 bytes per sample (S16)

for (int i = 0; i < totalSamples; i++)
{
    double t = (double)i / sampleRate;
    short sample = (short)(short.MaxValue * 0.5 * Math.Sin(2.0 * Math.PI * frequency * t));
    int offset = i * channels * 2;
    audioData[offset] = (byte)(sample & 0xFF);
    audioData[offset + 1] = (byte)((sample >> 8) & 0xFF);
}

using SdlDevice sdlDevice = new SdlDevice(sampleRate, channels, format);
Console.WriteLine("Playing 440Hz sine wave...");

// Queue audio in chunks
int chunkSize = sampleRate * 2; // 1 second chunks
for (int offset = 0; offset < audioData.Length; offset += chunkSize)
{
    int len = Math.Min(chunkSize, audioData.Length - offset);
    byte[] chunk = new byte[len];
    Array.Copy(audioData, offset, chunk, 0, len);

    SdlSourceQueueResult result = sdlDevice.QueueAudio(chunk);
    if (result != SdlSourceQueueResult.Success)
    {
        Console.WriteLine($"QueueAudio failed: {result}");
        return;
    }
}

await sdlDevice.WaitUntilPlayCompleteAsync();
Console.WriteLine("Playback complete.");