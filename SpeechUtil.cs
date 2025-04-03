using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple_Bible_Reader
{
    public class SpeechUtil
    {
        public static void speak(string text)
        {
            try
            {
                //
                SpeechLib.SpVoice oVoice = new SpeechLib.SpVoice();
                oVoice.Volume = GlobalMemory.getInstance().Mp3Volume;
                oVoice.Rate = GlobalMemory.getInstance().Mp3Speed;
                oVoice.Voice = oVoice.GetVoices().Item(GlobalMemory.getInstance().Mp3VoiceIndex);
                if (GlobalMemory.getInstance().Mp3Transliterate)
                    oVoice.Speak(Unidecoder.Unidecode(text), SpeechLib.SpeechVoiceSpeakFlags.SVSFDefault);
                else
                    oVoice.Speak(text, SpeechLib.SpeechVoiceSpeakFlags.SVSFDefault);
                oVoice = null;
            }
            catch (Exception)
            {
                Themes.MessageBox("Text to speech not supported.");
            }
            
        }
    }
}
