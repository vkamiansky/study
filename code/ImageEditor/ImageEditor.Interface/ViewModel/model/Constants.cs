namespace ImageEditor.Interface.ViewModel.model
{
    public class Constants
    {
        private Constants()
        {
        }

        public const int ChannelsCount = 4;
        public const float ColorNormalizeRatio = 1 / 255f;
        public const float ColorDenormalizeRatio = 255f;
        public const float ScaleRatio = 1.1f / 120f;
        public const int BgTileSide = 8; //px
        public const float Grey = 204 * ColorNormalizeRatio;
        public const float White = 255 * ColorNormalizeRatio;
        public const float Opaque = 1f;
    }
}