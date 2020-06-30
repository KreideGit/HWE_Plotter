using System.Device.Gpio;

namespace Main
{
    /// <summary>
    /// Servo driver
    /// </summary>
    public class Servo
    {
        /// <summary>
        /// The current angle of the Servo
        /// </summary>
        public int Angle { get; private set; }

        private int _servoPin;
        private GpioController _controller;
        private int _frequency;

        /// <summary>
        /// Initializes the Servo driver
        /// </summary>
        /// <param name="servoPin">The pin of the PWM Control</param>
        /// <param name="controller">Instance of a GpioController</param>
        public Servo(int servoPin, GpioController controller)
        {
            Angle = 0;

            _servoPin = servoPin;
            _controller = controller;
            _frequency = 50;

            _controller.OpenPin(servoPin, PinMode.Output);

            _controller.Write(servoPin, PinValue.High);
        }

        /// <summary>
        /// Sets the angle of the Servo
        /// </summary>
        /// <param name="angle">The angle in degrees between 0 and 180</param>
        public void SetAngle(int angle)
        {
            float dutyCycle = angle / 180.0f * 100.0f;
            float onTime = 1 / _frequency * dutyCycle;
            float offTime = 1 / _frequency - onTime; 

            _controller.Write(_servoPin, PinValue.High);
            Utils.UDelay((long)(onTime * 1000000L));
            _controller.Write(_servoPin, PinValue.Low);
            Utils.UDelay((long)(offTime * 1000000L));

            Angle = angle;
        }
    }
}