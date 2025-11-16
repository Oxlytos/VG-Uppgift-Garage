using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingGarage
{
    class Helpers
    {
        static Random rand = new Random();
        public static string RandomReg()
        {
            //String to return
            string reg = "";

            //Alphabet A-z
            string alphabet = "A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z";

            //Remove spaces
            alphabet = alphabet.Replace(" ", "");

            //Divide int o substrings
            string[] letters = alphabet.Split(",");

            //For loop for 3 letters
            for (int i = 0; i < 3; i++)
            {
                //Expand this string
                reg += letters[rand.Next(0, letters.Length)];
            }
            //3 Numbers
            for (int i = 0; i < 3; i++)
            {
                //0-9
                reg += rand.Next(0, 10);
            }


            return reg;
        }

        public static string RandomColor()
        {
            string[] colors = {
                "Red",
                "Orange",
                "Yellow",
                "Green",
                "Blue",
                "Purple",
                "Pink",
                "Brown",
                "Gray",
                "Black"
            };

            string randoColor = colors[rand.Next(0, colors.Length)];
            return randoColor;
        }


        public static string RenterString(Car vec)
        {
            string yesNoElectric = vec.Electric ? "Yes" : "No";
            return $"{vec.GetType().Name} REG: {vec.RegNr} | Color: {vec.VehicleColor} | Electric?: {yesNoElectric}";
        }
        public static string RenterString(Motorcycle vec)
        {
            return $"{vec.GetType().Name} REG: {vec.RegNr} | Color: {vec.VehicleColor} | Make: {vec.BikeMake}";
        }
        public static string RenterString(Bus vec)
        {
            return $"{vec.GetType().Name} REG: {vec.RegNr} | Color: {vec.VehicleColor} | Max Capacity: {vec.MaxPassagerCount}";
        }

        internal static bool CheckIfWeCanFitCar(List<ParkingSpace> allSpaces)
        {
            Car carDummy = new Car("", false);
            //Return true if any space can fit a car
            bool canFit = allSpaces.Any(ps => ps.RemainingSpace >= carDummy.RequiredSpace);
            if (!canFit)
            {
                carDummy.GetParkingSpaceRequirment();
            }
            return canFit;
        }
        internal static bool CheckIfWeCanFitMC(List<ParkingSpace> allSpaces)
        {
            Motorcycle mcDummy = new Motorcycle("", "");
            //Any space can fit an MC
            bool canFit = allSpaces.Any(ps => ps.RemainingSpace >= mcDummy.RequiredSpace);
            if (!canFit)
            {
                mcDummy.GetParkingSpaceRequirment();
            }
            return canFit;
        }
        internal static bool CheckIfBusCanFit(List<ParkingSpace> allSpaces)
        {
            Bus busDummy = new Bus("", 20);
            for (int i = 0; i < allSpaces.Count; i++)
            {
                if (i + 1 < allSpaces.Count)
                {
                    if (!allSpaces[i].Occupied() && !allSpaces[i + 1].Occupied())
                    {
                        return true;
                    }
                }

            }
            busDummy.GetParkingSpaceRequirment();
            return false;

        }
        //Finding a neighbour to park next to is better than parkin in the middle a obstructing possible busses to park
        static internal List<ParkingSpace> FindViableNeighbouringSpot(List<ParkingSpace> parkingSpaces, IVehicle vehicle)
        {
            List<ParkingSpace> bestSpaces = null;
            int spaceReq = -1;
            //If its empty, park at earliest position and leave func
            if (!parkingSpaces.Any(ps => ps.Occupied()))
            {
                //If its a bus, give it the first 2 spaces
                if (vehicle is Bus)
                {
                    return new List<ParkingSpace> { parkingSpaces[0], parkingSpaces[1] };
                }
                else
                {
                    return new List<ParkingSpace> { parkingSpaces[0] };
                }

            }
            //Find a shared space
            if (vehicle is Motorcycle)
            {
                Console.WriteLine("Lets see if this MC can find a shared space");
                var sharedSpace = parkingSpaces.FirstOrDefault(ps => ps.OccupyingVehicles.Keys.OfType<Motorcycle>().Count() < 2 && ps.RemainingSpace == 1);

                if (sharedSpace != null)
                {
                    Console.WriteLine("This MC will be sharing with another MC");
                    return new List<ParkingSpace> { sharedSpace };
                }
                else
                {
                    Console.WriteLine("We did not in fact find a shared space");
                }
            }
            //Just give the bus any 2 spots
            if (vehicle is Bus)
            {
                for (int i = 0; i < parkingSpaces.Count - 1; i++)
                {
                    if (!parkingSpaces[i].Occupied() && !parkingSpaces[i + 1].Occupied())
                        return new List<ParkingSpace> { parkingSpaces[i], parkingSpaces[i + 1] };
                }
            }

            //Count up through the list
            for (int s = 0; s < parkingSpaces.Count; s++)
            {
                if (!parkingSpaces[s].Occupied())
                {
                    continue;
                }
                //Left
                if(s - 1 >= 0 && !parkingSpaces[s - 1].Occupied() && parkingSpaces[s - 1].RemainingSpace >= vehicle.RequiredSpace)
                {
                    return new List<ParkingSpace> { parkingSpaces[s - 1] };
                }

                //Right
                if (s + 1 < parkingSpaces.Count && !parkingSpaces[s + 1].Occupied() && parkingSpaces[s + 1].RemainingSpace >= vehicle.RequiredSpace)
                {
                    return new List<ParkingSpace> { parkingSpaces[s + 1] };
                }
                var anySpot = parkingSpaces.FirstOrDefault(ps => !ps.Occupied() && ps.RemainingSpace >= vehicle.RequiredSpace);
                if (anySpot != null)
                    return new List<ParkingSpace> { anySpot };

            }
            return bestSpaces;
        }
        internal static void InformUserOfParkingError()
        {
            Console.WriteLine("Sadly we can't park this vehicle at the current moment, please come back later");
            Console.ReadLine();
        }

        internal static string AskColour()
        {
            Console.WriteLine("Color?");
            string colour = Console.ReadLine();
            if (string.IsNullOrEmpty(colour))
            {
                colour = "Undefined";
            }
            return colour;
        }


        static List<ParkingSpace> FindBussSpot(List<ParkingSpace> parkingSpaces)
        {
            List<ParkingSpace> bussSpots = new List<ParkingSpace>();

            for (int spotA = 0; spotA < parkingSpaces.Count; spotA++)
            {
                int spotB = spotA + 1;
                if (spotB >= parkingSpaces.Count)
                {
                    break;
                }

                if (parkingSpaces[spotA].Occupied() == false && parkingSpaces[spotB].Occupied() == false)
                {
                    bussSpots.Add(parkingSpaces[spotA]);
                    bussSpots.Add(parkingSpaces[spotB]);
                    break;
                }
            }
            if (bussSpots.Count == 2)
            {
                return bussSpots;
            }
            else
            {
                return null;
            }
        }

    }



}
