using System;
using System.IO;
using Files.Manager;
using Files.AudioFiles;

namespace Sound
{
    /// <summary>
    /// Replacement for SubRoutine to handle the kTurnOnTV event. Used by radio, TV and UI music.
    /// </summary>
    public class HITTVOn
    {
        public HITTVOn(uint ID)
        {
            if(ID == 5) //Loadloop, play the sound directly.
            {
                ISoundCodec Loadloop = FileManager.GetSound(0x00004f85);
                SoundPlayer.PlaySound(Loadloop.DecompressedWav(), 5, Loadloop.GetSampleRate(), true, true);
            }
        }
    }
}
