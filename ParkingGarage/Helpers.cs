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
        //We do this with chunks, explained in the helper function
        static internal List<ParkingSpace> FindViableNeighbouringSpot(List<ParkingSpace> parkingSpaces, IVehicle vehicle)
        {
            //If its empty, park at earliest position and leave method
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
            //Find a reference to all EMPTY chunks
            //A chunk is the space between occupied spaces
            //O = Occupied, E = Empty
            //O {E E} O O O {E} O {E E E E} O {E E}
            //Every {} is a chunk in this case
            var chunks = FindEmptyChunks(parkingSpaces);

            //Bus park with 2 spaces, find a chunk of 2
            if(vehicle is Bus)
            {
                foreach(var chunk in chunks)
                {
                    //If there's a chunk with 2 spaces or more
                    if (chunk.Count >= 2)
                    {
                        //Park there
                        //Take takes the first 2 it an find, like SELECT TOP in SQL
                        //Literally just the first 2 things
                        return chunk.Take(2).ToList();
                    }
                }
            }
            //Car or MC
            else if(vehicle is Car||vehicle is Motorcycle)
            {
                //Find a chunk with just one or more spaces, odrder by count (
                var smallChunk = chunks.Where(c=>c.Count>=1).OrderBy(c=>c.Count).FirstOrDefault();
                if(smallChunk != null)
                {
                    //Find a share space if its a MC
                    if(vehicle is Motorcycle)
                    {
                        Console.WriteLine("Lets see if this MC can find a shared space");
                        //Find the first MC to park next to, in occuping keys as a MC and if there's 1 space left
                        var sharedSpace = parkingSpaces.FirstOrDefault(ps => ps.OccupyingVehicles.Keys.OfType<Motorcycle>().Count() < 2 && ps.RemainingSpace == vehicle.RequiredSpace);

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
                    //Small chunk should be available always here
                    return new List<ParkingSpace> { smallChunk[0] };
                }
            }
            return null;

            
        }

        //Nested List
        //O = Occupied, E = Empty
        //In total it might be O E O E E O E O E E O O
        //That would be 4 chunks of empty parkable areas
        //The list are Like: {E}, {E, E}, {E}, {E, E}
        //Like a jagged array, but lists, read more at;
        //https://www.dotnetperls.com/nested-list
        //https://stackoverflow.com/questions/23015171/how-to-get-display-item-from-nested-listlistint-in-c-sharp
        internal static List<List<ParkingSpace>> FindEmptyChunks(List<ParkingSpace> spaces)
        {
            //Big important list of UnOccopiedSpaces
            List<List<ParkingSpace>> chunks = new List<List<ParkingSpace>>();

            //Outer list with the current chunk, temp variaböe
            List<ParkingSpace> currentChunk = new List<ParkingSpace>();


            //Loop through
            foreach (var space in spaces)
            {
                //If empty => Its a new empty chunk
                if (!space.Occupied())
                {
                    //Add this chunk
                   // Console.WriteLine("Adding this space to current chunk");
                    currentChunk.Add(space);
                    //Console.WriteLine("Current chunk is at: " + currentChunk.Count);
                }
                //Occupied space
                else
                {
                  //  Console.WriteLine("Found a occupied spot, ending this chunk");
                    //End a chunk, if this CurrentChunk already has some stuff in it
                    if(currentChunk.Count > 0)
                    {
                        //Console.WriteLine("Total chunks before adding newest one: " +  chunks.Count);
                        //Add this new completed chunk of empty spaces, that we can use later
                        chunks.Add(new List<ParkingSpace>(currentChunk));

                        //Console.WriteLine("Chunks after adding latest one: " + chunks.Count);
                        //Clear the list of current spaces in this temp chunk handler
                        //We keep track of unoccioied spaces, add it to the list list chunk, then clear this list for keeping track of new chunks
                        currentChunk.Clear();
                    }
                }
            }
            //Make sure to add this last chunk if the last parking space didn't have a occupying vehicle
            if (currentChunk.Count > 0)
            {
               // Console.WriteLine("We got one last chunk with a size of: " + currentChunk.Count);
              //  Console.WriteLine("Adding last chunk to chunks, chunks at: " + chunks.Count);
                chunks.Add(new List<ParkingSpace>(currentChunk));
              
            }

          /*  Console.WriteLine("Last count of chunks: " + chunks.Count);
            Console.WriteLine("The chunks look like this;\n");
            
            for(int i = 0; i<chunks.Count; i++)
            {
                Console.WriteLine($"Chunk {i} size is {chunks[i].Count} aka that many spaces");
            }

            //Example access to first spot in first chunk
           // Console.WriteLine(chunks[0][0].RemainingSpace.ToString());*/
            return chunks;
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

        /*
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
        }*/

    }



}
