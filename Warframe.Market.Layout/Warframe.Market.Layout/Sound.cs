using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace Warframe.Market.Layout
{
    public class Sound
    {
        private static void PlaySound(UnmanagedMemoryStream stream)
        {
            if(!MainFrame.muted)
            {
                SoundPlayer audio = new SoundPlayer(stream);
                audio.Play();
            }
        }

        public static void PlayUndercutSound()
        {
            // PlaySound(Properties.Resources.to_the_point_online_audio_converter_com);
        }

        public static void PlayDealsSound()
        {
            //PlaySound(Properties.Resources.);
        }
    }
}
