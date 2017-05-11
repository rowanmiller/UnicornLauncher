using System;
using System.Threading.Tasks;
using Windows.Media.SpeechSynthesis;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UnicornLauncher
{
    // From https://blogs.windows.com/buildingapps/2016/05/23/using-speech-in-your-uwp-apps-from-talking-to-conversing/#MWvxUZ0veOpA0sm3.97
    static class MediaElementExtensions
    {
        public static async Task SpeakTextAsync(this MediaElement mediaElement, string text)
        {
            using (SpeechSynthesizer synthesizer = new SpeechSynthesizer())
            {
                var stream = await synthesizer.SynthesizeTextToStreamAsync(text);
                await mediaElement.PlayStreamAsync(stream, true);
            }
        }

        public static async Task PlayStreamAsync(
          this MediaElement mediaElement,
          IRandomAccessStream stream,
          bool disposeStream = true)
        {
            // bool is irrelevant here, just using this to flag task completion.
            TaskCompletionSource<bool> taskCompleted = new TaskCompletionSource<bool>();

            // Note that the MediaElement needs to be in the UI tree for events
            // like MediaEnded to fire.
            RoutedEventHandler endOfPlayHandler = (s, e) =>
            {
                if (disposeStream)
                {
                    stream.Dispose();
                }
                taskCompleted.SetResult(true);
            };
            mediaElement.MediaEnded += endOfPlayHandler;

            mediaElement.SetSource(stream, string.Empty);
            mediaElement.Play();

            await taskCompleted.Task;
            mediaElement.MediaEnded -= endOfPlayHandler;
        }
    }
}
