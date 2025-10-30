using System.Text;

namespace ParkingGarage
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Start simulation
            Console.OutputEncoding = Encoding.UTF8;
            Garage.Run();
        }
    }
}
