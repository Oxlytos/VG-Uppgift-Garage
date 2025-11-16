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
        //A parking space has a size of 2
        private int Size {get; set;}
        
        //Occupation is dependent on if count is > 0
        public bool Occupied ()=> CheckIfOccupied();

        //Vehicles on this spot
        //Vehicle object as the KEY, then the required space as the VALUE
        public Dictionary<IVehicle, int> OccupyingVehicles { get; set; } = new();

        //Return available space directly, if 2 MC => RemainingSpace = Size(2) - Vehicles (2) = 0
        //We can have 1 space left if there's just a MC parked here
        public int RemainingSpace => Size - OccupyingVehicles.Values.Sum();

        //Thing to print in the console
        public string RenterInfo {get; set;}
        public ParkingSpace()
        {
            Size = 2;
            RenterInfo = "Available";
        }

        public void AssignSpace(IVehicle parkingVehicle)
        {
            //If there's space left that fits the boundaries of the vehicle
            if (RemainingSpace >= parkingVehicle.RequiredSpace) 
            {
              //  OccupyingVehicles[parkingVehicle] = parkingVehicle.RequiredSpace;
                string renterInfoString = parkingVehicle.GetRenterInfo();
                if (OccupyingVehicles.Count == 0)
                {
                    RenterInfo = "\n"+renterInfoString;
                }
                else
                {
                    RenterInfo += "\n" + renterInfoString;
                }
                parkingVehicle.Parked = DateTime.Now;
                OccupyingVehicles[parkingVehicle] = parkingVehicle.RequiredSpace;
            }
            // if its a bus
            else if (parkingVehicle is Bus)
            {
                if(RemainingSpace >= 2)
                {
                    string renterInfoString = parkingVehicle.GetRenterInfo();
                    if (OccupyingVehicles.Count == 0)
                    {
                        RenterInfo = "\n"+renterInfoString;
                    }
                    parkingVehicle.Parked = DateTime.Now;
                    OccupyingVehicles[parkingVehicle] = parkingVehicle.RequiredSpace;
                }
            }
            
        }

        public bool VehicleExuent(string reg)
        {
            //All vehicles to be removed on this spot
            var vehiclesToRemove = OccupyingVehicles.Keys
               .Where(v => v.RegNr.Equals(reg, StringComparison.OrdinalIgnoreCase))
               .ToList();

            if (vehiclesToRemove != null) 
            {
                //Make relevant vehicles leave
                foreach (var vehicle in vehiclesToRemove)
                {
                    Garage.MoreEarnings(vehicle.Parked);
                    OccupyingVehicles.Remove(vehicle);
                }

                //If there's one left i.e. a MC
                if (OccupyingVehicles.Count > 0)
                {
                    //Reset string
                    string renterInfo = "";
                    foreach (var remVec in OccupyingVehicles.Keys)
                    {
                        renterInfo += remVec.GetRenterInfo();
                    }

                    //Resets the string nice and tidy
                    RenterInfo = renterInfo.TrimEnd();
                }
                else
                {
                    //If its empty, its available
                    RenterInfo = "Available";
                }
                return true;
            }
            else
            {

                return false;
            }

        }


        public bool CheckIfOccupied()
        {
            return OccupyingVehicles.Count > 0;
        }

        
    }
}
