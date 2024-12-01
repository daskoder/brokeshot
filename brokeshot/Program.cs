using LibreHardwareMonitor.Hardware;
using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace HardwareToESP32
{
    class Program
    {
        private static Computer _computer;
        private static SerialPort _serialPort;

        static void Main(string[] args)
        {
            // Set up the SerialPort for communication with ESP32
            _serialPort = new SerialPort("COM3", 9600); // Use the correct COM port
            _serialPort.Open();

            // Initialize hardware monitor
            _computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true
            };
            _computer.Open();

            Console.WriteLine("Monitoring hardware and sending data to ESP32...");

            // Monitor hardware and send data every second
            while (true)
            {
                string data = GetHardwareTemps();
                SendDataToESP32(data);
                Thread.Sleep(1000); // Update every second
            }
        }

        // Function to collect only CPU and GPU temperatures, rounding the CPU temperature
        static string GetHardwareTemps()
        {
            string result = "";

            foreach (var hardware in _computer.Hardware)
            {
                hardware.Update();

                // CPU Monitoring: Get the first CPU Core temperature and round it
                if (hardware.HardwareType == HardwareType.Cpu)
                {
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature && sensor.Name.Contains("Core"))
                        {
                            double cpuTemp = sensor.Value.GetValueOrDefault();
                            double roundedCpuTemp = Math.Round(cpuTemp, 1); // Round CPU temperature to 1 decimal place
                            result += "CPU Temp: " + roundedCpuTemp + "C, ";
                            break; // Only take the first CPU core temperature
                        }
                    }
                }

                // GPU Monitoring: Get the first GPU temperature (Nvidia or AMD)
                if (hardware.HardwareType == HardwareType.GpuNvidia || hardware.HardwareType == HardwareType.GpuAmd)
                {
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            result += "GPU Temp: " + sensor.Value.GetValueOrDefault() + "C, ";
                            break; // Only take the first GPU temperature
                        }
                    }
                }
            }

            return result.TrimEnd(','); // Remove last comma
        }

        // Function to send the collected data to ESP32 via Serial
        static void SendDataToESP32(string data)
        {
            try
            {
                _serialPort.WriteLine(data); // Send data to the ESP32
                Console.WriteLine("Sent data: " + data);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending data: " + ex.Message);
            }
        }
    }
}
