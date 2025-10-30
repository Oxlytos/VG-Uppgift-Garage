using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingGarage
{
    internal class ParkingSpace
    {
        private int Size {get; set;}
        
        //Check automatically if its occupado
        public bool Occupied ()=> CheckIfOccupied ();

        //Keep track of the vehicle and the time occupied
        //Vehicle and Time basically
        public Dictionary<IVehicle, int> OccupyingVehicles { get; set; } = new();

        //Return available space directly
        public int RemainingSpace => CheckSpaceLeft();

        //Thing to print in the console
        public string RenterInfo {get; set;}
        public ParkingSpace()
        {
            Size = 2;
            RenterInfo = "Empty";
        }

        public void AssignSpace(IVehicle parkingVehicle)
        {
            //If there's space left that fits the boundaries of the vehicle
            if (RemainingSpace >= parkingVehicle.RequiredSpace) 
            {
                OccupyingVehicles[parkingVehicle] = parkingVehicle.RequiredSpace;
                string renterInfoString = parkingVehicle.GetRenterInfo();
            }
            else
            {
            }
        }


        public bool CheckIfOccupied()
        {
            return OccupyingVehicles.Values.Sum()>=Size;
        }

        public int CheckSpaceLeft()
        {
            return Size - OccupyingVehicles.Values.Sum();
            return Size;
        }
    }
}
