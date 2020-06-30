using System.Device.Gpio;

namespace Main
{
    /// <summary>
    /// 3-axis Plotter
    /// </summary>
    public class Plotter
    {
        private GpioController _controller;
        private Stepper _motorX;
        private Stepper _motorY;
        private Servo _motorZ;

        /// <summary>
        /// Initializes the plotter 
        /// </summary>
        public Plotter()
        {
            _controller = new GpioController();
            _motorX = new Stepper(new int[] { 20, 21, 17, 27, 22 }, _controller);
            _motorY = new Stepper(new int[] { 16, 12,  5,  6, 26 }, _controller);
            _motorZ = new Servo(14, _controller);
        }

        /// <summary>
        /// Moves the plotter to a specific location
        /// </summary>
        /// <param name="x">The x coordinate</param>
        /// <param name="y">The y coordinate</param>
        /// <param name="z">The z coordinate</param>
        public void MoveTo(int x, int y, int z)
        {
            if(x == GetX())
            {
                if (y < GetY())
                    for (int i = GetY() - 1; i > (y - 1); i--)
                        _motorY.MoveTo(i);
                else
                    for (int i = GetY() + 1; i < (y + 1); i++)
                        _motorY.MoveTo(i);

                _motorZ.SetAngle(z);
                return;
            }

            float k = (float)(y - GetY()) / (float)(x - GetX());
            float d = y - k * x;

            if (x < GetX())
            {
                for (int i = GetX() - 1; i > (x - 1); i--)
                {
                    _motorX.MoveTo(i);
                    _motorY.MoveTo((int)(k * i + d));
                }
            }   
            else
            {
                for (int i = GetX() + 1; i < (x + 1); i++)
                {
                    _motorX.MoveTo(i);
                    _motorY.MoveTo((int)(k * i + d));
                }
            }
            _motorZ.SetAngle(z);
        }

        /// <summary>
        /// Returns the position of the motor in x direction
        /// </summary>
        /// <returns></returns>
        public int GetX()
        {
            return _motorX.Position;
        }

        /// <summary>
        /// Returns the position of the motor in y direction
        /// </summary>
        /// <returns></returns>
        public int GetY()
        {
            return _motorY.Position;
        }

        /// <summary>
        /// Returns the position of the motor in z direction
        /// </summary>
        /// <returns></returns>
        public int GetZ()
        {
            return _motorZ.Angle;
        }
    }
}