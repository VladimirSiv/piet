namespace Piet.Core
{
    public class Measure
    {
        public int width { get; private set; }
        public int height { get; private set; }

        public Measure(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }
}
