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
        public const float ScaleRatio = 0.001f;
        public const int BgTileSide = 8; //px
        public const float BgGrey = 204 * ColorNormalizeRatio;
        public const float BgWhite = 255 * ColorNormalizeRatio;
        public const float Opaque = 1f;
    }
}