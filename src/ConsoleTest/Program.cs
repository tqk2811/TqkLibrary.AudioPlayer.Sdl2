using TqkLibrary.AudioPlayer.Sdl2;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TqkLibrary.AudioPlayer.Sdl2.Enums;

string filePath = "01 Rainbow.mp3";
if (!File.Exists(filePath))
{
    filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "01 Rainbow.mp3");
}
#if NETFRAMEWORK
string ffmpegPath = AppDomain.CurrentDomain.BaseDirectory;
#else
string ffmpegPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "runtimes", Environment.Is64BitProcess ? "win-x64" : "win-x86", "native");
#endif
// If tools package is used, it might be in root on net462 or specific runtimes folder on net8.0
string ffmpegExe = Path.Combine(ffmpegPath, "ffmpeg.exe");

filePath = Path.GetFullPath(filePath);
if (!File.Exists(filePath))
{
    Console.Error.WriteLine($"Error: File not found: {filePath}");
    return;
}

Console.WriteLine($"Using ffmpeg from: {ffmpegExe}");
Console.WriteLine($"Opening file: {filePath}");

// We'll force ffmpeg to output specific PCM format
byte channels = 2;
int sampleRate = 48000;
int bitsPerSample = 16;

Console.WriteLine($"Target Info: {channels} channels, {sampleRate}Hz (forced via ffmpeg)");

var devices = SdlDevice.GetAudioDeviceNames().ToArray();
string? deviceSelected = null;
for (int i = 0; i < devices.Length; i++)
{
    Console.WriteLine($"\t{i}: {devices[i]}");
}
Console.Write("SelectDevice Index:");
if (int.TryParse(Console.ReadLine()?.Trim(), out int result))
{
    deviceSelected = devices.Skip(result).FirstOrDefault();
}

using SdlDevice device = new SdlDevice(sampleRate, channels, SdlAudioFormat.AUDIO_S16, deviceSelected);

// SdlDevice_Alloc (constructor) already initialzes and starts (SDL_PauseAudioDevice 0)
// But we can call Pause(0) to be sure
device.Pause(0);

Console.WriteLine("Playing... Press 'Q' to quit.");

CancellationTokenSource cts = new CancellationTokenSource();

// Use a direct approach with Process to stream data block by block.
var processStartInfo = new ProcessStartInfo
{
    FileName = ffmpegExe,
    Arguments = $"-i \"{filePath}\" -f s16le -acodec pcm_s16le -ar {sampleRate} -ac {channels} pipe:1",
    RedirectStandardOutput = true,
    RedirectStandardError = true,
    UseShellExecute = false,
    CreateNoWindow = true
};

using var process = Process.Start(processStartInfo);
if (process == null) throw new ApplicationException("Failed to start ffmpeg process");

// Capture ffmpeg logs to Console.Error so the user can see them
process.ErrorDataReceived += (s, e) => { if (e.Data != null) Console.Error.WriteLine($"FFMpeg: {e.Data}"); };
process.BeginErrorReadLine();

byte[] buffer = new byte[sampleRate * channels * (bitsPerSample / 8) / 10]; // 100ms buffer

var readTask = Task.Run(async () =>
{
    try
    {
        using var stdout = process.StandardOutput.BaseStream;
        while (!cts.Token.IsCancellationRequested)
        {
            int bytesToRead = buffer.Length;
            int totalRead = 0;
            while (totalRead < bytesToRead)
            {
                int read = await stdout.ReadAsync(buffer, totalRead, bytesToRead - totalRead, cts.Token);
                if (read == 0)
                    break;
                totalRead += read;
            }

            if (totalRead == 0)
                break;

            byte[] dataToQueue = new byte[totalRead];
            Array.Copy(buffer, 0, dataToQueue, 0, totalRead);

            SdlSourceQueueResult queueResult;
            do
            {
                queueResult = device.QueueAudio(dataToQueue);
                if (device.GetQueuedAudioSize() > buffer.Length * 2) // Basic flow control
                {
                    await Task.Delay(10, cts.Token);
                }
            } while (queueResult != SdlSourceQueueResult.Success && !cts.Token.IsCancellationRequested);

            if (queueResult == SdlSourceQueueResult.Failed)
            {
                Console.WriteLine("QueueFrame Failed");
                break;
            }
        }

        Console.WriteLine("Waiting for playback to complete...");
        await device.WaitUntilPlayCompleteAsync(cancellationToken: cts.Token);
        Console.WriteLine("Playback finished.");
    }
    catch (OperationCanceledException) { }
    catch (Exception ex)
    {
        Console.WriteLine($"Error in playback: {ex.Message}");
    }
});

while (!readTask.IsCompleted)
{
    if (Console.KeyAvailable)
    {
        var keyInfo = Console.ReadKey(true);
        if (keyInfo.Key == ConsoleKey.Q)
        {
            cts.Cancel();
            try { process.Kill(); } catch { }
            break;
        }
    }
    await Task.Delay(100);
}

await readTask;
