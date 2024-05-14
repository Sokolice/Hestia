
using System;
using System.Threading.Tasks;
using Windows.Media.Capture;

namespace Hestia.ViewModel
{
    public class AudioCapturePermissions
    {
         private static int NoCaptureDevicesHResult = -1072845856;

        public async static Task<bool> RequestMicrophonePermission()
        {
            try
            {
                MediaCaptureInitializationSettings settings = new MediaCaptureInitializationSettings();
                settings.StreamingCaptureMode = StreamingCaptureMode.Audio;
                settings.MediaCategory = MediaCategory.Speech;
                MediaCapture capture = new MediaCapture();

                await capture.InitializeAsync(settings);
            }
            catch (UnauthorizedAccessException)
            {
                var messageDialog = new Windows.UI.Popups.MessageDialog("Přístup k mikrofonu byl odepřen. Pro správnou funčnost hlasového ovládání povolte " +
                                                                         "přístup \nk mikrofonu v nastavení systému a restartujte aplikaci.");
                messageDialog.Commands.Add(new Windows.UI.Popups.UICommand("OK") { Id = 0 });
                messageDialog.DefaultCommandIndex = 0;
                await messageDialog.ShowAsync();
                return false;
            }
            catch (Exception exception)
            {
                if (exception.HResult == NoCaptureDevicesHResult)
                {
                    var messageDialog = new Windows.UI.Popups.MessageDialog("No Audio Capture devices are present on this system.");
                    await messageDialog.ShowAsync();
                    return false;
                }
                else
                {
                    throw;
                }
            }
            return true;
        }
    }
}
