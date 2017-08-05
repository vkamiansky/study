using ImageEditor.Interface.ViewModel.model;

namespace ImageEditor.scalers
{
    public class ScalerChooser
    {
        private static ScalerChooser _instance;
        
        private ScalerChooser(){}
        
        public static ScalerChooser Instance => _instance ?? (_instance = new ScalerChooser());

        private BilinearScaler _bilinearScaler = new BilinearScaler();
        private NearestNeighbourScaler _nearestNeighbourScaler = new NearestNeighbourScaler();
        private StubScaler _stubScalerr = new StubScaler();
        
        public ImageScaler ChooseScaler(float scale)
        {
            if (scale == 1)
            {
                return new StubScaler();
            }
            if (scale < 2)
            {
                return new BilinearScaler();
            }
            return new NearestNeighbourScaler();
        }
       
    }
}