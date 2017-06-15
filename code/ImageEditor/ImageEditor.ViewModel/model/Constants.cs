using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEditor.ViewModel.model
{
    public class Constants
    {
        private Constants()
        {
        }

        public const int ChannelsCount = 4;
        public const float ColorNormalizeRatio = 1 / 255f;
        public const float ColorDenormalizeRatio = 255f;
    }
}