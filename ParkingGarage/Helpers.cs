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
            string reg ="";

            //Alphabet A-z
            string alphabet = "A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z";

            //Remove spaces
            alphabet = alphabet.Replace(" ", "");

            //Divide int o substrings
            string[] letters = alphabet.Split(",");

            //For loop for 3 letters
            for(int i = 0; i<3; i++)
            {
                //Expand this string
                reg += letters[rand.Next(0, letters.Length)];
            }
            //3 Numbers
            for(int i = 0; i<3; i++)
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
            return allSpaces.Any(ps => ps.RemainingSpace >= carDummy.RequiredSpace);
        }
        internal static bool CheckIfWeCanFitMC(List<ParkingSpace> allSpaces)
        {
            Motorcycle mcDummy = new Motorcycle("", "");
           //Any space can fit an MC
            return allSpaces.Any(ps => ps.RemainingSpace >= mcDummy.RequiredSpace);
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
            return false;

        }
        static internal List<ParkingSpace> FindViableNeighbouringSpot(List<ParkingSpace> parkingSpaces, IVehicle vehicle)
        {
            //If its empty, park at earliest position and leave func
            if (!parkingSpaces.Any(ps => ps.Occupied()))
            {
                return new List<ParkingSpace> { parkingSpaces[0] };
            }

            //Count up through the list
            for (int s = 0; s < parkingSpaces.Count; s++)
            {
                if (!parkingSpaces[s].Occupied())
                {
                    continue;
                }
                //Count and check if there's more space to the right or left
                int freeSpacesToTheLeft = 0;
                int freeSpacesToTheRight = 0;

                //Increase left count
                for (int l = s - 1; l >= 0; l--)
                {
                    if (!parkingSpaces[l].Occupied() && parkingSpaces[l].RemainingSpace >= vehicle.RequiredSpace)
                    {
                        freeSpacesToTheLeft++;
                    }
                    else
                    {
                        break;
                    }
                }
                //Then the right
                for (int r = s + 1; r < parkingSpaces.Count; r++)
                {
                    if (!parkingSpaces[r].Occupied() && parkingSpaces[r].RemainingSpace >= vehicle.RequiredSpace)
                    {
                        freeSpacesToTheRight++;
                    }
                    else
                    {
                        break;
                    }
                }

                //Console.WriteLine($"There's {freeSpacesToTheLeft} spaces free to the left and {freeSpacesToTheRight} spaces to the right");
                if (vehicle is Motorcycle)
                {
                    var sharedSpace = parkingSpaces.FirstOrDefault(ps => ps.OccupyingVehicles.Keys.OfType<Motorcycle>().Count() < 2 && ps.RemainingSpace >= 1);

                    if (sharedSpace != null)
                    {
                        return new List<ParkingSpace> { sharedSpace };
                    }
                }


                //Check for neightbouring spots to park with a bus
                if (vehicle is Bus)
                {

                    // Count up => To the right
                    for (int busRightCheckIndex = s; busRightCheckIndex < parkingSpaces.Count - 1; busRightCheckIndex++)
                    {
                        //If not occupied with 2 conscecutive spaces
                        if (!parkingSpaces[busRightCheckIndex].Occupied() && !parkingSpaces[busRightCheckIndex + 1].Occupied())
                        {
                            return new List<ParkingSpace> { parkingSpaces[busRightCheckIndex], parkingSpaces[busRightCheckIndex + 1] };
                        }

                    }

                    // Check right from left if that helps at all
                    for (int busLeftCheckIndex = s; busLeftCheckIndex > 0; busLeftCheckIndex--)
                    {
                        if (!parkingSpaces[busLeftCheckIndex - 1].Occupied() && !parkingSpaces[busLeftCheckIndex].Occupied())
                        {
                            return new List<ParkingSpace> { parkingSpaces[busLeftCheckIndex - 1], parkingSpaces[busLeftCheckIndex] };
                        }
                    }
                   
                }
                else
                {

                    //If there's space at all from a neighbouring vehicle
                    if (freeSpacesToTheLeft > 0 || freeSpacesToTheRight > 0)
                    {
                        //If there's less space to the left than the right
                        if (freeSpacesToTheLeft <= freeSpacesToTheRight && freeSpacesToTheLeft > 0)
                        {
                            //Current S in spaces -1 for left
                            if (s - 1 >= 0)
                            {
                                return new List<ParkingSpace> { parkingSpaces[s - 1] };
                            }

                        }

                        //If there's space to the right, and more to the left
                        else if (freeSpacesToTheRight > 0)
                        {
                            Console.WriteLine(parkingSpaces[s + 1].RenterInfo);
                            //Current s in spaces +1 for right
                            if (s + 1 < parkingSpaces.Count)
                            {
                                return new List<ParkingSpace> { parkingSpaces[s + 1] };
                            }

                        }
                    }
                }
            }
            return null;
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

      
        /* static List<ParkingSpace> FindBussSpot(List<ParkingSpace> parkingSpaces)
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
       }*/

    }



}
