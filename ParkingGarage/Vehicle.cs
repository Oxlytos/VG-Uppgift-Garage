using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingGarage
{
    //Gör till interface klass
    public abstract class IVehicle
    {
        public string RegNr { get; set; }

        public string VehicleColor { get; set; }

        public int RequiredSpace {  get; set; }

        internal string Sprite = "";

        public DateTime Parked;


        internal IVehicle(string color)
        {
            RegNr = Helpers.RandomReg();
            VehicleColor = color;
        }

        //Empty abstract void to be overwritten elsewhere
        public abstract void GetParkingSpaceRequirment();

        public abstract string GetRenterInfo();
    }

    public class Car : IVehicle
    {
        public bool Electric { get; set; }
        public Car(string _color, bool _electric) : base(_color)
        {
            this.RequiredSpace = 2;
            this.Electric = _electric;
            this.Sprite = "🚗";
        }

        public override void GetParkingSpaceRequirment()
        {
            Console.WriteLine("A car needs 2 spaces to park comfortably");
        }

        public override string GetRenterInfo() 
        {
            string yesNoElectric = this.Electric ? "Yes" : "No";
            return $"{this.GetType().Name} {this.Sprite} REG: {this.RegNr} | Color: {this.VehicleColor} | Electric?: {yesNoElectric}";
        }
    }
    public class Motorcycle : IVehicle
    {
        public string BikeMake { get; set; }
        public Motorcycle(string _color, string _bikeMake) : base(_color)
        {
            this.BikeMake = _bikeMake;
            this.RequiredSpace = 1;
            this.Sprite = "🏍️";

        }
        public override void GetParkingSpaceRequirment()
        {
            Console.WriteLine("A MC needs 1 space to park, and can park with other MCs");
        }

        public override string GetRenterInfo()
        {
            return $"{this.GetType().Name} {this.Sprite} REG: {this.RegNr} | Color: {this.VehicleColor} | Make: {this.BikeMake}";
        }
    }

    class Bus : IVehicle
    {
        public int MaxPassagerCount { get; set; }
        public Bus(string _color, int _maxPassagerCount) : base(_color)
        {
            this.MaxPassagerCount = _maxPassagerCount;
            this.RequiredSpace = 4;
            this.Sprite = "🚌";
        }
        public override void GetParkingSpaceRequirment()
        {
            Console.WriteLine("A bus needs 2 consecutive spaces in a row to be clear of other vehicles to park properly");
        }

        public override string GetRenterInfo()
        {
            return $"{this.GetType().Name} {this.Sprite} REG: {this.RegNr} | Color: {this.VehicleColor} | Max Capacity: {this.MaxPassagerCount}";
        }
    }
}
