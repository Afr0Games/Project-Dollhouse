using Files.Manager;
using Files.AudioFiles;

namespace Sound
{
    /// <summary>
    /// Replacement for SubRoutine to handle the kTurnOnTV event. Used by radio, TV and UI music.
    /// </summary>
    public class HITTVOn
    {
        private uint m_ID = 0;

        /// <summary>
        /// The SoundID for the sound that this HITTVOn event handles.
        /// </summary>
        public uint SoundID
        {
            get { return m_ID; }
        }

        public HITTVOn(uint ID)
        {
            m_ID = ID;

            if(ID == 5) //Loadloop, play the sound directly.
            {
                ISoundCodec Loadloop = FileManager.GetSound(0x00004f85);
                SoundPlayer.PlaySound(Loadloop.DecompressedWav(), 5, Loadloop.GetSampleRate(), true, true);
            }
        }
    }
}
