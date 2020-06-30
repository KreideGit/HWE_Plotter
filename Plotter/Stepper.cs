using System.Device.Gpio;

namespace Main
{
    /// <summary>
    /// Enumeration for the direction of the stepper
    /// </summary>
    public enum Direction
    {
        Clockwise = 1, 
        CounterClockwise = -1
    }

    /// <summary>
    /// Stepper driver A4988
    /// </summary>
    public class Stepper
    {
        /// <summary>
        /// Current position of the stepper
        /// </summary>
        public int Position { get; private set; }
        /// <summary>
        /// Current direction of the stepper
        /// </summary>
        public Direction Direction { get; private set; }

        private int _stepPin;
        private int _directionPin;
        private GpioController _controller;

        /// <summary>
        /// Initializes the stepper driver and the pins
        /// </summary>
        /// <param name="pins">[0]Step pin, [1]Direction pin, [2-4]Resolution pins</param>
        /// <param name="controller">Controller to enable pins</param>
        public Stepper(int[] pins, GpioController controller)
        {
            Position = 0;
            Direction = Direction.Clockwise;

            _stepPin = pins[0];
            _directionPin = pins[1];
            _controller = controller;
        
            _controller.OpenPin(_stepPin, PinMode.Output);
            _controller.OpenPin(_directionPin, PinMode.Output);
            
            _controller.OpenPin(pins[2], PinMode.Output);
            _controller.OpenPin(pins[3], PinMode.Output);
            _controller.OpenPin(pins[4], PinMode.Output);

            _controller.Write(pins[2], PinValue.High);
            _controller.Write(pins[3], PinValue.High);
            _controller.Write(pins[4], PinValue.High);
        }

        /// <summary>
        /// Moves the stepper to a specific location
        /// </summary>
        /// <param name="value">The new location</param>
        public void MoveTo(int value)
        {
            SetDirection(value > Position ? Direction.Clockwise : Direction.CounterClockwise);

            while(value - Position != 0)
            {
                Step();
                Position += (int)Direction;
            }
        }

        /// <summary>
        /// Moves the stepper
        /// </summary>
        /// <param name="value">The amount to move</param>
        private void Step()
        {
            _controller.Write(_stepPin, PinValue.High);
            Utils.UDelay(100);
            _controller.Write(_stepPin, PinValue.Low);
            Utils.UDelay(100);
        }

        /// <summary>
        /// Sets the direction of the stepper
        /// </summary>
        /// <param name="direction">The new direction</param>
        private void SetDirection(Direction direction)
        {
            _controller.Write(_directionPin, (direction == Direction.Clockwise) ? PinValue.High : PinValue.Low);
            Direction = direction;
        }
    }
}